/// <summary>
/// Gets weather data from National Weather Service.
/// Ported to C#/ASP.NET by Jeremy Coulson.
/// Original project by Ron Pringle (https://github.com/rpringle/National-Weather-Service-Parser).
/// 
/// COPYRIGHT AND LICENSING NOTICE
/// 
/// Copyright 2011 County of Frederick, Virginia. All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without modification, are
/// permitted provided that the following conditions are met:
///
///   1. Redistributions of source code must retain the above copyright notice, this list of
///      conditions, and the following disclaimer.
///
///   2. Redistributions in binary form must reproduce the above copyright notice, this list
///      of conditions and the following disclaimer in the documentation and/or other materials
///      provided with the distribution.
///
/// THIS SOFTWARE IS PROVIDED BY County of Frederick, Virginia ''AS IS'' AND ANY EXPRESS OR IMPLIED
/// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
/// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
/// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
/// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
/// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
/// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
/// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
/// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
/// The views and conclusions contained in the software and documentation are those of the
/// authors and should not be interpreted as representing official policies, either expressed
/// or implied, of the County of Frederick, Virginia.
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
        string strRemoteFeed = "KOKV.xml"; // Replace with name of your chosen local feed's XML file.
        string strIconsPath = "/wx/images/weather/"; // Replace with path to local image directory.

        protected void Page_Load(object sender, EventArgs e)
        {
            // Call the function that makes the XML file.
            parseWeather();

            // Check to see if the local file exists.
            if (File.Exists(Server.MapPath(strLocalFeed)))
            {
                // Create an XDocument.
                XDocument xmlDoc = XDocument.Load(Server.MapPath(strLocalFeed));

                // Get the stuff I want from the XML file.
                // I also formatted the Literal output in here.  Bad idea?
                var q = from c in xmlDoc.Descendants("current_observation")
                        select "<div id=\"wxContainer\">" +
                                    "<img id=\"icon\" alt=\" \" src=\"" + strIconsPath + (string)c.Element("icon_url_name") + "\" />" +
                                    "<p class=\"boxText\" id=\"basic\">" +
                                        "<span class=\"tempSpan\">" + (string)c.Element("temp_f") + "&#176; F</span><br />" +
                                        "<span class=\"weatherSpan\">" + (string)c.Element("weather") + "</span>" +
                                    "</p>" +
                                    "<p class=\"boxText\" id=\"details\">" +
                                        "<span class=\"category\">Wind chill: <span class=\"value\">" + (string)c.Element("windchill_f") + "</span>&#176; F<br /></span>" +
                                        "<span class=\"category\">Heat index: <span class=\"value\">" + (string)c.Element("heat_index_f") + "</span>&#176; F<br /></span>" +
                                        "<span class=\"category\">Humidity: <span class=\"value\">" + (string)c.Element("relative_humidity") + "</span>%<br /></span>" +
                                        "<span class=\"category\">Wind: <span class=\"value\">" + (string)c.Element("wind_mph") + " MPH " + (string)c.Element("wind_dir") + "</span></span>" +
                                    "</p>" +
                                    "<p class=\"boxText\"><a href=\"http://forecast.weather.gov/MapClick.php?CityName=Winchester&amp;state=VA&amp;site=LWX&amp;lat=39.1745&amp;lon=-78.175\" target=\"_blank\" title=\"National Weather Service Forecast\">View Forecast</a></p>" +
                                    "<p class=\"boxText\" id=\"obstime\">" + (string)c.Element("observation_time") + "</p>" +
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

            // Display app version.
            litVersion.Text = "<p class=\"appVersion\">" + String.Format("{0} : {1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location)) + "</p>";

        }

        protected void parseWeather()
        {
            // I'll need some variables here.
            string strFileName = Server.MapPath(strLocalFeed);
            string strWeatherUrl = "http://www.nws.noaa.gov/data/current_obs/" + strRemoteFeed;

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
                    using (StreamWriter sw = new StreamWriter(Server.MapPath(strLocalFeed)))
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
                using (StreamWriter sw = new StreamWriter(Server.MapPath(strLocalFeed)))
                {
                    sw.Write(strResult);
                    sw.Close();
                }
            }
        }
    }
}
