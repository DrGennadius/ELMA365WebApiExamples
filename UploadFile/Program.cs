using BaseHelpersForExamples;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace UploadFile
{
    class Program
    {
        static Configuration _configuration = null;

        static void Main(string[] args)
        {
            _configuration = ConfigurationManager.Load();

            UploadFileToServer((string)_configuration["folderId"], (string)_configuration["filePath"]);
        }

        static void UploadFileToServer(string folderId, string filePath)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(data, 0, data.Length), "file", "example.txt");
                string responseFromServer = RequestPost($"{_configuration.UrlBase}/pub/v1/disk/directory/{folderId}/upload", form);
                Console.WriteLine(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string RequestPost(string requestUri, Dictionary<string, string> dictionaryParameters)
        {
            if (dictionaryParameters != null && dictionaryParameters.Count > 0)
            {
                return RequestPost(requestUri, new FormUrlEncodedContent(dictionaryParameters));
            }
            return RequestPost(requestUri);
        }

        static string RequestPost(string requestUri, string parametersJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(parametersJson))
            {
                parametersJson = "{}";
            }
            return RequestPost(requestUri, new StringContent(parametersJson));
        }

        static string RequestPost(string requestUri, HttpContent httpContent)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Token", _configuration.XToken);
                var response = httpClient.PostAsync(requestUri, httpContent).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
