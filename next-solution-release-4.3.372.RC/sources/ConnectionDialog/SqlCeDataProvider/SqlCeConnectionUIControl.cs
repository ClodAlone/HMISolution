//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using Microsoft.Data.ConnectionUI;
using Microsoft.Win32;


namespace Microsoft.Data.ConnectionUI
{
	/// <summary>
	/// Represents the connection UI control for the SQL Server Compact provider.
	/// </summary>
	internal partial class SqlCeConnectionUIControl : UserControl, IDataConnectionUIControl
	{
		private bool _loading;

		private SqlCeConnectionProperties _properties;

		public SqlCeConnectionUIControl()
		{
			InitializeComponent();
			RightToLeft = RightToLeft.Inherit;

			// Disable the active sync radio button for standalone connection dialog.
			this.activeSyncRadioButton.Enabled = false;
			this.createButton.Enabled = false;
		}

		private string DataSourceProperty
		{
			get
			{
				return "Data Source";
			}
		}

		public static string MobileDevicePrefix
		{
			get
			{
				return "Mobile Device" + @"\";
			}
		}

		private string PasswordProperty
		{
			get
			{
				return "Password";
			}
		}


		public string PersistSecurityInfoProperty
		{
			get
			{
				return "Persist Security Info";
			}
		}


		public void Initialize(IDataConnectionProperties connectionProperties)
		{
			if (connectionProperties == null)
			{
				throw new ArgumentNullException("connectionProperties");
			}
			SqlCeConnectionProperties properties = connectionProperties as SqlCeConnectionProperties;
			if (properties == null)
			{
				throw new ArgumentException(Resources.SqlCeConnectionUIControl_InvalidConnectionProperties);
			}
			_properties = properties;
		}

		public void LoadProperties()
		{
			_loading = true;

			string dataSource = _properties[DataSourceProperty] as string;
			myComputerRadioButton.Checked = true;
			databaseTextBox.Text = dataSource;
			passwordTextBox.Text = _properties[PasswordProperty] as string;
			savePasswordCheckBox.Checked = (bool)_properties[PersistSecurityInfoProperty];
            createButton.Enabled = _properties.IsComplete;

            _loading = false;
		}


		private void myComputerRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			databaseTextBox_TextChanged(sender, e);
		}

		private void activeSyncRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			databaseTextBox_TextChanged(sender, e);
		}

		private void databaseTextBox_TextChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				string dataSource = databaseTextBox.Text.Trim();
				if (activeSyncRadioButton.Checked)
				{
					dataSource = Path.Combine(MobileDevicePrefix, dataSource);
				}
				if (dataSource.Length == 0)
				{
					dataSource = null;
				}
				_properties[DataSourceProperty] = dataSource;
                createButton.Enabled = _properties.IsComplete;
            }
		}

        private void createButton_Click(object sender, EventArgs e)
        {
            if (myComputerRadioButton.Checked)
            {
                if (!String.IsNullOrEmpty(_properties[DataSourceProperty] as string))
                {
                    var fileName = _properties[DataSourceProperty] as string;
                    var directoryName = Path.GetDirectoryName(fileName);
                    if (String.IsNullOrEmpty(directoryName))
                        _properties[DataSourceProperty] = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

                    using (SqlCeEngine engine = new SqlCeEngine(_properties.ToFullString()))
                    {
                        engine.CreateDatabase();
                    }
                }
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
		{
			if (myComputerRadioButton.Checked)
			{
				//
				// We're exploring the desktop, let's use an OpenFileDialog
				//
				using (OpenFileDialog fileDialog = new OpenFileDialog())
				{
					fileDialog.Title = Resources.SqlConnectionUIControl_BrowseFileTitle;
					fileDialog.Multiselect = false;
					if (String.IsNullOrEmpty(_properties[DataSourceProperty] as string))
					{
						fileDialog.InitialDirectory = InitialDirectory;
					}
					fileDialog.RestoreDirectory = true;
					fileDialog.Filter = Resources.SqlConnectionUIControl_BrowseFileFilter;
					fileDialog.DefaultExt = Resources.SqlConnectionUIControl_BrowseFileDefaultExt;
					if (fileDialog.ShowDialog() == DialogResult.OK)
					{
						_properties[DataSourceProperty] = fileDialog.FileName.Trim();
						LoadProperties();
					}
				}
			}
		}

		private void passwordTextBox_TextChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_properties[PasswordProperty] = (passwordTextBox.Text.Length > 0) ? passwordTextBox.Text : null;
				passwordTextBox.Text = passwordTextBox.Text; // forces reselection of all text
			}
		}

		private void savePasswordCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_properties[PersistSecurityInfoProperty] = savePasswordCheckBox.Checked;
			}
		}

		private void TrimControlText(object sender, EventArgs e)
		{
			Control c = sender as Control;
			c.Text = c.Text.Trim();
		}

		private static string InitialDirectory
		{
			get
			{
				string path = null;
				RegistryKey sqlCEBaseRegKey = Registry.LocalMachine.OpenSubKey(
					@"SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v4.0");
				if (sqlCEBaseRegKey != null)
				{
					using (sqlCEBaseRegKey)
					{
						path = sqlCEBaseRegKey.GetValue("InstallDir") as string;
						if (path != null)
						{
							path = Path.Combine(path, "Samples");
						}
					}
				}
				return path;
			}
		}
    }
}
