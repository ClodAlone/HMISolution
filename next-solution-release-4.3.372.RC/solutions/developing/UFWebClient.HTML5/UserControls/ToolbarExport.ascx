<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToolbarExport.ascx.cs" Inherits="UFWebClient.HTML5.UserControls.ToolbarExport" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.13.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<style type="text/css" scoped="scoped">
    .exportToolbarCaption {
        height: 38px;
        padding: 0 6px 0 6px;
        text-align: center;
        display: table-cell;
        vertical-align: middle;
        cursor: default;
    }
    li[id*=MenuExportButtons_DXI1_IS] {
        display: none;
    }
</style>
<div class="OptionsBottomMargin">
    <dx:ASPxMenu runat="server" ID="MenuExportButtons" ClientInstanceName="MenuExportButtons" ShowAsToolbar="true" OnItemClick="MenuExportButtons_ItemClick" AutoPostBack="true" Theme="Metropolis">
        <Items>
            <dx:MenuItem Name="ToolbarCaption">
                <Template>
                    <div class="exportToolbarCaption">
                        <dx:ASPxLabel runat="server" ID="labelCaption" Text="Export type:" Theme="Metropolis" />
                    </div>
                </Template>
            </dx:MenuItem>
        </Items>
    </dx:ASPxMenu>
</div>
