<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DCEditor.aspx.cs" Inherits="UFUAWebEditor.DCEditor" %>
<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Xpo.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <dx:XpoDataSource ID="XpoDataSource1" runat="server" ServerMode="false" 
        TypeName="UFUAModel.UFUAConfiguration">
    </dx:XpoDataSource>
    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" 
        ClientIDMode="AutoID" DataSourceID="XmlDataSource1" KeyFieldName="Driver" Width="100%">
        <Columns>
            <dx:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
            <EditButton Visible="True">
            </EditButton>
            <NewButton Visible="True">
            </NewButton>
            <DeleteButton Visible="True">
            </DeleteButton>
            <ClearFilterButton Visible="True">
            </ClearFilterButton>
            <dx:GridViewDataCheckColumn VisibleIndex="0" ReadOnly="false">
            <DataItemTemplate>
                <dx:ASPxCheckBox ID="chk" runat="server" OnInit="chk_Init" OnCheckedChanged="chk_Checked">
                </dx:ASPxCheckBox>
            </DataItemTemplate> 
            </dx:GridViewDataCheckColumn>
            <dx:GridViewDataTextColumn FieldName="Factory" VisibleIndex="1" GroupIndex="0" SortOrder="Ascending">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="2" Caption="Driver Name">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Help" VisibleIndex="3" Caption="Description">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataCheckColumn FieldName="Free" VisibleIndex="4">
            </dx:GridViewDataCheckColumn>
        </Columns>
        <Settings ShowFilterRow="True" ShowGroupPanel="True" />
    </dx:ASPxGridView>
    <asp:XmlDataSource ID="XmlDataSource1" runat="server" 
        DataFile="~/App_Data/Drivers.xml" EnableCaching="false">
    </asp:XmlDataSource>
</asp:Content>