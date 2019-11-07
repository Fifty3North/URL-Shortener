using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace nixfox.App{
	public class Config
	{
		public string BASE_URL;
        private string _db_location;
        private string _baseDir;

        public string DB_LOCATION 
        { 
            get 
            {
                return Path.GetFullPath(_baseDir + Path.DirectorySeparatorChar + _db_location);
            } 
            set 
            { 
                _db_location = value; 
            } 
        }

        public void SetBaseDir(string baseDir)
        {
            _baseDir = baseDir;
        }
    }
	public class NixConf
    {
        public NixConf(IWebHostEnvironment env)
        {
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText("App/Config.json"));
            Config.SetBaseDir(env.ContentRootPath);
        }

		public Config Config;

	}
}
