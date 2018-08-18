using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApi.Models;
using System.Runtime.InteropServices;
using WebApi.Settings;
using System.Device.Location;
using System.Globalization;
using Plugin.Geolocator;
using System.Xml;
using APIXULib;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApi.Services
{
    public class Service : IService
    {
        public string apiKey = Utility.GetApiKey();
        public string apiForecastKey = Utility.GetApiForecastKey();

        /***
         *Get api request string
         * searchText : zipcode or string (city,country : ex. Lacey,us or danang,vn)
         * cor: current location
         * type: type of request
         * curUrl: location request string
         * weekInfoUrl: forecast request string 
         */
        public void getApiRequestString(string searchText, [Optional] Coord cor, int type, ref string curUrl, ref string weekInfoUrl)
        {
            curUrl = ""; weekInfoUrl = "";
            if (type == 1) // city
            {
                curUrl = getCurrentApiRequestBasedOnCity(searchText);
                weekInfoUrl = getForecastApiRequestBasedOnCity(searchText);
            }
            else if (type == 2)//zip code
            {
                curUrl = getCurrentApiRequestBasedOnZipcode(searchText);
                weekInfoUrl = getForecastApiRequestBasedOnZipcode(searchText);
            }
            else if (type == 3)//cor
            {
                curUrl = getCurrentApiRequestBasedOnCoord(cor);
                weekInfoUrl = getForecastApiRequestBasedOnCoord(cor);
            }
        }

      
       /**
        * get responded info
        * searchText: search string
        * cor: Current location
        * type: type of search
        * rootObject: search response weather object
        * weekData: search response of incoming week
        * 
        */ 
        public void getResponseWeather(string searchText,
                                                  [Optional] Coord cor,
                                                  int type,
                                                  ref ResponseWeather rootObject, ref List<Models.Forecastday> weekData)
        {
            string curUrl = "", weekUrl = "";
            getApiRequestString(searchText, cor, type,ref curUrl, ref weekUrl);
            System.Net.HttpWebRequest apiRequest = WebRequest.Create(curUrl) as HttpWebRequest;

            string apiResponse = "";
            try
            {

                using (System.Net.HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    apiResponse = reader.ReadToEnd();
                }
                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse, serSettings);

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("not found"))
                {
                    rootObject = null;
                }
            }

            
            //-----get 7 days info-------------
            Models.WeatherModel model = GetData(weekUrl);
            if (model != null && model.forecast != null)
            {
                weekData = model.forecast.forecastday;
            }
        }


        /**
         * Get forecast data model from url 
         */ 
        private Models.WeatherModel GetData(string url)
        {
            string urlParameters = "";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                return response.Content.ReadAsAsync<Models.WeatherModel>().Result;

            }
            else
            {
                return new Models.WeatherModel();
            }
        }

        public string getCurrentApiRequestBasedOnCity(string city)
        {
            return String.Format(Utility.GetStringRequestAPI() + "q={0}&appid={1}&units=metric", city, apiKey);
        }

        public string getCurrentApiRequestBasedOnCoord(Coord cor)
        {
            return String.Format(Utility.GetStringRequestAPI() + "lat={0}&lon={1}&appid={2}&units=metric", cor.lat.ToString(), cor.lon.ToString(), apiKey);
        }

        public string getCurrentApiRequestBasedOnZipcode(string zipcode)
        {
            return String.Format(Utility.GetStringRequestAPI() + "zip={0},us&appid={1}&units=metric", zipcode, apiKey);
        }

        public string getForecastApiRequestBasedOnCity(string city)
        {
            return String.Format(Utility.GetStringRequestAPIXU()+ "/forecast.json?key={0}&q={1}&days=7", apiForecastKey,city);
        }

        public string getForecastApiRequestBasedOnCoord(Coord cor)
        {
            return String.Format(Utility.GetStringRequestAPIXU() + "/forecast.json?key={0}&q={1},{2}&days=7", apiForecastKey, cor.lat,cor.lon);
        }

        public string getForecastApiRequestBasedOnZipcode(string zipcode)
        {
            return String.Format(Utility.GetStringRequestAPIXU() + "/forecast.json?key={0}&q={1}&days=7", apiForecastKey, zipcode);
        }
    }
}