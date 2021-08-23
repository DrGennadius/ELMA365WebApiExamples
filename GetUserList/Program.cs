using BaseHelpersForExamples;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GetUserList
{
    class Program
    {
        static Configuration _configuration = null;

        static void Main(string[] args)
        {
            _configuration = ConfigurationManager.Load();

            // 1. Загрузка данных пользователя
            var userListInfo = LoadUserList(_configuration.Parameters);

            if (!string.IsNullOrEmpty(userListInfo.Error))
            {
                Console.WriteLine(userListInfo.Error);
            }
            else
            {
                foreach (var item in userListInfo.Result.UserItemJObjects)
                {
                    Console.WriteLine(item);
                }
            }
        }

        static UserListInfo LoadUserList(Dictionary<string, object> parameters)
        {
            UserListInfo appItemsInfo = null;

            try
            {
                string responseFromServer = RequestPost($"{_configuration.UrlBase}/pub/v1/user/list", parameters);
                appItemsInfo = JsonConvert.DeserializeObject<UserListInfo>(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return appItemsInfo;
        }

        static string RequestPost(string requestUri, Dictionary<string, object> dictionaryParameters)
        {
            if (dictionaryParameters != null && dictionaryParameters.Count > 0)
            {
                return RequestPost(requestUri, JsonConvert.SerializeObject(dictionaryParameters));
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

    class UserListInfo
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("result")]
        public UserListInfoResult Result { get; set; }
    }

    class UserListInfoResult
    {
        [JsonProperty("result")]
        public List<JObject> UserItemJObjects { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
