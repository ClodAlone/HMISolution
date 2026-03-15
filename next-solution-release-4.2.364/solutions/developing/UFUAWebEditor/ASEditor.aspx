<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
        CodeBehind="ASEditor.aspx.cs" Inherits="UFUAWebEditor.ASEditor" %>

<%@ Register Src="DeleteConfirm.ascx" TagName="DeleteConfirm" TagPrefix="uc1" %>
<%@ Register Src="DynamicSettings.ascx" TagName="DynamicSettings" TagPrefix="ds1" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxTreeList.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxTreeList" tagprefix="dx" %>


<%@ Register assembly="DevExpress.Xpo.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>





<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        td.fmToolbar
        {
            padding: 0 2px 3px 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
    // <![CDATA[

        var dontAskConfirmation; //Store the value specifying whether delete operations should be confirmed
        var rowVisibleIndex; //Store the visible index of the grid row to be deleted

        function popupControl_Init(s, e) {
            //Synchronize the client variable's value with the confirm dialog checkbox' setting
            dontAskConfirmation = cbDontAsk.GetChecked();
        }

        function grid_CustomButtonClick(s, e) {
            if (e.buttonID != 'del') return;
            //If confirmation is not required, delete the processed grid row 
            if (dontAskConfirmation) DeleteGridRow(e.visibleIndex);
            //If confirmation is required, preserve the row's visible index and show the confirmation dialog, passing it the row's ID value
            else {
                rowVisibleIndex = e.visibleIndex;
                s.GetRowValues(e.visibleIndex, 'Name', ShowPopup);
            }
        }

        function ShowPopup(rowId) {
            //Assign the row's ID value to a specific label within the confirmation dialog, show the dialog, and set focus to the Yes button
            lbRowId.SetText(rowId);
            popupControl.Show();
            btnYes.Focus();
        }

        function cbDontAsk_CheckedChanged(s, e) {
            //Synchronize the client variable's value with the confirm dialog checkbox' setting, and focus the Yes button
            dontAskConfirmation = cbDontAsk.GetChecked();
            btnYes.Focus();
        }

        function btnYes_Click(s, e) {
            ClosePopup(true);
        }

        function btnNo_Click(s, e) {
            ClosePopup(false);
        }

        function ClosePopup(result) {
            popupControl.Hide();
            if (result) DeleteGridRow(rowVisibleIndex);
        }

        function DeleteGridRow(visibleIndex) {
            grid.DeleteRow(visibleIndex);
        }


        function button1_Click(s, e) {
            if (grid.IsCustomizationWindowVisible())
                grid.HideCustomizationWindow();
            else
                grid.ShowCustomizationWindow();
            UpdateButtonText();
        }
        function grid_CustomizationWindowCloseUp(s, e) {
            UpdateButtonText();
        }
        function UpdateButtonText() {
            var text = grid.IsCustomizationWindowVisible() ? "Hide" : "Show";
            text += " Customization Window";
            button1.SetText(text);
        }

        // Button handlers
        function bNewFolder_Click(s, e) {
            var key = tree.GetFocusedNodeKey();
            tree.StartEditNewNode(key);
        }
        function bNewRoot_Click(s, e) {
            tree.StartEditNewNode();
        }
        function bDelete_Click(s, e) {
            if (!confirm("Are you sure?"))
                return;
            tree.DeleteNode(tree.GetFocusedNodeKey());
        }
        function bRename_Click(s, e) {
            tree.StartEdit(tree.GetFocusedNodeKey());
        }
        // TreeList handlers
        function tree_Init(s, e) {
            FM_UpdateButtons();
        }
        function tree_FocusChanged(s, e) {
            FM_UpdateButtons();
        }
        function tree_BeginCallback(s, e) {
            FM_UpdateButtons(true);
        }
        function tree_EndCallback(s, e) {
            FM_UpdateButtons();
        }
        function tree_NodeClick(s, e) {
            var key = e.nodeKey;
            if (key == tree.GetFocusedNodeKey() && !tree.IsEditing())
                tree.StartEdit(key);
        }
        // Editor handlers
        function editor_Init(s, e) {
            s.SelectAll();
            s.SetFocus();
        }
        function editor_KeyPress(s, e) {
            var code = e.htmlEvent.keyCode;
            if (code == 13)
                tree.UpdateEdit();
            else if (code == 27)
                tree.CancelEdit();
            if (code == 13 || code == 27) {
                var he = e.htmlEvent;
                he.preventDefault && he.preventDefault();
                he.stopPropagation && he.stopPropagation();
                he.returnValue = false;
                he.cancelBubble = true;
            }
        }
        function editor_LostFocus(s, e) {
            tree.UpdateEdit();
        }
        // Uploader handlers
        function uploader_Complete(s, e) {
            tree.PerformCustomCallback("upload_complete");
        }
        // Helper functions
        function FM_UpdateButtons(disableAll) {
            var isEditing = tree.IsEditing();
            var focusedKey = tree.GetFocusedNodeKey();
            bNewFolder.SetEnabled(!disableAll && !isEditing);
            bNewRoot.SetEnabled(!disableAll && !isEditing);
            bDelete.SetEnabled(!disableAll && focusedKey != "" && !isEditing);
            bRename.SetEnabled(!disableAll && focusedKey != "" && !isEditing);
        }

        function DisplayPopup(elementID) {
            popup.SetHeaderText("Editing...");
            popupElement = eval(elementID);
            popup.ShowAtElementByID(popupElement.name);
            clientEditTextbox.SetText(popupElement.GetText());
        }

    // ]]>
    </script>
    <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0" 
        ClientIDMode="AutoID" AutoPostBack="True" Width="100%" 
        onactivetabchanging="ASPxPageControl1_ActiveTabChanging">
        <TabPages>
            <dx:TabPage Name="HierarchyModel" Text="Hierarchy Model">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl1" runat="server" SupportsDisabledAttribute="True">

                        <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" ResizingMode="Live" Height="400px" Width="100%">
                        <panes>
                            <dx:SplitterPane ShowCollapseBackwardButton="True" ScrollBars="Vertical">
                                <ContentCollection>
                                    <dx:SplitterContentControl ID="SplitterContentControl1" runat="server" SupportsDisabledAttribute="True">

                                        <table width="100%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td class="fmToolbar">
                                                    <dx:ASPxButton runat="server" ID="bNewFolder" ClientInstanceName="bNewFolder" UseSubmitBehavior="false"
                                                        AutoPostBack="false" Text="New Folder" Wrap="false">
                                                        <ClientSideEvents Click="bNewFolder_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                                <td class="fmToolbar">
                                                    <dx:ASPxButton runat="server" ID="bNewRoot" ClientInstanceName="bNewRoot" UseSubmitBehavior="false"
                                                        AutoPostBack="false" Text="Add Root" Wrap="false">
                                                        <ClientSideEvents Click="bNewRoot_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                                <td class="fmToolbar">
                                                    <dx:ASPxButton runat="server" ID="bDelete" ClientInstanceName="bDelete" UseSubmitBehavior="false"
                                                        AutoPostBack="false" Text="Delete">
                                                        <ClientSideEvents Click="bDelete_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                                <td class="fmToolbar">
                                                    <dx:ASPxButton runat="server" ID="bRename" ClientInstanceName="bRename" UseSubmitBehavior="false"
                                                        AutoPostBack="false" Text="Rename">
                                                        <ClientSideEvents Click="bRename_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                                <td style="width: 100%">
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        <dx:ASPxTreeList runat="server" ID="tree" ClientInstanceName="tree" Width="100%"
                                        ClientSideEvents-FocusedNodeChanged='<%#                                                "function(s,e){grid.Refresh();}"                                                %>' 
                                        OnNodeInserting="tree_NodeInserting" 
                                        OnNodeUpdating="tree_NodeUpdating" OnNodeDeleting="tree_NodeDeleting" OnNodeValidating="tree_NodeValidating"
                                        AutoGenerateColumns="False" OnVirtualModeCreateChildren="tree_VirtualModeCreateChildren" 
                                        OnVirtualModeNodeCreating="tree_VirtualModeNodeCreating">
                                            <ClientSideEvents Init="tree_Init" FocusedNodeChanged="tree_FocusChanged" BeginCallback="tree_BeginCallback"
                                                        EndCallback="tree_EndCallback" NodeClick="tree_NodeClick" />
                                            <SettingsBehavior AllowFocusedNode="true" />
                                            <SettingsEditing AllowNodeDragDrop="false" AllowRecursiveDelete="true" />

                                            <Columns>
                                                <dx:TreeListTextColumn FieldName="Name" SortIndex="0" SortOrder="Ascending">
                                                    <CellStyle>
                                                        <Paddings Padding="1" />
                                                        <Paddings Padding="1px"></Paddings>
                                                    </CellStyle>
                                                    <EditCellStyle>
                                                        <Paddings Padding="1" />
                                                        <Paddings Padding="1px"></Paddings>
                                                    </EditCellStyle>
                                                    <DataCellTemplate>
                                                        <table width="100%" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td style="width: 20px">
                                                                    <dx:ASPxImage ID="ASPxImage1" runat="server" IsPng="true" Width="21" Height="21" ImageUrl='<%# GetNodeGlyph(Container) %>'
                                                                        ImageAlign="Top" />
                                                                </td>
                                                                <td>
                                                                    <%# Container.Text %>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </DataCellTemplate>
                                                    <EditCellTemplate>
                                                        <table width="100%" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td style="width: 20px">
                                                                    <dx:ASPxImage ID="ASPxImage1" runat="server" IsPng="true" Width="21" Height="21"
                                                                        ImageUrl='<%# GetNodeGlyph(Container) %>' ImageAlign="Top" />
                                                                </td>
                                                                <td>
                                                                    <dx:ASPxTextBox runat="server" ID="ed" Text='<%# Bind("Name") %>' Width="200px">
                                                                        <ClientSideEvents Init="editor_Init" KeyPress="editor_KeyPress" LostFocus="editor_LostFocus" />
                                                                    </dx:ASPxTextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </EditCellTemplate>
                                                </dx:TreeListTextColumn>
                                            </Columns>
                                            <Settings GridLines="Horizontal"></Settings>
                                            <SettingsBehavior AllowFocusedNode="True"></SettingsBehavior>
                                            <SettingsPager Mode="ShowPager">
                                            </SettingsPager>
                                            <Settings GridLines="Horizontal" />
                                            <SettingsEditing AllowRecursiveDelete="True"></SettingsEditing>
                                            <ClientSideEvents FocusedNodeChanged="tree_FocusChanged" 
                                                NodeClick="tree_NodeClick" 
                                                BeginCallback="tree_BeginCallback" 
                                                EndCallback="tree_EndCallback" 
                                                Init="tree_Init">
                                            </ClientSideEvents>
                                        </dx:ASPxTreeList>
                                    </dx:SplitterContentControl>
                                </ContentCollection>
                            </dx:SplitterPane>
                            <dx:SplitterPane ShowCollapseBackwardButton="True" ScrollBars="Vertical">
                                <ContentCollection>
                                    <dx:SplitterContentControl ID="SplitterContentControl2" runat="server" SupportsDisabledAttribute="True">
                                        <dx:ASPxButton runat="server" ID="button1" ClientInstanceName="button1" Text="Show Customization Window"
                                                    UseSubmitBehavior="false" AutoPostBack="false">
                                                    <ClientSideEvents Click="button1_Click" />
                                        </dx:ASPxButton>

                                        <br />

                                        <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" 
                                            ClientInstanceName="grid"
                                            KeyFieldName="Oid" OnDataBinding="ASPxGridView1_DataBinding" Width="100%" 
                                            OnRowDeleting="ASPxGridView1_RowDeleting" 
                                            OnRowInserting="ASPxGridView1_RowInserting" 
                                            OnRowUpdating="ASPxGridView1_RowUpdating" 
                                            OnInitNewRow="ASPxGridView1_InitNewRow" 
                                            OnCellEditorInitialize="ASPxGridView1_CellEditorInitialize"
                                            OnRowValidating="ASPxGridView1_RowValidating" 
                                            OnAutoFilterCellEditorInitialize="ASPxGridView1_AutoFilterCellEditorInitialize">
                                            <ClientSideEvents CustomizationWindowCloseUp="grid_CustomizationWindowCloseUp" CustomButtonClick="grid_CustomButtonClick"></ClientSideEvents>
                                            <Columns>
                                                <dx:GridViewCommandColumn ShowInCustomizationForm="True"
                                                    ShowSelectCheckbox="True" VisibleIndex="0">
                                                    <EditButton Visible="True">
                                                    </EditButton>
                                                    <NewButton Visible="True">
                                                    </NewButton>
                                                    <CustomButtons>
                                                        <dx:GridViewCommandColumnCustomButton Text="Delete" ID="del">
                                                        </dx:GridViewCommandColumnCustomButton>
                                                    </CustomButtons>
                                                </dx:GridViewCommandColumn>
                                                <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="1">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="2">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="InitialValue" VisibleIndex="3">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="DynamicSettings" VisibleIndex="4">
                                                    <EditItemTemplate>
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <dx:ASPxButtonEdit ID="txtDynSett" runat="server" ClientInstanceName="txtDynSett" 
                                                                        Text='<%# Eval("DynamicSettings")%>'
                                                                        ReadOnly="false">
                                                                        <Buttons>
                                                                            <dx:EditButton>
                                                                            </dx:EditButton>
                                                                        </Buttons>
                                                                        <ClientSideEvents 
                                                                            ButtonClick="function(s, e) {
	                                                                                            DisplayPopup('txtDynSett');
                                                                                            }" />
                                                                    </dx:ASPxButtonEdit>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </EditItemTemplate>
                                                </dx:GridViewDataTextColumn>                           
                                                <dx:GridViewDataComboBoxColumn FieldName="DataType" VisibleIndex="5">
                                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>
                                                <dx:GridViewDataComboBoxColumn FieldName="ModelType" VisibleIndex="6">
                                                    <PropertiesComboBox ValueType="System.String">
                                                    
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) {   
                                                        debugger;
                                                        var value = s.GetValue();
                                                            if(value == &quot;ObjectType&quot;)
                                                                grid.GetEditor(&quot;PrototypeModel&quot;).SetVisible(true);
                                                            else
                                                                grid.GetEditor(&quot;PrototypeModel&quot;).SetVisible(false);

                                                            if(value == &quot;Analog&quot; || 
                                                               value == &quot;Variable&quot;)
                                                            {
                                                                grid.GetEditor(&quot;DataType&quot;).SetVisible(true);
                                                                grid.GetEditor(&quot;HistorianSettings&quot;).SetVisible(true);
                                                                grid.GetEditor(&quot;UFUAEngineeringUnit&quot;).SetVisible(true);
                                                            }
                                                            else
                                                            {
                                                                grid.GetEditor(&quot;DataType&quot;).SetVisible(false);
                                                                grid.GetEditor(&quot;HistorianSettings&quot;).SetVisible(false);
                                                                grid.GetEditor(&quot;UFUAEngineeringUnit&quot;).SetVisible(false);
                                                            }
                                                        }" />
                                                    
                                                    </PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>
                                                <dx:GridViewDataComboBoxColumn FieldName="PrototypeModel" VisibleIndex="7">
                                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>  

                                                <dx:GridViewDataComboBoxColumn FieldName="HistorianSettings" VisibleIndex="8">
                                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>

                                                <dx:GridViewDataComboBoxColumn FieldName="UFUAEngineeringUnit" VisibleIndex="9">
                                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>
                                            </Columns>
                                            <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                                            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" Mode=PopupEditForm/>
                                            <SettingsLoadingPanel Mode="ShowOnStatusBar" />
                                            <SettingsCustomizationWindow Enabled="True" />
                                            <ClientSideEvents CustomizationWindowCloseUp="grid_CustomizationWindowCloseUp" CustomButtonClick="grid_CustomButtonClick"/>

                                            <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="600px" PopupEditFormModal="True"></SettingsEditing>

                                            <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>

                                            <SettingsCustomizationWindow Enabled="True"></SettingsCustomizationWindow>

                                            <SettingsLoadingPanel Mode="ShowOnStatusBar"></SettingsLoadingPanel>
                                        </dx:ASPxGridView>
                                    </dx:SplitterContentControl>
                                </ContentCollection>
                            </dx:SplitterPane>
                        </panes>
                    </dx:ASPxSplitter>
                    
                    <dx:ASPxPopupControl ID="ASPxPopupControlConfirmDelete" runat="server" ClientInstanceName="popupControl" >
                        <ClientSideEvents Init="popupControl_Init" />
                        <ContentCollection>
                            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                                <uc1:DeleteConfirm ID="DeleteConfirmControl" runat="server" />
                            </dx:PopupControlContentControl>
                        </ContentCollection>
                    </dx:ASPxPopupControl>       


                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Name="FlatModel" Text="Flat Model">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl2" runat="server" SupportsDisabledAttribute="True">

                        <dx:ASPxGridView ID="ASPxGridViewFlat" runat="server" AutoGenerateColumns="False" 
                            ClientInstanceName="gridFlat"
                            KeyFieldName="Oid" OnDataBinding="ASPxGridViewFlat_DataBinding" Width="100%" 
                            OnRowDeleting="ASPxGridView1_RowDeleting" 
                            OnRowInserting="ASPxGridView1_RowInserting" 
                            OnRowUpdating="ASPxGridView1_RowUpdating" 
                            OnCellEditorInitialize="ASPxGridView1_CellEditorInitialize"
                            OnRowValidating="ASPxGridView1_RowValidating" 
                            OnAutoFilterCellEditorInitialize="ASPxGridView1_AutoFilterCellEditorInitialize">
                            <ClientSideEvents CustomizationWindowCloseUp="grid_CustomizationWindowCloseUp"></ClientSideEvents>
                            <Columns>
                                <dx:GridViewCommandColumn ShowInCustomizationForm="True" 
                                    ShowSelectCheckbox="True" VisibleIndex="0">
                                    <EditButton Visible="True">
                                    </EditButton>
                                    <NewButton Visible="True">
                                    </NewButton>
                                    <DeleteButton Visible="True">
                                    </DeleteButton>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="FolderPath" VisibleIndex="1" ReadOnly="true">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="2">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="InitialValue" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="DynamicSettings" VisibleIndex="5">
                                    <EditItemTemplate>
                                        <table>
                                            <tr>
                                                <td>
                                                    <dx:ASPxButtonEdit ID="txtDynSett" runat="server" ClientInstanceName="txtDynSett"
                                                        Text='<%# Eval("DynamicSettings")%>'
                                                        ReadOnly="false">
                                                        <Buttons>
                                                            <dx:EditButton>
                                                            </dx:EditButton>
                                                        </Buttons>
                                                        <ClientSideEvents ButtonClick="function(s, e) {
	                                                                        DisplayPopup('txtDynSett');
                                                                        }" />
                                                    </dx:ASPxButtonEdit>
                                                </td>
                                            </tr>
                                        </table>
                                    </EditItemTemplate>
                                </dx:GridViewDataTextColumn>                           
                                <dx:GridViewDataComboBoxColumn FieldName="DataType" VisibleIndex="6">
                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="ModelType" VisibleIndex="7">
                                    <PropertiesComboBox ValueType="System.String">
                                        
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) {   
                                                        debugger;
                                                        var value = s.GetValue();
                                                            if(value == &quot;ObjectType&quot;)
                                                                gridFlat.GetEditor(&quot;PrototypeModel&quot;).SetVisible(true);
                                                            else
                                                                 gridFlat.GetEditor(&quot;PrototypeModel&quot;).SetVisible(false);

                                                            if(value == &quot;Analog&quot; || 
                                                               value == &quot;Variable&quot;)
                                                            {
                                                                gridFlat.GetEditor(&quot;DataType&quot;).SetVisible(true);
                                                                gridFlat.GetEditor(&quot;HistorianSettings&quot;).SetVisible(true);
                                                                gridFlat.GetEditor(&quot;UFUAEngineeringUnit&quot;).SetVisible(true);
                                                            }
                                                            else
                                                            {
                                                                gridFlat.GetEditor(&quot;DataType&quot;).SetVisible(false);
                                                                gridFlat.GetEditor(&quot;HistorianSettings&quot;).SetVisible(false);
                                                                gridFlat.GetEditor(&quot;UFUAEngineeringUnit&quot;).SetVisible(false);
                                                            }
                                                        }" />
                                    
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="PrototypeModel" VisibleIndex="8">
                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>                
                                <dx:GridViewDataComboBoxColumn FieldName="HistorianSettings" VisibleIndex="9">
                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                   
                                <dx:GridViewDataComboBoxColumn FieldName="UFUAEngineeringUnit" VisibleIndex="10">
                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                            </Columns>
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" Mode=PopupEditForm/>
                            <SettingsLoadingPanel Mode="ShowOnStatusBar" />
                            <SettingsCustomizationWindow Enabled="True" />
                            <ClientSideEvents CustomizationWindowCloseUp="grid_CustomizationWindowCloseUp" />
                            <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="600px" PopupEditFormModal="True"></SettingsEditing>
                            <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>
                            <SettingsCustomizationWindow Enabled="True"></SettingsCustomizationWindow>
                            <SettingsLoadingPanel Mode="ShowOnStatusBar"></SettingsLoadingPanel>
                        </dx:ASPxGridView>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Name="Prototypes" Text="Prototypes">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl3" runat="server" SupportsDisabledAttribute="True">
                        <dx:ASPxGridView ID="ASPxGridViewPrototypes" runat="server" 
                        AutoGenerateColumns="False" Width="100%" ClientInstanceName="gridPrototype"
                            ClientIDMode="AutoID" DataSourceID="XpoDataSource1" KeyFieldName="Oid">
                            <Settings ShowFilterRow="True" ShowGroupPanel="True"></Settings>
                            <SettingsDetail ShowDetailRow="True"></SettingsDetail>
                                <Templates>
                                    <DetailRow>
                                        <dx:ASPxGridView ID="ASPxGridViewPrototypeMembers" runat="server" AutoGenerateColumns="False"
                                            DataSourceID="XpoDataSource2" KeyFieldName="Oid" 

                                            OnCellEditorInitialize="ASPxGridView1_CellEditorInitialize"
                                            OnRowValidating="ASPxGridView1_RowValidating" 
                                            OnAutoFilterCellEditorInitialize="ASPxGridView1_AutoFilterCellEditorInitialize"

                                            OnBeforePerformDataSelect="ASPxGridViewPrototypeMembers_BeforePerformDataSelect"
                                            OnInitNewRow="ASPxGridViewPrototypeMembers_InitNewRow" 
                                            OnRowInserting="ASPxGridViewPrototypeMembers_RowInserting">
                                            <Columns>
                                                <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="0">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Description" VisibleIndex="0">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataComboBoxColumn FieldName="DataType" VisibleIndex="0">
                                                    <PropertiesComboBox ValueType="System.String"></PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>
                                                <dx:GridViewDataComboBoxColumn FieldName="ModelType" VisibleIndex="0">
                                                    <PropertiesComboBox ValueType="System.String">
                                                    
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) {   
                                                        debugger;
                                                        var value = s.GetValue();
                                                            if(value == &quot;ObjectType&quot;)
                                                                gridPrototype.GetEditor(&quot;PrototypeModel&quot;).SetVisible(true);
                                                            else
                                                                 gridPrototype.GetEditor(&quot;PrototypeModel&quot;).SetVisible(false);

                                                            if(value == &quot;Analog&quot; || 
                                                               value == &quot;Variable&quot;)
                                                                gridPrototype.GetEditor(&quot;DataType&quot;).SetVisible(true);
                                                            else
                                                                 gridPrototype.GetEditor(&quot;DataType&quot;).SetVisible(false);

                                                        }" />
                                                    
                                                    </PropertiesComboBox>
                                                </dx:GridViewDataComboBoxColumn>

                                                <dx:GridViewCommandColumn ShowInCustomizationForm="True" 
                                                    ShowSelectCheckbox="True" VisibleIndex="-1">
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
                        TypeName="UFUAModel.UFUATagPrototype">
                    </dx:XpoDataSource>
                    <dx:XpoDataSource ID="XpoDataSource2" runat="server" ServerMode="True" 
                        TypeName="UFUAModel.UFUATag" Criteria="[UFUATagPrototype!Key] = ?">
                        <CriteriaParameters>
                            <asp:SessionParameter DefaultValue="-1" Name="unnamedParam0" SessionField="UFUATagPrototypeKey" />
                        </CriteriaParameters>
                    </dx:XpoDataSource>

                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
            <dx:TabPage Text="Engineering Unit Settings">
                <ContentCollection>
                    <dx:ContentControl ID="ContentControl4" runat="server" SupportsDisabledAttribute="True">
                        <dx:ASPxGridView ID="ASPxGridView2" runat="server" ClientIDMode="AutoID" 
                            Width="100%" AutoGenerateColumns="False" DataSourceID="XpoDataSource3" 
                            KeyFieldName="Oid">
                            <Columns>
                                <dx:GridViewCommandColumn ShowInCustomizationForm="True" 
                                    ShowSelectCheckbox="True" VisibleIndex="0">
                                    <EditButton Visible="True">
                                    </EditButton>
                                    <NewButton Visible="True">
                                    </NewButton>
                                    <DeleteButton Visible="True">
                                    </DeleteButton>
                                    <ClearFilterButton Visible="True">
                                    </ClearFilterButton>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="Oid" ReadOnly="True" 
                                    ShowInCustomizationForm="True" VisibleIndex="0">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Name" ShowInCustomizationForm="True" 
                                    VisibleIndex="1">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="UnitName" ShowInCustomizationForm="True" 
                                    VisibleIndex="2">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="EURangeLow" 
                                    ShowInCustomizationForm="True" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="EURangeHigh" 
                                    ShowInCustomizationForm="True" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="InstrumentRangeLow" 
                                    ShowInCustomizationForm="True" VisibleIndex="5">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="InstrumentRangeHigh" 
                                    ShowInCustomizationForm="True" VisibleIndex="6">
                                </dx:GridViewDataTextColumn>
                            </Columns>
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                        </dx:ASPxGridView>
                        <dx:XpoDataSource ID="XpoDataSource3" runat="server" 
                            TypeName="UFUAModel.UFUAEngineeringUnit" ServerMode="True">
                        </dx:XpoDataSource>
                    </dx:ContentControl>
                </ContentCollection>
            </dx:TabPage>
        </TabPages>
    </dx:ASPxPageControl>
                        
    <dx:ASPxPopupControl ID="ASPxPopupControl1" runat="server" Height="177px" Width="273px"
    ClientInstanceName="popup" Modal="True">
    <ContentCollection>
        <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
            <dx:ASPxTextBox runat="server" Width="100%" ClientInstanceName="clientEditTextbox"
                ID="ASPxTextBox2">
            </dx:ASPxTextBox>
            <ds1:DynamicSettings ID="DynamicSettings" runat="server" />
            <dx:ASPxButton ID="btnSubmit" runat="server" Text="OK" AutoPostBack="false" OnClick="btnSubmit_Click">
                <ClientSideEvents Click="function(s, e) {
                        popup.Hide();
                }" />
            </dx:ASPxButton>
        </dx:PopupControlContentControl>
    </ContentCollection>
    </dx:ASPxPopupControl>
</asp:Content>
