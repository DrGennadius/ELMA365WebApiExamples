using BaseHelpersForExamples;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace UploadDownloadFile
{
    class Program
    {
        static Configuration _configuration = null;

        static void Main(string[] args)
        {
            try
            {
                _configuration = ConfigurationManager.Load();

                string fileId = UploadFileToServer((string)_configuration["folderId"], (string)_configuration["filePath"]);
                DowloadFileFromServer(fileId, (string)_configuration["filePathForSave"]);
                // Далее нужно удалить файл на сервере или использовать изначально другое наименование.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка файла на сервер
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="filePath"></param>
        /// <returns>Id</returns>
        static string UploadFileToServer(string folderId, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(data, 0, data.Length), "file", "example.txt");
            string responseFromServer = RequestPost($"{_configuration.UrlBase}/pub/v1/disk/directory/{folderId}/upload", form);
            Console.WriteLine(responseFromServer);
            if (string.IsNullOrEmpty(responseFromServer))
            {
                throw new Exception("responseFromServer is empty.");
            }
            var responseData = (JObject)JsonConvert.DeserializeObject(responseFromServer);
            bool isSuccess = responseData["success"].Value<bool>();
            Console.WriteLine($"success: {isSuccess}");
            if (!isSuccess)
            {
                throw new Exception("success is FALSE.");
            }
            string fileId = responseData["file"]["__id"].Value<string>();
            if (string.IsNullOrEmpty(fileId))
            {
                throw new Exception("fileId is empty.");
            }
            return fileId;
        }

        /// <summary>
        /// Загрузка файла с сервера
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="filePathForSave"></param>
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

        /// <summary>
        /// Запрос POST
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="parametersJson"></param>
        /// <returns></returns>
        static string RequestPost(string requestUri, string parametersJson = "{}")
        {
            if (string.IsNullOrWhiteSpace(parametersJson))
            {
                parametersJson = "{}";
            }
            return RequestPost(requestUri, new StringContent(parametersJson));
        }

        /// <summary>
        /// Запрос POST
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="httpContent"></param>
        /// <returns></returns>
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
