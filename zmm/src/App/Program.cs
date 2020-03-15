using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ZMM.App
{
    public class Program
    {
        #region Variables to get Web Hosting configurations i.e. Http or Https port       
        private static IConfiguration Configuration { get; set; }
        private static readonly string AppSettingsFilePrefix = "appsettings";
        private static readonly string AppSettingsFileExtension = ".json";
        private static readonly string AppSettingsDevelopmentName = ".Development";
        #endregion
        public static void Main(string[] args)
        {
            ConfigurationBuilder Builder = new ConfigurationBuilder();
            Builder.SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables().AddJsonFile(GetAppSettingFile(args));
            Configuration = Builder.Build(); 
            IWebHostBuilder HostBuilder = CreateWebHostBuilder(args);
            HostBuilder.UseUrls("http://+:" + Configuration["WebHosting:HttpPort"] +";https://+:" + Configuration["WebHosting:HttpsPort"]);
            HostBuilder.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)            
            .ConfigureAppConfiguration((hostingContext, config) =>
            {                
                config.AddCommandLine(args);
            })   
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            })   
            .UseStartup(Assembly.GetEntryAssembly().FullName)
            .UseKestrel(opt => opt.AddServerHeader = false);
        
        private static string GetAppSettingFile(string[] args)
        {
            string AppSettingsFile;
            bool IsDefaultProduction = true;
            string EnvironmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if(string.IsNullOrEmpty(EnvironmentName) && args.Length > 0)  EnvironmentName = args[0].Substring(14);
            else if(string.IsNullOrEmpty(EnvironmentName) && args.Length == 0) EnvironmentName = "production";
            IsDefaultProduction = EnvironmentName.ToLower().Equals("production");
            Console.WriteLine("Environment " + EnvironmentName);
            if(IsDefaultProduction) AppSettingsFile = AppSettingsFilePrefix + AppSettingsFileExtension;
            else AppSettingsFile = AppSettingsFilePrefix + "." + EnvironmentName + AppSettingsFileExtension;
            return AppSettingsFile;
        }
    }
}