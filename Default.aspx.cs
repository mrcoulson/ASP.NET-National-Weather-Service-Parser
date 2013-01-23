/// <summary>
/// Gets weather data from National Weather Service.
/// Ported to C#/ASP.NET by Jeremy Coulson.
/// Original project by Ron Pringle (https://github.com/rpringle/National-Weather-Service-Parser).
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Net;

namespace NWSParser
{
    public partial class _Default : System.Web.UI.Page
    {

        // Variables we will need throughout.
        string strLocalFeed = "/wx/KOKV.xml"; // Replace with whatever file name you want.
        string strRemoteFeed = "KOKV.xml"; // Replace with name of your chosen remote feed's XML file.
        string strLocalFeedBackup = "/wx/KIAD.xml"; // Replace with a backup file for the first local feed.
        string strRemoteFeedBackup = "KIAD.xml"; // Replace with the name of a backup remote feed.
        string strIconsPath = "/wx/images/weather/"; // Replace with path to local image directory.

        protected void Page_Load(object sender, EventArgs e)
        {
            // Parse and display weather.
            parseWeather(strLocalFeed, strRemoteFeed);
            displayWeather(strLocalFeed);

            // Check for broken weather display.
            if (!litOut.Text.Contains("png"))
            {
                // Run functions again with backup feed if main feed is broken.
                parseWeather(strLocalFeedBackup, strRemoteFeedBackup);
                displayWeather(strLocalFeedBackup);
            }

            // Display app version.
            litVersion.Text = "<p class=\"appVersion\">" + String.Format("{0} : {1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location)) + "</p>";
        }

        #region weather functions

        void parseWeather(string strLocalFeedArg, string strRemoteFeedArg)
        {
            // I'll need some variables here.
            string strFileName = Server.MapPath(strLocalFeedArg);
            string strWeatherUrl = "http://www.nws.noaa.gov/data/current_obs/" + strRemoteFeedArg;

            // Check to see if the local file exists.
            // If it does...
            if (File.Exists(strFileName))
            {
                // Write weather depending on time.
                // Get difference in second between now and last modified date.
                DateTime dtNow = DateTime.Now;
                DateTime dtLastWrite = File.GetLastWriteTime(strFileName);
                TimeSpan tspDifference = dtNow.Subtract(dtLastWrite);
                litOut.Text = tspDifference.TotalMinutes.ToString();
                if (tspDifference.TotalMinutes > 60)
                {
                    // Write the file.
                    // Read remote NWS XML file.
                    string strResult;
                    WebResponse objResponse;
                    WebRequest objRequest = HttpWebRequest.Create(strWeatherUrl);
                    objResponse = objRequest.GetResponse();
                    using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                    {
                        strResult = sr.ReadToEnd();
                        sr.Close();
                    }
                    // Write remote NWS XML to local XML.
                    using (StreamWriter sw = new StreamWriter(Server.MapPath(strLocalFeedArg)))
                    {
                        sw.Write(strResult);
                        sw.Close();
                    }
                }
            }
            else
            {
                // Write the file.
                // Since the file doesn't exist locally, I don't need to get the time differences.
                // Read remote NWS XML file.
                string strResult;
                WebResponse objResponse;
                WebRequest objRequest = HttpWebRequest.Create(strWeatherUrl);
                objResponse = objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    sr.Close();
                }
                // Write remote NWS XML to local XML.
                using (StreamWriter sw = new StreamWriter(Server.MapPath(strLocalFeedArg)))
                {
                    sw.Write(strResult);
                    sw.Close();
                }
            }
        }

        void displayWeather(string strLocalFeedArg)
        {
            // Check to see if the local file exists.
            if (File.Exists(Server.MapPath(strLocalFeedArg)))
            {
                // Create an XDocument.
                XDocument xmlDoc = XDocument.Load(Server.MapPath(strLocalFeedArg));

                // Get the stuff I want from the XML file.
                // I also formatted the Literal output in here.  Bad idea?
                var q = from c in xmlDoc.Descendants("current_observation")
                        select "<div id=\"wxContainer\">" +
                                    "<img id=\"icon\" alt=\" \" src=\"" + strIconsPath + (string)c.Element("icon_url_name") + "\" />" +
                                    "<p class=\"boxText\" id=\"basic\">" +
                                        "<span class=\"tempSpan\">" + (float)c.Element("temp_f") + "&#176; F</span><br />" +
                                        "<span class=\"weatherSpan\">" + (string)c.Element("weather") + "</span>" +
                                    "</p>" +
                                    "<p class=\"boxText\" id=\"details\">" +
                                        "<span class=\"category\">Wind chill: <span class=\"value\">" + (string)c.Element("windchill_f") + "</span>&#176; F<br /></span>" +
                                        "<span class=\"category\">Heat index: <span class=\"value\">" + (string)c.Element("heat_index_f") + "</span>&#176; F<br /></span>" +
                                        "<span class=\"category\">Humidity: <span class=\"value\">" + (string)c.Element("relative_humidity") + "</span>%<br /></span>" +
                                        "<span class=\"category\">Wind: <span class=\"value\">" + (string)c.Element("wind_mph") + " MPH " + (string)c.Element("wind_dir") + "</span></span>" +
                                    "</p>" +
                                    "<p class=\"boxText\"><a href=\"http://forecast.weather.gov/MapClick.php?CityName=Winchester&amp;state=VA&amp;site=LWX&amp;lat=39.1745&amp;lon=-78.175\" target=\"_blank\" title=\"National Weather Service Forecast\">View Forecast</a></p>" +
                                    "<p class=\"boxText\" id=\"obstime\">Last updated " + (string)c.Element("observation_time_rfc822") + ".<br />Observation from " + (string)c.Element("location") + ".</p>" +
                                "</div>";

                // I'll need a string for the Literal.
                string strOut;

                foreach (string str in q)
                {
                    // Loop through the strings in q (there's only 1) and add it to the Literal. 
                    strOut = String.Format("{0}", str);
                    litOut.Text = str;
                }
            }
            else
            {
                // Notify that no weather file is found.
                Response.Write("No weather file found.");
            }
        }

        #endregion
    }
}
