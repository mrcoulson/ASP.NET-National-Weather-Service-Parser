ASP.NET National Weather Service Parser
=======================================

Information
-----------

This is a port of a project originally by rpringle:
https://github.com/rpringle/National-Weather-Service-Parser

Included here are the C# code and an example ASPX page. The example page includes scripting and styles for my work environment. The Literal called "litOut" is all you really need.

This parser is for anyone who wants to display local weather data without having to use a provider's branding, styles, or ads. This code targets .NET 3.5.

Setup
-----

1. Download weather icons.

Go to http://www.weather.gov/xml/current_obs/weather.php. As of the day I'm writing this, the "Weather Icons Zip file" link is broken, so you'll need to download them one at a time (like I did).  Put them on your web server where your application can get to them.

2. Set up a local feed directory.

This is where the application will write your XML file that gets parsed.

3. Define variables.

In Default.aspx.cs, there are three strings you'll need to set for your installation:
- strLocalFeed: /directoryfromstep2/the name of the XML file to be parsed.
- strRemoteFeed: The remote feed you want to read from NWS. Search at http://www.weather.gov/xml/current_obs/.
- strIconsPath: This is where you stored your icons.

4. Add a Literal control to the ASPX page.

The Literal is where the output goes.

5. Compile.

There you go.

Notes
-----

The XML file that is written from NWS will have a lot more stuff in it than what I'm using. Explore it because you may find you want to display other things in your final page.

As it is, the query is also where the results are formatted. The result of that is stuck into a Literal. I'm sure there are other cool ideas for getting the data to display on the page.

On line 123 of Default.aspx.cs, I'm checking the age of my local feed.  Since I don't want to overburdern the NWS server with requests from my site, I make sure I'm only updating the data hourly. I don't think there's a requirement for this and I'm sure you can set it to a shorter interval, but some sort of delay in asking for new data seems pretty polite to me. They are, after all, offering the data for free.

If this code is of any use to you, please let me know. Also, let me know if you have any questions. The community has given much to me over the last few years, so I hope I can give a little back.