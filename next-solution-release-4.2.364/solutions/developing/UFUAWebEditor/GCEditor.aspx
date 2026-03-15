
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GCEditor.aspx.cs" Inherits="UFUAWebEditor.GCEditor" %>

<%@ Register Src="DriversList.ascx" TagName="DriversList" TagPrefix="dl1" %>

<%@ Register Assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Xpo.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Xpo" TagPrefix="dx" %>





<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0" 
        ClientIDMode="AutoID" AutoPostBack="True" Width="100%">
        <TabPages>
            <dx:TabPage Name="General" Text="General">
                <ContentCollection>
                        <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                            <dx:XpoDataSource ID="XpoDataSource1" runat="server" ServerMode="false" 
                                TypeName="UFUAModel.UFUAConfiguration">
                            </dx:XpoDataSource>
                            <asp:DetailsView ID="DetailsView" runat="server"
                            ClientIDMode="AutoID" ColumnCount="1" DataSourceID="XpoDataSource1" 
                            Layout="Flow" AutoGenerateRows="False" DataKeyNames="Oid" DefaultMode="Edit">
                                <Fields>
                                    <asp:BoundField DataField="Oid" HeaderText="Oid" ReadOnly="True" Visible="false"
                                        SortExpression="Oid" />
                                    <asp:BoundField DataField="ManufacturerName" HeaderText="ManufacturerName" 
                                        SortExpression="ManufacturerName" />
                                    <asp:BoundField DataField="ProductName" HeaderText="ProductName" 
                                        SortExpression="ProductName" />
                                    <asp:BoundField DataField="ProductUri" HeaderText="ProductUri" 
                                        SortExpression="ProductUri" />
                                    <asp:BoundField DataField="SoftwareVersion" HeaderText="SoftwareVersion" 
                                        SortExpression="SoftwareVersion" />
                                    <asp:BoundField DataField="BuildNumber" HeaderText="BuildNumber" 
                                        SortExpression="BuildNumber" />
                                    <asp:BoundField DataField="BuildDate" HeaderText="BuildDate" 
                                        SortExpression="BuildDate" />
                                    <asp:BoundField DataField="AliasRoot" HeaderText="AliasRoot" 
                                        SortExpression="AliasRoot" />
                                    <asp:BoundField DataField="NamespaceUri" HeaderText="NamespaceUri" 
                                        SortExpression="NamespaceUri" />
                                    <asp:BoundField DataField="HistorianDefaultConnection" 
                                        HeaderText="HistorianDefaultConnection" 
                                        SortExpression="HistorianDefaultConnection" />
                                    <asp:CommandField ShowEditButton="True" />
                                </Fields>
                            </asp:DetailsView>
                        </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Name="Drivers" Text="Drivers">
                <ContentCollection>
                    <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                        <dx:ASPxButton ID="ASPxButtonAddDriver" runat="server" ClientIDMode="AutoID" Text="Add/Remove Drivers" >
                            <ClientSideEvents Click="function(s, e) {
                                    DriversListPopup.Show();
                                }" />
                        </dx:ASPxButton>
                        <br />
                        <dx:ASPxGridView ID="ASPxGridViewDrivers" runat="server" ClientInstanceName="grid" Width="100%" 
                            AutoGenerateColumns="False" ClientIDMode="AutoID" KeyFieldName="Name"
                            OnDataBinding="ASPxGridViewDrivers_DataBinding"
                            OnStartRowEditing="ASPxGridViewDrivers_StartRowEditing"
                            OnHtmlRowCreated="ASPxGridViewDrivers_RowCreated">
                            <Columns>
                                <dx:GridViewCommandColumn ShowSelectCheckbox="false" VisibleIndex="0" ButtonType="Link">
                                    <EditButton Visible="True">
                                    </EditButton>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="Name" ReadOnly="true" Visible="false">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Path" ReadOnly="true" Visible="false">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="FriendlyName" Caption="Name" ReadOnly="true" VisibleIndex="1">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Full Path" Caption="Path" ReadOnly="true" VisibleIndex="2">
                                <DataItemTemplate>
                                    <dx:ASPxLabel ID="txtFullPath" Width="100%" runat="server">
                                    </dx:ASPxLabel>
                                </DataItemTemplate>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Version" ReadOnly="true" VisibleIndex="3">
                                <DataItemTemplate>
                                    <dx:ASPxLabel ID="txtVersion" Width="100%" runat="server">
                                    </dx:ASPxLabel>
                                </DataItemTemplate>

                                </dx:GridViewDataTextColumn>
                            </Columns>
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                        </dx:ASPxGridView>
                        <dx:ASPxPopupControl ID="ASPxPopupDriversList" runat="server" ShowCloseButton="true"
                            PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Width="600px" Height="400px"
                            ClientSideEvents-CloseUp="function(s, e) { 
                                    CallbackControl.PerformCallback();
                                    grid.Refresh();
                                }" 
                            ClientInstanceName="DriversListPopup" Modal="True">
<ClientSideEvents CloseUp="function(s, e) { 
                                    CallbackControl.PerformCallback();
                                    grid.Refresh();
                                }"></ClientSideEvents>
                            <ContentCollection>
                                <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server" SupportsDisabledAttribute="True">
                                <dl1:DriversList ID="DriversList1" runat="server"/>
                                </dx:PopupControlContentControl>
                            </ContentCollection>
                        </dx:ASPxPopupControl>
                        <dx:ASPxCallback ID="ASPxCallback1" runat="server" ClientInstanceName="CallbackControl"
                            OnCallback="ASPxCallback1_Callback">
                        </dx:ASPxCallback>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
        </TabPages>
    </dx:ASPxPageControl>
</asp:Content>
