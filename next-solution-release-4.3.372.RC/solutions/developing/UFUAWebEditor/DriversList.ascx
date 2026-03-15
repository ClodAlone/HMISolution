<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DriversList.ascx.cs" Inherits="UFUAWebEditor.DriversList" %>
<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Xpo.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>
<dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False"
    ClientIDMode="AutoID" DataSourceID="XmlDataSource1" KeyFieldName="AssemblyName" Width="100%"
    OnAutoFilterCellEditorInitialize="ASPxGridView1_AutoFilterCellEditorInitialize"
    OnHtmlRowCreated="ASPxGridView1_HtmlRowCreated">
    <Columns>
        <dx:GridViewCommandColumn ShowInCustomizationForm="true" ShowSelectCheckbox="true" VisibleIndex="0">
        </dx:GridViewCommandColumn>
        <dx:GridViewDataTextColumn FieldName="Factory" VisibleIndex="1" GroupIndex="0" SortOrder="Ascending">
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataTextColumn FieldName="FriendlyName" VisibleIndex="2" Caption="Name">
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataTextColumn FieldName="Help" VisibleIndex="3" Caption="Description">
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataComboBoxColumn FieldName="Free" VisibleIndex="4" Caption="Free">
            <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataTextColumn FieldName="Path" Visible="false">
        </dx:GridViewDataTextColumn>
    </Columns>
    <Settings ShowFilterRow="True" ShowGroupPanel="True" />
</dx:ASPxGridView>
<asp:XmlDataSource ID="XmlDataSource1" runat="server" 
    DataFile="~/App_Data/Drivers.xml" EnableCaching="false">
</asp:XmlDataSource>
<dx:XpoDataSource ID="XpoDataSource1" runat="server" ServerMode="true" 
    TypeName="UFUAModel.UFUAConfiguration">
</dx:XpoDataSource>

