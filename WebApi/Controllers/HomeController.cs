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
using WebApi.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Caching;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        public IService myService = new Service();
        public OpenWeatherMap openWeatherMap = new OpenWeatherMap();
        public ActionResult Index()
        {
            string cookievalue = "";
            if (Request.Cookies["HistorySearches"] != null)
            {
                //already there
                cookievalue = Request.Cookies["HistorySearches"].Value.ToString();
                openWeatherMap.historySearchs = JsonConvert.DeserializeObject<List<HistorySearch>>(cookievalue);
            }
            if (openWeatherMap.historySearchs == null)
            {
                //new cookie
                Response.Cookies["HistorySearches"].Value = "";
                Response.Cookies["HistorySearches"].Expires = DateTime.Now.AddDays(10); // add expiry time
            }
            return View(openWeatherMap);
        }

        [HttpPost]
        public ActionResult Index(string curlat, string curlon, string requestString)
        {
            Coord currentLocation = null;
            int type = 0;
            // navigating search criteria
            if (!String.IsNullOrEmpty(requestString) || (!String.IsNullOrEmpty(curlat) && !String.IsNullOrEmpty(curlon)))
            {

                ResponseWeather rootObjectt = new ResponseWeather();
                List<Forecastday> weekInfo = new List<Forecastday>();

                requestString = requestString.Trim();

                if (Regex.Match(requestString, @"^\d{5}$", RegexOptions.IgnoreCase).Success)
                {//search by zip code
                    type = 2;
                    openWeatherMap.apiResponse = "";
                }
                else if (Regex.Match(requestString, @"^\w+\,\w+$", RegexOptions.IgnoreCase).Success)
                {//search by city (Lacey,us), (danang,vn)
                    type = 1;
                    openWeatherMap.apiResponse = "";
                }
                else if (String.IsNullOrEmpty(curlat) == false && String.IsNullOrEmpty(curlon) == false)
                {//current location
                    currentLocation = new Coord() { lat = Double.Parse(curlat), lon = Double.Parse(curlon) };
                    type = 3;
                }
                else
                {
                    openWeatherMap.apiResponse = "Invalid zipcode or city";
                    return View(openWeatherMap);
                }

                if (type == 0)
                {
                    openWeatherMap.apiResponse = "Nothing to render";
                    return View(openWeatherMap);
                }

                myService.getResponseWeather(searchText: requestString,
                                            cor: currentLocation,
                                            type: type,
                                            rootObject: ref rootObjectt,
                                            weekData: ref weekInfo
                                            );

                openWeatherMap.rootObject = rootObjectt;

                if (rootObjectt != null)
                {
                    var recent = new HistorySearch()
                    {
                        coord = rootObjectt.coord,
                        nameLocation = rootObjectt.name
                    };

                    retrieveInfo(recent);
                    saveToCookie(openWeatherMap.historySearchs);
                    weekInfo.ForEach(x => x.date = DateTime.Parse(x.date).Month.ToString() + "/" + DateTime.Parse(x.date).Day.ToString());
                    openWeatherMap.forecastResponse = weekInfo;
                }else
                {
                    openWeatherMap.apiResponse = "Info you are searching is not found";
                    return View(openWeatherMap);
                }
                
            }
            else if (Request.Form["submit"] != null)
            {
                openWeatherMap.apiResponse = "Put in city or zip code";
                
            }

            return View(openWeatherMap);
          
        }

        //save to cookie
        public void saveToCookie(List<HistorySearch> obj)
        {
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonString = javaScriptSerializer.Serialize(obj);

            Response.Cookies["HistorySearches"].Value = jsonString;
        }

        //load from cookie
        public void retrieveInfo (HistorySearch obj)
        {
            if (Request.Cookies["HistorySearches"] != null)
            {
                //already there
                string cookievalue = Request.Cookies["HistorySearches"].Value.ToString();
                openWeatherMap.historySearchs = JsonConvert.DeserializeObject<List<HistorySearch>>(cookievalue);
            }
            else
            {
                //create one
                if (openWeatherMap.historySearchs == null)
                {
                    openWeatherMap.historySearchs = new List<HistorySearch>();
                }
            }

            if (obj != null)
            {
                HistorySearch item = null;
                if (openWeatherMap.historySearchs != null) { 
                    item = openWeatherMap.historySearchs.Find(x => x.coord.lat == obj.coord.lat
                                                        && x.coord.lon == obj.coord.lon);
                }else
                {
                    openWeatherMap.historySearchs = new List<HistorySearch>();
                }
                if (item == null)
                {
                    openWeatherMap.historySearchs.Add(obj);
                }
                else
                {
                    openWeatherMap.historySearchs.Find(x => x.coord.lat == obj.coord.lat
                                                        && x.coord.lon == obj.coord.lon).count++;
                }
            }
        }







    }
}
