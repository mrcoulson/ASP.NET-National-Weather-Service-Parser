ASP.NET National Weather Service Parser
=======================================

Information
-----------

This is a port [National Weather Service Parser](https://github.com/rpringle/National-Weather-Service-Parser) originally by rpringle.

Included here are the C# code and an example ASPX page. The example page includes scripting and styles for my work environment. The Literal called `litOut` is all you really need.  [Example output](http://www.co.frederick.va.us/apps/nwsparser/default.aspx).

This parser is for anyone who wants to display local weather data without having to use a provider's branding, styles, or ads.

Requirements
------------
* ASP.NET 3.5+.

Latest Changes
--------------

NWS seems to be using PNG graphics instead of JPEG.  I changed the line that tests for the existence of the weather graphic to use the PNG.  If you are using this in your site, you'll need to save all of your JPEG images as PNG and make this change to your code.

Setup
-----

### 1. Download weather icons.

Go to [http://www.weather.gov/xml/current_obs/weather.php](http://www.weather.gov/xml/current_obs/weather.php). As of the day I'm writing this, the "Weather Icons Zip file" link is broken, so you'll need to download them one at a time (like I did).  Put them on your web server where your application can get to them.

### 2. Select two observation locations.

Search at [http://www.weather.gov/xml/current_obs/](http://www.weather.gov/xml/current_obs/).  Pick a primary and secondary (backup) in case the primary is broken.

### 3. Set up a local feed directory.

This is where the application will write your XML file that gets parsed.

### 4. Define variables.

In Default.aspx.cs, there are five strings you'll need to set for your installation:

* `strLocalFeed`: XML file to be written and read for primary location.
* `strRemoteFeed`: Primary remote feed.
* `strLocalFeedBackup`: Secondary location local XML.
* `sreRemoteFeedBackup`: Secondary location remote feed. 
* `strIconsPath`: This is where you stored your icons.

### 5. Add a Literal control to the ASPX page.

The Literal is where the output goes.

### 6. Compile.

There you go.

Notes
-----

The XML file that is written from NWS will have a lot more stuff in it than what I'm using. Explore it because you may find you want to display other things in your final page.

As it is, the query is also where the results are formatted. The result of that is stuck into a Literal. I'm sure there are other cool ideas for getting the data to display on the page.

On line 66 of Default.aspx.cs, I'm checking the age of my local feed.  Since I don't want to overburdern the NWS server with requests from my site, I make sure I'm only updating the data hourly. I don't think there's a requirement for this and I'm sure you can set it to a shorter interval, but some sort of delay in asking for new data seems pretty polite to me. They are, after all, offering the data for free.

If this code is of any use to you, please let me know. Also, let me know if you have any questions. The community has given much to me over the last few years, so I hope I can give a little back.
