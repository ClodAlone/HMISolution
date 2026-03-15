<%@ Page Title="Reports" Language="C#" MasterPageFile="~/ServerStuff/ServerStuff.master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="UFWebClient.HTML5.Report" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.XtraReports.v21.2.Web.WebForms, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraReports.Web" tagprefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    Reports
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">

    <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" Width="100%" Height="1024px">
        <panes>
            <dx:SplitterPane Size="20%" Name="listBoxContainer" ShowCollapseBackwardButton="True">
                <ContentCollection>
                    <dx:SplitterContentControl ID="SplitterContentControl1" runat="server" SupportsDisabledAttribute="True">
						<dx:ASPxListBox ID="ASPxListBox1" runat="server" Height="100%" Width="100%" AutoPostBack="true" />
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>
            <dx:SplitterPane>
                <ContentCollection>
                    <dx:SplitterContentControl ID="SplitterContentControl2" runat="server" SupportsDisabledAttribute="True">
                        <dx:ASPxWebDocumentViewer ID="ASPxDocumentViewer1" runat="server" Theme="Office2010Black" />
                        <dx:ASPxLabel ID="LabelErrorInfo" runat="server" Width="100%" Height="100%" Wrap="True" Font-Bold="true" />
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>
        </panes>
    </dx:ASPxSplitter>

</asp:Content>
