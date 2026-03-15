<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LTEditor.aspx.cs" Inherits="UFUAWebEditor.LTEditor" %>
<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Xpo.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" Width="100%"
        ClientIDMode="AutoID" DataSourceID="XpoDataSource1" KeyFieldName="Oid">
            <Templates>
                <DetailRow>
                    <dx:ASPxGridView ID="ASPxGridView2" runat="server" AutoGenerateColumns="False"
                        DataSourceID="XpoDataSource2" KeyFieldName="Oid" OnBeforePerformDataSelect="ASPxGridView2_BeforePerformDataSelect"
                        OnInitNewRow="ASPxGridView2_InitNewRow" OnRowInserting="ASPxGridView2_RowInserting">
                        <Columns>
                            <dx:GridViewDataTextColumn FieldName="Text" VisibleIndex="3">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Locale" VisibleIndex="4">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewCommandColumn VisibleIndex="0">
                                <EditButton Visible="True">
                                </EditButton>
                                <NewButton Visible="True">
                                </NewButton>
                                <DeleteButton Visible="True">
                                </DeleteButton>
                            </dx:GridViewCommandColumn>
                        </Columns>
                        <SettingsDetail IsDetailGrid="True" />
                    </dx:ASPxGridView>
                </DetailRow>
            </Templates>
        <Columns>
            <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="1">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Locale" VisibleIndex="1">
            </dx:GridViewDataTextColumn>
            <dx:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
                <EditButton Visible="True">
                </EditButton>
                <NewButton Visible="True">
                </NewButton>
                <DeleteButton Visible="True">
                </DeleteButton>
            </dx:GridViewCommandColumn>
        </Columns>
        <SettingsDetail ShowDetailRow="True" />
        <Settings ShowFilterRow="True" ShowGroupPanel="True" />
    </dx:ASPxGridView>
    <dx:XpoDataSource ID="XpoDataSource1" runat="server" ServerMode="True" 
        TypeName="UFUAModel.UFUALocale">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="XpoDataSource2" runat="server" ServerMode="True" 
        TypeName="UFUAModel.UFUALocaleText" Criteria="[UFUALocale!Key] = ?">
        <CriteriaParameters>
            <asp:SessionParameter DefaultValue="-1" Name="unnamedParam0" SessionField="UFUALocaleKey" />
        </CriteriaParameters>
    </dx:XpoDataSource>
</asp:Content>
