using BaseHelpersForExamples;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GetAppItems
{
    class Program
    {
        static Configuration _configuration = null;

        static void Main(string[] args)
        {
            _configuration = ConfigurationManager.Load();

            // 1. Загрузка элементов приложения "app" раздела "chapter"
            var appItemsInfo = LoadAppItemsInfo(_configuration["chapter"], _configuration["app"]);

            if (appItemsInfo != null)
            {
                foreach (var item in appItemsInfo.Result.AppItemJObjects)
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine("-----------------");

            string firstItemId = appItemsInfo.Result.AppItemJObjects[0]["__id"].Value<string>();

            // 2. Загрузка одного элемента приложения по id.
            var singleItemInfo = LoadAppSingleItemInfo(_configuration["chapter"], _configuration["app"], firstItemId);
            Console.WriteLine(singleItemInfo.Item);
        }

        static AppItemsInfo LoadAppItemsInfo(string chapterName, string appName)
        {
            AppItemsInfo appItemsInfo = null;

            try
            {
                string responseFromServer = RequestPost($"{_configuration.UrlBase}/pub/v1/app/{chapterName}/{appName}/list");
                appItemsInfo = JsonConvert.DeserializeObject<AppItemsInfo>(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return appItemsInfo;
        }

        static AppSingleItemInfo LoadAppSingleItemInfo(string chapterName, string appName, string itemId)
        {
            AppSingleItemInfo appItemsInfo = null;

            try
            {
                string responseFromServer = RequestPost($"{_configuration.UrlBase}/pub/v1/app/{chapterName}/{appName}/{itemId}/get");
                appItemsInfo = JsonConvert.DeserializeObject<AppSingleItemInfo>(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return appItemsInfo;
        }

        static string RequestPost(string requestUri, Dictionary<string, string> dictionaryParameters)
        {
            if (dictionaryParameters != null && dictionaryParameters.Count > 0)
            {
                return RequestPost(requestUri, dictionaryParameters);
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

    class AppSingleItemInfo
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("item")]
        public JObject Item { get; set; }
    }

    class AppItemsInfo
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("result")]
        public AppItemsInfoResult Result { get; set; }
    }

    class AppItemsInfoResult
    {
        [JsonProperty("result")]
        public List<JObject> AppItemJObjects { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
