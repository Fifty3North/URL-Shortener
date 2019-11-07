using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using nixfox.App;
using Volt.Http;

namespace nixfox.Controllers
{
    public class ShortenRequest
    {
        public string Code { get; set; }
        public string Url { get; set; }

    }

	public class URLResponse
	{
		public string url { get; set; }
		public string status { get; set; }
		public string token { get; set; }
	}
	public class HomeController : Controller
	{
        private Config _config;

        public HomeController(NixConf conf)
        {
            _config = conf.Config;
        }

		[HttpGet, Route("/")]
		public IActionResult Index() {
			return View();
		}

		[HttpPost, Route("/")]
		public IActionResult PostURL([FromBody] ShortenRequest req) 
        {
            string url = req.Url;
            string code = req.Code;

            using(var db = new LiteDB.LiteDatabase(new LiteDB.ConnectionString($"Filename={_config.DB_LOCATION}"))) {
                try
                {
                    if (!url.Contains("http"))
                    {
                        url = "http://" + url;
                    }
                    if (db.GetCollection<NixURL>().Exists(u => u.ShortenedURL == url))
                    {
                        Response.StatusCode = 405;
                        return Json(new URLResponse() { url = url, status = "already shortened", token = null });
                    }
                    
                    Shortener shortURL = new Shortener(_config, url, code);
                    return Json(shortURL.Token);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "URL already exists")// || )
                    {
                        Response.StatusCode = 400;
                        return Json(new URLResponse() { url = url, status = ex.Message, token = db.GetCollection<NixURL>().Find(u => u.Token == code).FirstOrDefault().Token });
                    } 
                    else if(ex.Message == "Code already used")
                    {
                        Response.StatusCode = 400;
                        return Json(new URLResponse() { url = db.GetCollection<NixURL>().Find(u => u.Token == code).FirstOrDefault().URL, status = ex.Message, token = code });
                    }
                    throw new Exception(ex.Message);
                }
            }
		}

		[HttpGet, Route("/{token}")]
		public IActionResult NixRedirect([FromRoute] string token) {
			return Redirect(new LiteDB.LiteDatabase(new LiteDB.ConnectionString($"Filename={_config.DB_LOCATION}")).GetCollection<NixURL>().FindOne(u => u.Token == token).URL);
		}
		
		private string FindRedirect(string url){
			string result = string.Empty;
			using (var client = new HttpClient())
			{
				var response = client.GetAsync(url).Result;
				if (response.IsSuccessStatusCode)
				{
					result = response.Headers.Location.ToString();
				}
			}
			return result;
		}

	}
}
