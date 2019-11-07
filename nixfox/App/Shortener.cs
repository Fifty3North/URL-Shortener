using LiteDB;
using System;
using System.Linq;

namespace nixfox.App{
	public class NixURL{
		public Guid ID { get; set; }
		public string URL { get; set; }
		public string ShortenedURL { get; set; }
		public string Token { get; set; }
		public int Clicked { get; set; } = 0;
		public DateTime Created { get; set; } = DateTime.Now;
	}

	public class Shortener{
       
        public string Token { get; set; } 
		private NixURL biturl;

        private string GenerateToken() {
			string urlsafe = string.Empty;
			Enumerable.Range(48, 75).Where(i => i < 58 || i > 64 && i < 91 || i > 96).OrderBy(o => new Random().Next()).ToList().ForEach(i => urlsafe += Convert.ToChar(i));
            return urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(2, 6));
		}

		public Shortener(Config config, string url, string code) 
        {
			var db = new LiteDatabase(new ConnectionString($"Filename={config.DB_LOCATION}"));
			
            var urls = db.GetCollection<NixURL>();

            if (urls.Exists(u => u.URL == url))
                throw new Exception("URL already exists");

            if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
            {
                Token = GenerateToken();

                while (urls.Exists(u => u.Token == Token))
                {
                    Token = GenerateToken();
                }
            }
            else
            {
                if(urls.Exists(u => u.Token == code))
                {
                    throw new Exception("Code already used");
                }
                
                Token = code;
            }

			biturl = new NixURL() { Token = Token, URL = url, ShortenedURL = config.BASE_URL + Token };

			urls.Insert(biturl);
		}
	}
}
