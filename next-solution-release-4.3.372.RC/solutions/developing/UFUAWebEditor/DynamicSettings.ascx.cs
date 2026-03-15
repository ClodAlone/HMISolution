using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Xpo;
using DevExpress.Web;
using DriverSettingsInterfaces;
using Utilities;
using System.Text;
using Xacc;

namespace UFUAWebEditor
{
    public partial class DynamicSettings : System.Web.UI.UserControl
    {
        readonly Session session = XpoHelper.GetNewSession();
        readonly Dictionary<string, string> MapFriendlyName = new Dictionary<string, string>();
        readonly Dictionary<string, string> MapPath = new Dictionary<string, string>();
        static ICommunicationDriverWebEditing CurrentCommDriver = null;
 
        protected void Page_Load(object sender, EventArgs e)
        {
            XpoDataSource1.Session = XpoHelper.GetNewSession();
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TODOE: Remove the part bellow
            if (!IsPostBack)
            {
                //var list = FindAndLoadDLL.LoadDLLs<ICommunicationDriverWebEditing>(@"E:\PRIVATE\12-0-Drivers\ModBus\bin\Debug\", "ModBus.dll");
                //if (list.Count > 0)
                //{
                //    string dynsettings = String.Empty;
                //    ASPxTextBox txtDyn = Parent.FindControl("ASPxTextBox2") as ASPxTextBox;
                //    if (txtDyn != null)
                //        dynsettings = txtDyn.Text;

                //    pgDynamicSettings.SelectedObject = list[0].DynamicSettingsEditor(dynsettings);
                //}
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        protected void ASPxComboBoxDriversList_Init(object sender, EventArgs e)
        {
            var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(session)/*.AsParallel()*/
                        select tag.ComunicationDrivers).ToList();

            MapFriendlyName.Clear();
            ASPxComboBoxDriversList.Items.Clear();
            if (list.Count > 0)
            {
                foreach (var drv in list[0])
                {
                    ASPxComboBoxDriversList.Items.Add(drv.FriendlyName);
                    MapFriendlyName[drv.FriendlyName] = drv.Name;
                    MapPath[drv.Name] = drv.Path;
                }
            }
        }
        
        protected void ASPxCallback1_Callback(object sender, CallbackEventArgs e)
        {
            ASPxCallback callbackControl = sender as ASPxCallback;
            if (e.Parameter == "LoadCommDriver")
            { 
                CurrentCommDriver = null;
                var item = ASPxComboBoxDriversList.Text;
                if (MapFriendlyName.ContainsKey(item))
                {
                    var name = MapFriendlyName[item];
                    var path = MapPath[name];
                    var list = FindAndLoadDLL.LoadDLLs<ICommunicationDriverWebEditing>(path, name + ".dll");
                    if (list.Count > 0)
                        CurrentCommDriver = list[0];
                }
            }
            else if (e.Parameter == "LoadDynSettings" && CurrentCommDriver != null)
            {
                string dynsettings = String.Empty;
                ASPxTextBox txtDyn = Parent.FindControl("ASPxTextBox2") as ASPxTextBox;
                if (txtDyn != null)
                    dynsettings = txtDyn.Text;

                pgDynamicSettings.SelectedObject = null;
                ASPxCallback.GetRenderResult(pgDynamicSettings);
                pgDynamicSettings.SelectedObject = CurrentCommDriver.DynamicSettingsEditor(dynsettings);
                e.Result = ASPxCallback.GetRenderResult(pgDynamicSettings);

            }
            else if (e.Parameter == "LoadDrvSettings" && CurrentCommDriver != null)
            {
                pgGeneralSettings.SelectedObject = CurrentCommDriver.GeneralSettingsEditor;
                e.Result = ASPxCallback.GetRenderResult(pgGeneralSettings);
            }
        }

        protected void pgDynamicSettings_PropertyChanged(object sender, RefObjectEventArgs e)
        {
            if (CurrentCommDriver != null)
            {
                ASPxTextBox txtDyn = Parent.FindControl("ASPxTextBox2") as ASPxTextBox;
                if (txtDyn != null)
                    txtDyn.Text = CurrentCommDriver.GetDynamicSettings(e.RefObject);
            }
        }
                   
    }
}