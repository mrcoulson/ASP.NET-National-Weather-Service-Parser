<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NWSParser._Default" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8">
    <title>weather parser</title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
    <script type="text/javascript">
        //<![CDATA[
        $(document).ready(function() {
            $("span.category").filter(function() {
                return $.trim($(this).find("span.value").text()).length == 0;
            }).hide();
            $("span.value:contains('0.0 MPH')").html("Calm");
            var weatherToLower = $("span.weatherSpan").html();
            weatherToLower = weatherToLower.toLowerCase();
            weatherToLower = weatherToLower.charAt(0).toUpperCase() + weatherToLower.substr(1);
            $("span.weatherSpan").html(weatherToLower);
        });
        //]]>
</script>
<style type="text/css">
	@import url("/default/css/home_style_2.css");
	body {background: none; padding: 0; margin: 0;}
	#wxContainer {width: 360px;}
	#icon {float: left; margin: 0 15px 0 0;}
	#basic {width: 95px; float: left; margin-top: 0;}
	#details {margin-top: 0;}
	#ln-messageBox p {margin-top: 30px;}
	#obstime {font-size: 9px; font-style: italic;}
	p.appVersion {font-size: .7em; font-style: italic;}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="litOut" runat="server" />
    </div>
    <asp:Literal ID="litVersion" runat="server" />
    </form>
</body>
</html>
