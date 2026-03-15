//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Microsoft.Data.ConnectionUI
{
	public partial class SQLiteConnectionUIControl : UserControl, IDataConnectionUIControl
	{
		public SQLiteConnectionUIControl()
		{
			InitializeComponent();
			RightToLeft = RightToLeft.Inherit;

		}

		public void Initialize(IDataConnectionProperties connectionProperties)
		{
			if (connectionProperties == null)
			{
				throw new ArgumentNullException("connectionProperties");
			}

			if (!(connectionProperties is SQLiteConnectionProperties) &&
				!(connectionProperties is OleDBConnectionProperties))
			{
				throw new ArgumentException(Strings.SqliteConnectionUIControl_InvalidConnectionProperties);
			}

			_connectionProperties = connectionProperties;
		}

		public void LoadProperties()
		{
			try
			{
				_loading = true;

				if (Properties.Contains(ServerProperty))
					serverTextBox.Text = Properties[ServerProperty] as string;
				if (Properties.Contains(PasswordProperty))
					passwordTextBox.Text = Properties[PasswordProperty] as string;
			}
			finally
			{
				_loading = false;
			}
		}

		// Simulate RTL mirroring
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			if (ParentForm != null &&
				ParentForm.RightToLeftLayout == true &&
				RightToLeft == RightToLeft.Yes)
			{
				LayoutUtils.MirrorControl(DatabaseLabel, serverTextBox);
			}
			else
			{
				LayoutUtils.UnmirrorControl(DatabaseLabel, serverTextBox);
			}
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			Size baseSize = Size;
			MinimumSize = Size.Empty;
			base.ScaleControl(factor, specified);
			MinimumSize = new Size(
				(int)Math.Round((float)baseSize.Width * factor.Width),
				(int)Math.Round((float)baseSize.Height * factor.Height));
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (Parent == null)
			{
				OnFontChanged(e);
			}
		}

		private void SetServer(object sender, System.EventArgs e)
		{
			if (!_loading)
			{
				Properties[ServerProperty] = (serverTextBox.Text.Trim().Length > 0) ? serverTextBox.Text.Trim() : null;
			}
		}

		private void SetPassword(object sender, EventArgs e)
		{
			if (!_loading)
			{
				Properties[PasswordProperty] = !String.IsNullOrWhiteSpace(passwordTextBox.Text) ? passwordTextBox.Text : null;
				passwordTextBox.Text = passwordTextBox.Text; // forces reselection of all text
			}
		}

		private void TrimControlText(object sender, EventArgs e)
		{
			Control c = sender as Control;
			c.Text = c.Text.Trim();
		}

		private string ServerProperty
		{
			get
			{
				if (!(Properties is OdbcConnectionProperties))
				{
					return "DataSource";
				}
				else
				{
					return "SERVER";
				}
			}
		}

		private string PasswordProperty
		{
			get
			{
				if (!(Properties is OdbcConnectionProperties))
				{
					return "Password";
				}
				else
				{
					return "PWD";
				}
			}
		}

		private IDataConnectionProperties Properties
		{
			get
			{
				return _connectionProperties;
			}
		}

		private bool _loading;
		private IDataConnectionProperties _connectionProperties;

        private void Browse(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = Strings.SqliteConnectionUIControl_BrowseFileTitle;
            fileDialog.Multiselect = false;
            fileDialog.CheckFileExists = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = Strings.SqliteConnectionUIControl_BrowseFileFilter;
            fileDialog.DefaultExt = Strings.SqliteConnectionUIControl_BrowseFileDefaultExt;
            fileDialog.FileName = Properties["DataSource"] as string;
            if (this.Container != null)
            {
                this.Container.Add(fileDialog);
            }
            try
            {
                DialogResult result = fileDialog.ShowDialog(ParentForm);
                if (result == DialogResult.OK)
                {
                    serverTextBox.Text = fileDialog.FileName.Trim();
                }
            }
            finally
            {
                if (this.Container != null)
                {
                    this.Container.Remove(fileDialog);
                }
                fileDialog.Dispose();
            }
        }
    }
}
