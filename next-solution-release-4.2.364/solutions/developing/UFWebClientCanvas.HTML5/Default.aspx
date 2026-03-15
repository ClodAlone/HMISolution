<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="UFWebClientCanvas.HTML5._Default" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <script type="text/javascript">
    // <![CDATA[

        window.onload = init;

        var ctx;
        var ctxBackground;
        var canvas;
        var canvasBackground;

        function init() {

            LoadingPanel.Show();

            canvas = document.getElementById('canvas');
            canvasBackground = document.getElementById('canvasBackground');
            // ctx = canvas.getContext("2d");
            // ctxBackground = canvasBackground.getContext("2d");

            window.addEventListener("resize", OnWindowResize, false);

            LoadingPanel.Hide();
        }

        function OnWindowResize(e) {

            LoadingPanel.Show();

            var bodyWidth = window.innerWidth;
            var bodyHeight = window.innerHeight;

            if (typeof e == 'undefined')
                e = window.event;
            //redraw the background
            canvasBackground.clearRect(0, 0, ctxBackground.width, ctxBackground.height);

            LoadingPanel.Hide();
        }

    // ]]>
    </script>

    <dx:ASPxLoadingPanel ID="ASPxLoadingPanel1" runat="server" Modal="true" HorizontalAlign="Center"           
            ClientInstanceName="LoadingPanel">               
    </dx:ASPxLoadingPanel>
</asp:Content>
