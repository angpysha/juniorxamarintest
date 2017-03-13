using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;


namespace TestProjLib
{

    public static class HelpFuncs
    {
        public enum TaskType
        {
            Country,
            City,
            University
        }
        public delegate void TaskCompeted(dynamic objects, TaskType tastType);

        public static event TaskCompeted TaskComletedEventHandler;
        public static bool IsInternetConnectionAvaible()
        {
            if (CrossConnectivity.Current.IsConnected == true &&
                CrossConnectivity.Current != null &&
                CrossConnectivity.Current.ConnectionTypes != null)
                return true;
            else
            {
                return false;
            }
        }

        private static void callEvent(dynamic obj, TaskType taskType)
        {
            TaskComletedEventHandler?.Invoke(obj, taskType);
        }
        public static async Task GetCounties()
        {

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            dynamic res;
            request.RequestUri =
                new Uri(@"http://api.vk.com/method/database.getCountries?v=5.5&need_all=1&count=1000");
            request.Method = HttpMethod.Get;
            request.Headers.Add("header", @"Accept-language: ru\r\n" + @"Cookie: remixlang=0\r\n");
            List<Country> countries = new List<Country>();
            HttpResponseMessage responce = await client.SendAsync(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                HttpContent content = responce.Content;
                var content_json = await content.ReadAsStringAsync();
                //JavaScriptSerializer
                JObject o = JObject.Parse(content_json);
                JToken countriesList = o["response"]["items"];

                foreach (var country in countriesList)
                {
                    countries.Add(new Country
                    {
                        id = Int32.Parse(country["id"].ToString()),
                        caption = country["title"].ToString()
                    });
                }

                res = (dynamic)countries;
                // res = responce.StatusCode.ToString();
            }
            else
            {
                res = (dynamic)countries;
            }
            callEvent(res, TaskType.Country);
            //return await res;
            // TaskComletedEventHandler(res,TaskType.Country);
        }

        public static async Task GetCities(string query, int countryId)
        {
            //http://api.vk.com/method/database.getRegions?v=5.5&need_all=1&offset=0&count=1000&country_id=
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            List<City> cities = new List<City>();
            dynamic res;
            request.RequestUri =
                new Uri($@"http://api.vk.com/method/database.getCities?v=5.5&need_all=1&offset=0&count=1000&country_id={countryId}&q={query}");
            request.Method = HttpMethod.Get;
            request.Headers.Add("header", @"Accept-language: ru\r\n" + @"Cookie: remixlang=0\r\n");
            HttpResponseMessage responce = await client.SendAsync(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                HttpContent content = responce.Content;
                var content_json = await content.ReadAsStringAsync();
                //int i = 0;
                JObject o = JObject.Parse(content_json);
                JToken cityList = o["response"]["items"];
                foreach (var city in cityList)
                {
                    cities.Add(new City
                    {
                        Id = Int32.Parse(city["id"].ToString()),
                        Title = city["title"].ToString()
                    });
                }

                res = (dynamic)cities;
            }
            else
                res = (dynamic)cities;
            callEvent(cities, TaskType.City);
        }

        public static async Task GetUniversities(string query,int countryId,int cityId)
        {
            //http://api.vk.com/method/database.getUniversities?v=5.5&country_id=2&offset=0&need_all=1&count=1000&q=К
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            List<University> universities = new List<University>();
            dynamic res;
            request.RequestUri =
                new Uri($@"http://api.vk.com/method/database.getUniversities?v=5.5&country_id={countryId}&offset=0&need_all=1&count=1000&q={query}?&city_id={cityId}");
            request.Method = HttpMethod.Get;
            request.Headers.Add("header", @"Accept-language: ru\r\n" + @"Cookie: remixlang=0\r\n");
            HttpResponseMessage responce = await client.SendAsync(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                HttpContent content = responce.Content;
                var json_content = await content.ReadAsStringAsync();

                JObject o = JObject.Parse(json_content);
                JToken universities_list = o["response"]["items"];
                foreach (var univesity in universities_list)
                {
                    universities.Add(new University
                    {
                        Id = Int32.Parse(univesity["id"].ToString()),
                        Title = univesity["title"].ToString()
                    });
                }

                res = universities;
                callEvent(universities,TaskType.University);
            }
            else
            {
                
            }
        }
    }
}
