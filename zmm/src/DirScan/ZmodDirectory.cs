using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using JgenCy.OperatingSystemCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;

namespace ZMM.DS
{
    public class ZmodDirectory
    {        
        public string Root { 
            get 
            {
                return root;
            }
            set 
            {
                root = value;
            }
        } 
        private string root = new RaiFile($"~/DropboxZMOD/ZMOD/").Path;
        public string CodeDir
        {
            get
            {
                return $"{Root}/Code/";
            }
        }
        public string DataDir
        {
            get
            {
                return $"{Root}/Data/";
            }
        }
        public string ModelDir
        {
            get
            {
                return $"{Root}/Models/";
            }
        }
        public Dictionary<string, PyFileInfo> PyFiles { get; set; } =
            new Dictionary<string, PyFileInfo>();
        public Dictionary<string, IpynbFileInfo> IpynbFiles { get; set; } =
            new Dictionary<string, IpynbFileInfo>();
        public Dictionary<string, CsvFileInfo> CsvFiles { get; set; } =
            new Dictionary<string, CsvFileInfo>();
        public Dictionary<string, JsonFileInfo> JsonFiles { get; set; } =
            new Dictionary<string, JsonFileInfo>();
        public Dictionary<string, VideoFileInfo> VideoFiles { get; set; } =
            new Dictionary<string, VideoFileInfo>();
        public Dictionary<string, ImageDirInfo> ImageFiles { get; set; } =
            new Dictionary<string, ImageDirInfo>();
        public Dictionary<string, PmmlFileInfo> PmmlFiles { get; set; } =
            new Dictionary<string, PmmlFileInfo>();        
        public Dictionary<string, H5FileInfo> H5Files { get; set; } =
            new Dictionary<string, H5FileInfo>();

        public Dictionary<string, ONNXFileInfo> OnnxFiles { get; set; } =
            new Dictionary<string, ONNXFileInfo>();
        public Dictionary<string, TextFileInfo> TextFiles { get; set; } =
            new Dictionary<string, TextFileInfo>();
        public Dictionary<string, RFileInfo> RFiles { get; set; } =
            new Dictionary<string, RFileInfo>();
        public void Refresh()
        {
            IpynbFiles = (from _ in Directory.GetFiles(CodeDir, "*.ipynb", SearchOption.AllDirectories)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new IpynbFileInfo(x));
            PyFiles = (from _ in Directory.GetFiles(CodeDir, "*.py", SearchOption.AllDirectories)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new PyFileInfo(x));
            CsvFiles = (from _ in Directory.GetFiles(DataDir, "*.csv", SearchOption.TopDirectoryOnly)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new CsvFileInfo(x));
            JsonFiles = (from _ in Directory.GetFiles(DataDir, "*.json", SearchOption.TopDirectoryOnly)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new JsonFileInfo(x));
            VideoFiles = (from _ in Directory.GetFiles(DataDir, "*.mp4", SearchOption.TopDirectoryOnly)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new VideoFileInfo(x));
            ImageFiles = (from png in Directory.GetFiles(DataDir, "*.png", SearchOption.TopDirectoryOnly)
                    select png)
                    .Concat(from jpg in Directory.GetFiles(DataDir, "*.jpg", SearchOption.TopDirectoryOnly)
                    select jpg)
                    .ToDictionary(x => new RaiFile(x).NameWithExtension, x => new ImageDirInfo(x));
            PmmlFiles = (from _ in Directory.GetFiles(ModelDir, "*.pmml", SearchOption.AllDirectories)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new PmmlFileInfo(x));
            H5Files = (from _ in Directory.GetFiles(ModelDir, "*.h5", SearchOption.AllDirectories)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new H5FileInfo(x));
            OnnxFiles = (from _ in Directory.GetFiles(ModelDir, "*.onnx", SearchOption.AllDirectories)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new ONNXFileInfo(x));
            TextFiles = (from _ in Directory.GetFiles(DataDir, "*.txt", SearchOption.TopDirectoryOnly)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new TextFileInfo(x));
            RFiles = (from _ in Directory.GetFiles(CodeDir, "*.r", SearchOption.TopDirectoryOnly)
                    select _).ToDictionary(x => new RaiFile(x).NameWithExtension, x => new RFileInfo(x));
            
        }
        public ZmodDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                Root = path;
            Refresh();
        }
    }
}