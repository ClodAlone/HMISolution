<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynamicSettings.ascx.cs" Inherits="UFUAWebEditor.DynamicSettings" %>
<%@ Register Assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register TagPrefix="xacc" Namespace="Xacc" Assembly="xacc.propertygrid" %>







<%@ Register assembly="DevExpress.Xpo.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>


<script type="text/javascript">

    function LoadDriverSettings(s, e) {
        CallbackControl.PerformCallback('LoadDrvSettings');
        LoadingPanel.SetText('Loading Driver Settings...')
        LoadingPanel.ShowInElement(s.GetMainElement());
    }

    function LoadDynamicSettings(s, e) {
        CallbackControl.PerformCallback('LoadDynSettings');
        LoadingPanel.SetText('Loading Dynamic Settings...')
        LoadingPanel.ShowInElement(s.GetMainElement());
    }

    function LoadCommDriver(s, e) {
        CallbackControl.PerformCallback('LoadCommDriver');
        LoadingPanel.SetText('Loading Communication Driver...')
        LoadingPanel.ShowInElement(s.GetMainElement());
    }
</script>

<dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0"
    ClientIDMode="AutoID" AutoPostBack="false">
    <ClientSideEvents ActiveTabChanging="function(s, e) {
        if (e.tab.name == 'GeneralSettings')
            LoadDriverSettings(s, e)
        else if (e.tab.name == 'DynamicSettings')
            LoadDynamicSettings(s, e)
    }" />
    <TabPages>
        <dx:TabPage Name="Drivers" Text="Drivers">
            <ContentCollection>
                <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                    <dx:ASPxComboBox ID="ASPxComboBoxDriversList" runat="server" ClientIDMode="AutoID"
                        OnInit="ASPxComboBoxDriversList_Init">
                    <ClientSideEvents TextChanged="LoadCommDriver" />
                    </dx:ASPxComboBox>
                </dx:ContentControl>
            </ContentCollection>
        </dx:TabPage>
        <dx:TabPage Name="DynamicSettings" Text="Dynamic Settings">
            <ContentCollection>
                <dx:ContentControl runat="server" SupportsDisabledAttribute="True">
                <div id="dynSettings">
                    <xacc:propertygrid ID="pgDynamicSettings" runat="server" ShowHelp="true"
                    OnPropertyChanged="pgDynamicSettings_PropertyChanged">
                    </xacc:propertygrid>
                </div>
                </dx:ContentControl>
            </ContentCollection>
        </dx:TabPage>
        <dx:TabPage Name="GeneralSettings" Text="General Settings">
            <ContentCollection>
                <dx:ContentControl ID="ContentControl1" runat="server" SupportsDisabledAttribute="True">
                <div id="drvSettings">
                    <xacc:propertygrid ID="pgGeneralSettings" runat="server" ShowHelp="true">
                    </xacc:propertygrid>
                </div>
                </dx:ContentControl>
            </ContentCollection>
        </dx:TabPage>
    </TabPages>
</dx:ASPxPageControl>
    
<%--<div id="drvSettings">
    <asp:PlaceHolder ID="DrvSettings" runat="server">
    </asp:PlaceHolder>
</div>--%>

<dx:ASPxCallback ID="ASPxCallback1" runat="server" ClientInstanceName="CallbackControl"
    OnCallback="ASPxCallback1_Callback">
    <ClientSideEvents CallbackComplete="function(s, e) {
        LoadingPanel.Hide();
        if (e.parameter == 'LoadDynSettings' && e.result != null)
            document.getElementById('dynSettings').innerHTML = e.result;
        else if (e.parameter == 'LoadDrvSettings' && e.result != null)
            document.getElementById('drvSettings').innerHTML = e.result;
    }" />
</dx:ASPxCallback>


<%--<asp:UpdatePanel ID="SettingsUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <dx:ASPxPopupControl ID="ASPxPopupControl1" ClientInstanceName="popupDrvSettings" runat="server">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:PlaceHolder ID="DrvSettings" runat="server">
                </asp:PlaceHolder>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    </ContentTemplate>
</asp:UpdatePanel>--%>


<dx:XpoDataSource ID="XpoDataSource1" runat="server"
    TypeName="UFUAModel.UFUAConfiguration">
</dx:XpoDataSource>

<%--<dx:ASPxCallback ID="ASPxCallback1" runat="server" ClientInstanceName="CallbackControl"
    OnCallback="ASPxCallback1_Callback">
    <ClientSideEvents CallbackComplete="function(s, e) {
        LoadingPanel.Hide();
    }" />
</dx:ASPxCallback>--%>

<dx:ASPxLoadingPanel ID="ASPxLoadingPanel1" runat="server" ClientInstanceName="LoadingPanel">
</dx:ASPxLoadingPanel>



