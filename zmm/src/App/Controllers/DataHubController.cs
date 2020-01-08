using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ZMM.Models.ResponseMessages;
using System.Linq;
using ZMM.Models.Payloads;
using ZMM.Helpers.ZMMDirectory;
using ZMM.Helpers.Common;

namespace ZMM.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DataHubController : Controller
    {
        #region Variables
        private readonly IWebHostEnvironment _environment;
        private IConfiguration Configuration { get; }
        readonly ILogger<DataHubController> Logger;
        private List<DataResponse> responseData;

        #endregion

        #region Constructor
        public DataHubController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<DataHubController> log)
        {
            this.Configuration = configuration;
            this.Logger = log;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        #endregion

        #region Post sql
        [HttpPost]
        public async Task<IActionResult> PostSqlAsync()
        {
            string reqBody = "";
            string reqSql = "";
            string dirFullpath = $"{DirectoryHelper.GetDataDirectoryPath()}";
            string newFile = "DataHub_" + DateTime.Now.Ticks.ToString()+".csv";
            string _filePath = Path.Combine(dirFullpath, newFile);
            long fileSize = 0L;
            List<string> resultRows = new List<string>();
            StringBuilder csvBuilder = new StringBuilder();
            List<Property> _props = new List<Property>();
            int numberOfColumns = 0;
            //
            using (var reader = new StreamReader(Request.Body))
            {
                reqBody = reader.ReadToEnd().ToString();
            }
            JObject jsonBody = JObject.Parse(reqBody);
            reqSql = jsonBody["sql"].ToString();
            //


            #region ODBC

            using (OdbcConnection connection = new OdbcConnection("Driver=Dremio Connector;ConnectionType=Direct;HOST=dremio-demo.westeurope.cloudapp.azure.com;PORT=31010;AuthenticationType=Plain;UID=;PWD="))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("DATAHUB CONNECTION ESTABLISHED...");
                    OdbcCommand DbCommand = connection.CreateCommand();
                    Console.WriteLine(reqSql);
                    DbCommand.CommandText = reqSql;
                    List<string> columns = new List<string>();
                    OdbcDataReader reader = DbCommand.ExecuteReader();
                    numberOfColumns = reader.FieldCount;
                    //add header column name
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                        csvBuilder.Append(reader.GetName(i));
                        if (i < reader.FieldCount - 1) csvBuilder.Append(",");
                    }
                    resultRows.Add(csvBuilder.ToString());
                    csvBuilder.Clear();
                    //
                    //add rows
                    while (reader.Read())
                    {
                        csvBuilder.Clear();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (!reader.IsDBNull(i))
                            {
                                csvBuilder.Append(reader[i]);
                            }
                            else
                            {
                                csvBuilder.Append("No Data");
                            }
                            if (i < reader.FieldCount - 1) csvBuilder.Append(",");
                        }
                        resultRows.Add(csvBuilder.ToString());
                    }
                }
                catch (Exception e)
                {
                    // return error message
                    Console.WriteLine("DataHub ERROR:>>>>" + e.Message);
                }
            }

            #endregion


            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                foreach (var line in resultRows)
                    writer.WriteLine(line);

                writer.Flush();
                fileSize = writer.BaseStream.Length;
            }
            string _url = DirectoryHelper.GetDataUrl(newFile);
            await Task.FromResult(0);
            //
            string type = "CSV";
            //get properties row and column count
            int[] csvProps = CsvHelper.GetCsvRowColumnCount(dirFullpath + @"/" + newFile);
            _props.Add(new Property { key = "Number of rows", value = resultRows.Count().ToString() });
            _props.Add(new Property { key = "Number of columns", value = numberOfColumns.ToString() });

            DataResponse newRecord = new DataResponse()
            {
                Created_on = DateTime.Now.ToString(),
                Edited_on = DateTime.Now.ToString(),
                Extension = "CSV",
                FilePath = _filePath,
                Id = newFile,
                MimeType = "text/csv",
                Name = newFile.Replace(".CSV", ""),
                Properties = _props,
                Size = fileSize,
                Type = type,
                Url = _url,
                DateCreated = DateTime.Now
            };
            DataPayload.Create(newRecord);
            //
            return Json(newRecord);
        }

        #endregion
    }
}