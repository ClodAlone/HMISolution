<%@ Page Title="Screens" Language="C#" AutoEventWireup="true" MasterPageFile="~/LightPopup.master"
    CodeBehind="PopupScreen.aspx.cs" Inherits="UFWebClient.HTML5.PopupScreen" %>

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
    </script>
    <script type="text/javascript">
        function ReturnToParentPage() {
            var parentWindow = window.parent;
            CloseCurrentPopupScreen();
            parentWindow.SelectAndClosePopup();
        }
    </script>
        
    <dx:ASPxButton ID="ASPxButton4" runat="server" AutoPostBack="False" Text="Close" ClientEnabled="True">
        <ClientSideEvents Click="ReturnToParentPage" />
    </dx:ASPxButton>

    <div id="canvasContainer" style="position: relative;left:0px;top:0px;width:100%;height:100%;">
        <canvas id="canvasBackground" 
                style="position: absolute; left: 0; top: 0; z-index: 0;">
        </canvas>
        <canvas id="canvasMain"
                style="position: absolute; left: 0; top: 0; z-index: 1;">
                (Your browser doesn't support canvas : this is mandatory for this application)
        </canvas>
    </div>

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

    <dx:ASPxPopupControl ID="ASPxPopupControl5" runat="server" EnableTheming="True" Theme="BlackGlass" 
        CloseAction="CloseButton" Modal="True" EnableViewState="False"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        ClientInstanceName="editListValue"
        HeaderText="Edit Value" AllowDragging="True" EnableAnimation="True">
        <ClientSideEvents PopUp="function(s, e) { editList.Focus(); }" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl4" runat="server" SupportsDisabledAttribute="True">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" DefaultButton="ASPxButton4">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxListBox ID="ASPxListBox1" runat="server" ClientInstanceName="editList" >
                            </dx:ASPxListBox>
                            <dx:ASPxButton ID="ASPxButton5" runat="server" Text="OK" AutoPostBack="False" EnableViewState="False" Theme="BlackGlass">
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
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
        CloseAction="CloseButton" HeaderText="Popup"
        ClientInstanceName="popupScreen" EnableViewState="False"
        EnableAnimation="True">
    </dx:ASPxPopupControl>

</asp:Content>

