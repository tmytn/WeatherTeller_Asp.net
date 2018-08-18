using APIXULib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IService
    {
        void getApiRequestString(string searchText, [Optional] Coord cor, int type, ref string curUrl, ref string weekInfoUrl);
        void getResponseWeather(string searchText, [Optional] Coord cor, int type, 
                                ref ResponseWeather rootObject, ref List<Models.Forecastday> weekData);
       
        string getCurrentApiRequestBasedOnCity(string city);
        string getCurrentApiRequestBasedOnCoord(Coord cor);
        string getCurrentApiRequestBasedOnZipcode(string zipcode);

        string getForecastApiRequestBasedOnCity(string city);
        string getForecastApiRequestBasedOnCoord(Coord cor);
        string getForecastApiRequestBasedOnZipcode(string zipcode);
        
    }
}
