using System;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;

namespace DriverCodeBaseEx.UI.SettingsControls
{
    /// <summary>
    /// Interaction logic for StructureStringLengths.xaml
    /// </summary>
    public partial class StructureStringLengths : UserControl
    {        
        public uint MAX_STRING_LENGHT = 255;

        public IDynamicSettingsEditing ThisTag = null;

        public StructureStringLengths()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                btnSet.Click += SetStructStringLength_Click;
                btnReset.Click += ResetStructStringLength_Click;
            };

            Unloaded += (o, e) =>
            {                
                btnSet.Click -= SetStructStringLength_Click;
                btnReset.Click -= ResetStructStringLength_Click;
            };
        }
    

        private void ResetStructStringLength_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;

            var nDC = DataContext as DynTagSettings;
            nDC.StructStringFieldLengths = String.Empty;
        }

        private void SetStructStringLength_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;            

            if (ThisTag.Members == null || ThisTag.Members.Count == 0)
            {
                MessageBox.Show(Properties.Resources.ErrorPrototypeNotExist);
                TbStructStringLength.Text = string.Empty;
                return;
            }

            StructStringLength SSL = new StructStringLength();
            SSL.Parse(ThisTag, TbStructStringLength.Text);
            // check if prototype contain at least one string's member
            if (!SSL.HasMembers())
            {
                MessageBox.Show(Properties.Resources.ErrorNoOnePrototypeMembersIsString);
                TbStructStringLength.Text = string.Empty;
                return;
            }

            PrototypeMemberStringLength d = new PrototypeMemberStringLength(MAX_STRING_LENGHT);
            d.DataContext = SSL.MemberView;

            GeneralDialogContent newChDetDialog = new GeneralDialogContent(d, GeneralDialogButtons.OkCancelButtons, false)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.PrototypeEditor
            };

            if (newChDetDialog.ShowDialog() == true)
            {
                StructStringLength.ProMemberView v = d.DataContext as StructStringLength.ProMemberView;
                if (v != null)
                    TbStructStringLength.Text = SSL.UnSplitToStructString(v);
            }
        }
    }
}
