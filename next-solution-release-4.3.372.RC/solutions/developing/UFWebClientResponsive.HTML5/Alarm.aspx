<%@ Page Title="Alarms" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeBehind="Alarm.aspx.cs" Inherits="UFWebClient.HTML5.Alarm" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <script type="text/javascript" language="javascript" src="Scripts/JScriptAlarmManager.js"></script>
<%--    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/AlarmSinkService.svc" />
        </Services>
        <Scripts>
            <asp:ScriptReference  Path="~/Scripts/JScriptAlarmManager.js" />    
        </Scripts>
    </asp:ScriptManager>    --%>
    <table class="BottomMargin">
        <tr>
            <td>
                <dx:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="false" UseSubmitBehavior="false"
                    Text="Ack All">
                    <ClientSideEvents Click="function() { AckAll() }" />
                </dx:ASPxButton>
            </td>
            <td style="padding-left: 4px">
                <dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="false" UseSubmitBehavior="false"
                    Text="Confirm All">
                    <ClientSideEvents Click="function() { ConfirmAll() }" />
                </dx:ASPxButton>
            </td>

            <td>
                <dx:ASPxButton ID="ASPxButton3" runat="server" AutoPostBack="false" UseSubmitBehavior="false"
                    Text="Ack Selected" ClientInstanceName="btnActSelected" ClientEnabled="False">
                    <ClientSideEvents Click="function() { AckSelected() }" />
                </dx:ASPxButton>
            </td>
            <td style="padding-left: 4px">
                <dx:ASPxButton ID="ASPxButton4" runat="server" AutoPostBack="false" UseSubmitBehavior="false"
                    Text="Confirm Selected" ClientInstanceName="btnConfirmSelected" ClientEnabled="False">
                    <ClientSideEvents Click="function() { ConfirmSelected() }" />
                </dx:ASPxButton>
            </td>
            <td style="padding-left: 4px">
                <dx:ASPxButton ID="ASPxButton5" runat="server" AutoPostBack="false" UseSubmitBehavior="false"
                    Text="Refresh" ClientInstanceName="btnRefresh">
                    <ClientSideEvents Click="function() { Refresh() }" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <dx:ASPxGridView style="padding-left: 4px"
        ID="grid" runat="server" DataSourceID="ObjectDataSource1" KeyFieldName="NodeIdString"
        Width="100%" AutoGenerateColumns="False">
        <ClientSideEvents Init="grid_Init" BeginCallback="grid_BeginCallback" EndCallback="grid_EndCallback" 
            SelectionChanged="grid_SelectionChanged"/>
        <Columns>
            <dx:GridViewDataDateColumn Caption="Time ON" FieldName="TimeOn" Width="200px">
                <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd hh:mm:ss" >
                </PropertiesDateEdit>
                <DataItemTemplate>
                    <%# GetTimeText(Container,"TimeOn") %>                    
                </DataItemTemplate>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn Caption="AckedTransitionTime" FieldName="AckedTransitionTime" Width="200px">
                <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd hh:mm:ss" >
                </PropertiesDateEdit>
                <DataItemTemplate>
                    <%# GetTimeText(Container,"AckedTransitionTime") %>                    
                </DataItemTemplate>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn Caption="ConfirmedTransitionTime" FieldName="ConfirmedTransitionTime" Width="200px">
                <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd hh:mm:ss" >
                </PropertiesDateEdit>
                <DataItemTemplate>
                    <%# GetTimeText(Container,"ConfirmedTransitionTime") %>                    
                </DataItemTemplate>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn Caption="ShelvingTransitionTime" FieldName="ShelvingTransitionTime" Width="200px">
                <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd hh:mm:ss" >
                </PropertiesDateEdit>
                <DataItemTemplate>
                    <%# GetTimeText(Container,"ShelvingTransitionTime") %>                    
                </DataItemTemplate>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataTextColumn Caption="Message" FieldName="Message" />
            <dx:GridViewDataTextColumn Caption="Source" FieldName="Source" Width="120px" />
            <dx:GridViewDataTextColumn Caption="State" FieldName="EnabledState" Width="120px">
                <DataItemTemplate>
                    <dx:ASPxImage runat="server" ID="icon" ImageUrl="<%# GetIconStateImageUrl(Container) %>" style="margin: 2px 4px;" Width="10" Height="10" />
                    <%# GetStateText(Container) %>                    
                </DataItemTemplate>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataDateColumn Caption="Change State Time" FieldName="ActiveEffectiveTransitionTime" Width="200px">
                <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd hh:mm:ss" >
                </PropertiesDateEdit>
                <DataItemTemplate>
                    <%# GetTimeText(Container,"ActiveEffectiveTransitionTime") %>                    
                </DataItemTemplate>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataTextColumn Caption="Condition" FieldName="Condition" Width="120px" />
            <dx:GridViewDataTextColumn Caption="Severity" FieldName="Severity" Width="120px" />
        </Columns>
        <Styles>
            <Header HorizontalAlign="Center" />
        </Styles>
        <Settings GridLines="Both" />
        <Settings ShowGroupPanel="False" />
        <Settings ShowFilterBar="Hidden" />
        <SettingsBehavior AllowDragDrop="false" />
        <SettingsBehavior AllowSelectByRowClick="true" />
        <SettingsLoadingPanel Mode="ShowOnStatusBar" />
        <Settings VerticalScrollableHeight="500" />
        <SettingsPager PageSize="20">
            <PageSizeItemSettings Visible="true" ShowAllItem="true" />
        </SettingsPager>
    </dx:ASPxGridView>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetAlarms"
        TypeName="AlarmProvider" />

    <dx:ASPxLoadingPanel ID="ASPxLoadingPanel1" runat="server" Modal="true" HorizontalAlign="Center"        
            ClientInstanceName="LoadingPanel" Text="Please, wait...">               
<%--         <ClientSideEvents Init="function() { InitClientOffset() }" />--%>
    </dx:ASPxLoadingPanel>

    <dx:ASPxLoadingPanel ID="ASPxLoadingPanel2" runat="server" Modal="false" HorizontalAlign="Center"        
            ClientInstanceName="ConnectingPanel" Text="Connecting to server...">               
<%--         <ClientSideEvents Init="function() { InitClientOffset() }" />--%>
    </dx:ASPxLoadingPanel>

</asp:Content>
