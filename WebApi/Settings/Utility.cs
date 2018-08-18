using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApi.Settings
{
    public static class Utility
    {
        public static string GetApiKey()
        {
            return  ConfigurationManager.AppSettings["OpenMapAPIKey"];
        }

        public static string GetStringRequestAPI()
        {
            return ConfigurationManager.AppSettings["APIRequest"];
        }

        public static string GetApiForecastKey()
        {
            return ConfigurationManager.AppSettings["APIXUKey"];
        }

        public static string GetStringRequestAPIXU()
        {
            return ConfigurationManager.AppSettings["APIRequestForecast"];
        }
    }
}