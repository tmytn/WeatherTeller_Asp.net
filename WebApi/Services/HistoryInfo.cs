using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApi.Models;

namespace WebApi.Services
{
    public static class HistoryInfo
    {
        public static string path = "";

        public static void SaveSearch(HistorySearch obj)
        {
            FileStream f;
            if (!System.IO.File.Exists(path))
            {
                f = System.IO.File.Create(path);
                System.IO.File.WriteAllText(path, "'Histories':[]");
            }
            try
            {
                if (!checkItemExisting(obj))
                {
                    var newRecord = parseToString(obj);

                    var json = File.ReadAllText(path);
                    var jsonObj = JObject.Parse(json);
                    var itemArrary = jsonObj.GetValue("Histories") as JArray;
                    var newItem = JObject.Parse(json);
                    itemArrary.Add(newItem);

                    jsonObj["Histories"] = itemArrary;
                    string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(path, newJsonResult);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
        }

        public static List<HistorySearch> GetAllRecords(string jsonString)
        {
            List<HistorySearch> records = new List<HistorySearch>();
            HistorySearch itemHis = null;
            try
            {
                var jObject = JObject.Parse(jsonString);
                JArray itemArrary = (JArray)jObject["Histories"];

                if (itemArrary != null)
                {
                    foreach (var item in itemArrary)
                    {
                        itemHis = new HistorySearch()
                        {
                            coord = new Coord()
                            {
                                lat = Double.Parse(item["latitude"].ToString()),
                                lon = Double.Parse(item["longtitude"].ToString())
                            },
                            nameLocation = item["nameLocation"].ToString()
                        };
                        records.Add(itemHis);
                    }

                }

            }catch (Exception ex)
            {
                throw ex;
            }
            return records;
        }

        public static bool checkItemExisting (HistorySearch obj)
        {
            var json = File.ReadAllText(path);
            bool isExisting = false;
            try
            {
                var jObject = JObject.Parse(json);
                JArray itemArrary = (JArray)jObject["Histories"];
                isExisting = itemArrary.Select(x => x["latitude"].Equals(obj.coord.lat.ToString()) &&
                                    x["longtitude"].Equals(obj.coord.lat.ToString())).First();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isExisting;
        }
        public static string parseToString (HistorySearch obj)
        {
            return "{ 'latitude': " + obj.coord.lat + 
                   ", 'longtitude': '" + obj.coord.lon +
                   ", 'nameLocation': '" + obj.nameLocation  + "'}";
        }


    }
}