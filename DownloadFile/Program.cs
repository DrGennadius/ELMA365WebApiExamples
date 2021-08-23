using BaseHelpersForExamples;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace DownloadFile
{
    class Program
    {
        static Configuration _configuration = null;

        static void Main(string[] args)
        {
            _configuration = ConfigurationManager.Load();

            DowloadFileFromServer((string)_configuration["fileId"], (string)_configuration["filePathForSave"]);
        }

        static void DowloadFileFromServer(string fileId, string filePathForSave)
        {
            WebRequest request = WebRequest.Create($"{_configuration.UrlBase}/pub/v1/disk/file/{fileId}/get-link");
            request.Method = "POST";
            request.Headers.Add("X-Token", _configuration.XToken);
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();

            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            string link = "";

            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);

                    var data = (JObject)JsonConvert.DeserializeObject(responseFromServer);
                    link = data["Link"].Value<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (string.IsNullOrEmpty(link))
            {
                return;
            }
            Console.WriteLine($"link: {link}");

            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(link, filePathForSave);
                bool fileExists = File.Exists(filePathForSave);
                Console.WriteLine($"File is downloaded: {fileExists}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
