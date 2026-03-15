<%@ Page Title="Screens" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeBehind="Screen.aspx.cs" Inherits="UFWebClient.HTML5._Default" %>

<%@ Register Assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>







<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">

<% if (HttpContext.Current.IsDebuggingEnabled) { %>
    <script type="text/javascript" src="Scripts/jquery-3.1.1.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-3.0.0.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.js"></script>
<% } else { %>
    <script type="text/javascript" src="Scripts/jquery-3.1.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-migrate-3.0.0.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.2.1.min.js"></script>
<% } %>
    <script type="text/javascript" src="SignalR/Hubs"></script>

    <script type="text/javascript" language="javascript" src="Scripts/stringformat.js">
    </script>
    <script type="text/javascript" language="javascript" src="Scripts/JScriptCanvasManager.js">
    </script>
    <script type="text/javascript">
        function SelectAndClosePopup() {
            popupScreen.Hide();
        }

        var timerCountDown;
        var countDown;
        function StartCountDown() {
            countDown = 5;
            buttonDemoMode.SetText(countDown);
            buttonDemoMode.SetEnabled(false);
            timerCountDown = setInterval(function() {
                if (--countDown > 0) {
                    buttonDemoMode.SetText(countDown);
                }
                else {
                    clearInterval(timerCountDown);
                    buttonDemoMode.SetText("OK");
                    buttonDemoMode.SetEnabled(true);
                }
            }, 1000);
        }
    </script>
            
    <div id="canvasContainer" style="position: relative;left:0px;top:0px;width:100%;height:100%;">
        <canvas id="canvasBackground" 
                style="position: absolute; left: 0; top: 0; z-index: 0;">
        </canvas>
        <canvas id="canvasMain"
                style="position: absolute; left: 0; top: 0; z-index: 1;">
                (Your browser doesn't support canvas : this is mandatory for this application)
        </canvas>
    </div>

    <dx:ASPxPanel ID="LeftPanel" runat="server" FixedPosition="WindowLeft" Collapsible="true" 
        ScrollBars="Auto" Width="25%">
        <SettingsAdaptivity CollapseAtWindowInnerWidth="900" />
        <PanelCollection>
            <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxTreeView ID="ASPxTreeView" runat="server">
                <ClientSideEvents NodeClick="function(s, e) 
                                            { 
                                                OpenScreenUrl(e.node.name, true);
                                            }" />

                </dx:ASPxTreeView>
<%--                <dx:ASPxNavBar ID="nbMain" runat="server">
                <ClientSideEvents ItemClick="function(s, e) 
                                            { 
                                                OpenScreenUrl(e.item.name, true);
                                            }" />
                <Paddings Padding="0px" />
                </dx:ASPxNavBar>--%>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxPanel>

    <dx:ASPxLoadingPanel ID="ASPxLoadingPanel1" runat="server" Modal="true" HorizontalAlign="Center"        
            ClientInstanceName="LoadingPanel" Text="Please, wait...">               
    </dx:ASPxLoadingPanel>

    <!-- Popup control for numeric value -->
    <dx:ASPxPopupControl ID="ASPxPopupControl1" runat="server" EnableTheming="True" Theme="BlackGlass" 
        CloseAction="CloseButton" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        ClientInstanceName="editValue" EnableViewState="False"
        HeaderText="Edit Value" AllowDragging="True" EnableAnimation="True">
        <ClientSideEvents PopUp="function(s, e) { editSpin.Focus(); }" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxPanel ID="ASPxPanelPopup1" runat="server" DefaultButton="ASPxButton1">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxSpinEdit ID="ASPxSpinEdit1" ClientInstanceName="editSpin" runat="server" EnableViewState="False" Height="21px" Number="0" Theme="BlackGlass">
                                <ClientSideEvents ValueChanged="OnValueChanged" />
                            </dx:ASPxSpinEdit>
                            <dx:ASPxTrackBar ID="ASPxTrackBar1" ClientInstanceName="sliderValue" runat="server" EnableViewState="False" Position="0" PositionStart="0" Step="1" Theme="BlackGlass" ScaleLabelHighlightMode="AlongBarHighlight">
                                <ClientSideEvents PositionChanged="OnPositionChanged" />
                            </dx:ASPxTrackBar>
                            <dx:ASPxButton ID="ASPxButton1" runat="server" Text="OK" AutoPostBack="False" EnableViewState="False" Theme="BlackGlass">
                                <ClientSideEvents Click="function(s, e) { ValidateNumericValue(); }" />
                            </dx:ASPxButton>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Popup control for string value -->
    <dx:ASPxPopupControl ID="ASPxPopupControl2" runat="server" EnableTheming="True" Theme="BlackGlass" 
        CloseAction="CloseButton" Modal="True" EnableViewState="False"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        ClientInstanceName="editStringValue"
        HeaderText="Edit Value" AllowDragging="True" EnableAnimation="True">
        <ClientSideEvents PopUp="function(s, e) { editString.Focus(); }" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxPanel ID="ASPxPanelPopup2" runat="server" DefaultButton="ASPxButton2">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxTextBox ID="ASPxTextBox1" ClientInstanceName="editString" runat="server" EnableViewState="False" Height="21px" Number="0" Theme="BlackGlass">
                            </dx:ASPxTextBox>
                            <dx:ASPxButton ID="ASPxButton2" runat="server" Text="OK" AutoPostBack="False" EnableViewState="False" Theme="BlackGlass">
                                <ClientSideEvents Click="function(s, e) { ValidateStringValue(); }" />
                            </dx:ASPxButton>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Popup control for boolean value -->
    <dx:ASPxPopupControl ID="ASPxPopupControl3" runat="server" EnableTheming="True" Theme="BlackGlass" 
        CloseAction="CloseButton" Modal="True" EnableViewState="False"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        ClientInstanceName="editBoolValue"
        HeaderText="Edit Value" AllowDragging="True" EnableAnimation="True">
        <ClientSideEvents PopUp="function(s, e) { editBool.Focus(); }" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl3" runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxPanel ID="ASPxPanelPopup3" runat="server" DefaultButton="ASPxButton3">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxCheckBox ID="ASPxCheckBox2" ClientInstanceName="editBool" Text="Current Value" runat="server" EnableViewState="False" Height="21px" Number="0" Theme="BlackGlass">
                            </dx:ASPxCheckBox>
                            <dx:ASPxButton ID="ASPxButton3" runat="server" Text="OK" AutoPostBack="False" EnableViewState="False" Theme="BlackGlass">
                                <ClientSideEvents Click="function(s, e) { ValidateBoolValue(); }" />
                            </dx:ASPxButton>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Popup control for List value -->
    <dx:ASPxPopupControl ID="ASPxPopupControl5" runat="server" EnableTheming="True" Theme="BlackGlass" 
        CloseAction="CloseButton" Modal="True" EnableViewState="False"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        ClientInstanceName="editListValue"
        HeaderText="Select Value" AllowDragging="True" EnableAnimation="True">
        <ClientSideEvents PopUp="function(s, e) { editList.Focus(); }" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl4" runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" DefaultButton="ASPxButton4">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxListBox ID="ASPxListBox1" runat="server" ClientInstanceName="editList" Width="100%" >
                            </dx:ASPxListBox>
                            <dx:ASPxButton ID="ASPxButton4" runat="server" Text="OK" AutoPostBack="False" EnableViewState="False" Theme="BlackGlass">
                                <ClientSideEvents Click="function(s, e) { ValidateSelectionValue(); }" />
                            </dx:ASPxButton>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Popup control for popup -->
    <dx:ASPxPopupControl ID="ASPxPopupControl4" runat="server" EnableTheming="True" Theme="BlackGlass" 
        Modal="True" AllowDragging="True" AllowResize="True" Width="0px" Height="0px" ShowCloseButton="False"
        CloseAction="CloseButton" HeaderText="Popup"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        ClientInstanceName="popupScreen" EnableViewState="False"
        EnableAnimation="True">
    </dx:ASPxPopupControl>

    <!-- Popup control for demo mode -->
    <dx:ASPxPopupControl ID="DemoModeAlert" runat="server" EnableTheming="True" Theme="BlackGlass" 
        Modal="True" AllowDragging="False" AllowResize="False" ShowCloseButton="False" Width="0px" Height="0px"
        CloseAction="None" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        HeaderText="Demo Mode" ClientInstanceName="demoModeAlert" EnableViewState="False" EnableAnimation="True">
        <ClientSideEvents PopUp=" function(s, e) { StartCountDown(); }" />  
        <ContentCollection>  
            <dx:PopupControlContentControl>  
                <dx:ASPxLabel ID="DemoLabel" runat="server" Text="Please activate your license to remove this message!" Wrap="False" />
                <br />
                <br />
                <div style="text-align: center">
                <dx:ASPxButton ID="ButtonDemoMode" ClientInstanceName="buttonDemoMode" runat="server"
                    Text="OK" AutoPostBack="false">
                    <ClientSideEvents Click="function(s, e) { demoModeAlert.Hide(); }" />  
                </dx:ASPxButton>  
                </div>
            </dx:PopupControlContentControl>  
        </ContentCollection>  
    </dx:ASPxPopupControl>

</asp:Content>

