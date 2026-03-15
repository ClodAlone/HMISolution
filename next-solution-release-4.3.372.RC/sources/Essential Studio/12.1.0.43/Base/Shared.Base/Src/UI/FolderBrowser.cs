#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Tools;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// The FolderBrowser component provides a convenient and easy to use object oriented wrapper for the
	/// Win32 Shell folder browser API.
	/// </summary>
	/// <remarks>
	/// The FolderBrowser class completely abstracts the various complex Shell API functions,
	/// structures and callback routines required for invoking the folder selection dialog and allows
	/// you to work with a more .NET-centric programming model consisting of aptly named properties, methods and
	/// events. Most convenient of all, you no longer need to allocate PIDLs as specifying
	/// the location of the rootfolder; browsing is now a simple task of setting the
	/// FolderBrowser.StartLocation property from one of the values provided in the FolderBrowserFolder
	/// enumeration. Using the FolderBrowser class to browse the Shell folders is simple and
	/// to the .NET developer a completely familiar issue of instantiating the FolderBrowser
	/// component, setting the appropriate properties and events on it and invoking the
	/// <see cref="FolderBrowser.ShowDialog()"/> method. For more detailed information on the Shell APIs refer
	/// to the Platform SDK documentation on the SHBrowseForFolder method.
	/// </remarks>
	/// <example>
	/// The following code creates an instance of the FolderBrowser component, sets the folder dialog start location 
	/// and styles and invokes the FolderBrowser.ShowDialog() method: 
	/// 
	/// <coderef file="Shared\Samples\FolderBrowserDemo\CS\Form1.cs" name="FolderBrowser" lang="C#"><code lang="C#">
	///		private void ShowFolderBrowserDialog()
	///		{
	///			// Create the FolderBrowser component:
	///			this.folderBrowser1 = new Syncfusion.Windows.Forms.FolderBrowser();
	///
	///			// Initialize the FolderBrowser component:
	///			this.folderBrowser1.Description = "Syncfusion FolderBrowser";
	///			this.folderBrowser1.StartLocation = Syncfusion.Windows.Forms.FolderBrowserFolder.Desktop;
	///			this.folderBrowser1.Style = 
	///				( Syncfusion.Windows.Forms.FolderBrowserStyles.RestrictToFilesystem | 
	///				Syncfusion.Windows.Forms.FolderBrowserStyles.BrowseForComputer );
	///
	///			// Provide a handler for the FolderBrowserCallback validation event:
	///			this.folderBrowser1.FolderBrowserCallback += new Syncfusion.Windows.Forms.FolderBrowserCallbackEventHandler(this.folderBrowser1_BrowseCallback);
	///
	///			// Display the folderbrowser dialog:
	///			if (this.folderBrowser1.ShowDialog() == DialogResult.OK)
	///				this.selectedFolder = this.folderBrowser1.DirectoryPath;
	///		}</code></coderef>
	/// 
	/// <coderef file="Shared\Samples\FolderBrowserDemo\CS\Form1.cs" name="FolderBrowserCallback" lang="C#"><code lang="C#">
	///		// Event handler for the FolderBrowser.FolderBrowserCallback validation event.
	///		// This handler is functionally equivalent of the Win32 BrowseCallbackProc callback function:
	///		private void folderBrowser1_BrowseCallback(object sender, Syncfusion.Windows.Forms.FolderBrowserCallbackEventArgs e)
	///		{
	///			this.label1.Text = String.Format("Event: {0}, Path: {1}", e.FolderBrowserMessage, e.Path);
	///
	///			if (e.FolderBrowserMessage == FolderBrowserMessage.ValidateFailed)
	///			{
	///				e.Dismiss = e.Path != "NONE";
	///			}
	///		}</code></coderef>
	/// 
	/// 
	/// <coderef file="Shared\Samples\FolderBrowserDemo\VB\Form1.vb" name="FolderBrowser" lang="VB"><code lang="VB">
	///        Private Sub ShowFolderBrowserDialog()
	///
	///            ' Create an instance of the FolderBrowser component:
	///            Me.folderBrowser1 = New Syncfusion.Windows.Forms.FolderBrowser(Me.components)
	///
	///            ' Set the descriptor text that will appear on the folder dialog:
	///            Me.folderBrowser1.Description = "Syncfusion FolderBrowser"
	///
	///            ' Specify the start location:
	///            Me.folderBrowser1.StartLocation = Syncfusion.Windows.Forms.FolderBrowserFolder.Desktop
	///
	///            ' Specify the styles for the folder browser dialog:
	///            Me.folderBrowser1.Style = Syncfusion.Windows.Forms.FolderBrowserStyles.RestrictToFilesystem Or Syncfusion.Windows.Forms.FolderBrowserStyles.BrowseForComputer
	///
	///            ' Provide a handler for the FolderBrowserCallback validation event:
	///            AddHandler Me.folderBrowser1.FolderBrowserCallback, New Syncfusion.Windows.Forms.FolderBrowserCallbackEventHandler(AddressOf folderBrowser1_BrowseCallback)
	///
	///            ' Show the folder browser dialog:
	///            Try
	///                If folderBrowser1.ShowDialog() = DialogResult.OK Then
	///                    Me.label1.Text = [String].Concat("Selection: ", folderBrowser1.DirectoryPath)
	///                End If
	///            Catch ex As Exception
	///                Console.WriteLine(ex.ToString())
	///            End Try
	///
	///        End Sub</code></coderef>
	///        
	/// <coderef file="Shared\Samples\FolderBrowserDemo\VB\Form1.vb" name="FolderBrowserCallback" lang="VB"><code lang="VB">
	///        ' Event handler for the FolderBrowser.FolderBrowserCallback validation event.
	///        ' This handler is functionally equivalent of the Win32 BrowseCallbackProc callback function:
	///        Private Sub folderBrowser1_BrowseCallback(ByVal sender As Object, ByVal e As FolderBrowserCallbackEventArgs)
	///
	///            Me.label1.Text = String.Format("Event: {0}, Path: {1}", e.FolderBrowserMessage, e.Path)
	///            If (e.FolderBrowserMessage = FolderBrowserMessage.ValidateFailed) Then
	///                e.Dismiss = (e.Path Is "NONE") = False
	///            End If
	///
	///        End Sub</code></coderef>
	///
	/// </example>
	//[Designer(typeof(FolderBrowserDesigner), typeof(System.ComponentModel.Design.IDesigner)),
	[System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.FolderBrowser), "ToolboxIcons.FolderBrowser.bmp"),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Represents a FolderBrowser component which provides a convenient and easy to use object oriented wrapper for the Win32 Shell folder browser API.")
	]
	public class FolderBrowser:
		Component,
		ICustomTypeDescriptor
	{
		// Constant
		const string c_sFileProtocolPrefix = "file://";
		const string c_sStartLocation = "StartLocation";
		const string c_sCustomStartLocation = "CustomStartLocation";

		// Fields
		private FolderBrowserFolder startLocation = FolderBrowserFolder.Desktop;
		private FolderBrowserStyles option = FolderBrowserStyles.RestrictToFilesystem;
		private string descriptionText = "";
		private string directoryPath = "";
		private string m_sSelectLocation = String.Empty;
		private string m_sCustomStartLocation = String.Empty;
		private static readonly int MAX_PATH = 0x104;

		/// <summary>
		/// Occurs when an event within the folder browser dialog triggers a call to the validation callback.
		/// </summary>
		/// <remarks>
		/// <seealso cref="FolderBrowserCallbackEventHandler"/>
		/// </remarks>		
		[
		Description("The validate event occurs when an event within the folder browser dialog triggers a call to the validation callback." ),
		]
		public event FolderBrowserCallbackEventHandler FolderBrowserCallback;

		
		/// <summary>
		/// Gets or sets the options for the folder browser dialog.
		/// </summary>
		/// <value>A <see cref="FolderBrowserStyles"/> value.</value>
		[
		Description("Specifies the options for the folder browser dialog." ),
		Category("Behavior"),
        DefaultValue(typeof(FolderBrowserStyles), "RestrictToFilesystem")
		]
		public FolderBrowserStyles Style 
		{ 
			get { return this.option; }
			set
			{
				if ((value&(FolderBrowserStyles.ShowAdministrativeShares|FolderBrowserStyles.AllowUrls)) != 0
					&& (value&FolderBrowserStyles.NewDialogStyle) == 0)
					throw new ArgumentException("NewDialogStyle must be specified when using ShowShares, ShowAdministrativeShares or AllowUrls", "value");

				if((value&FolderBrowserStyles.UAHint) != 0
					&& (value&FolderBrowserStyles.NewDialogStyle) == 0)
					throw new ArgumentException("NewDialogStyle must be specified when using UAHint", "value");

				this.option = value;
			} 
		}
        
		/// <summary>
		/// Retrieves the location of the selected folder.
		/// </summary>
		/// <value>A String value.</value>
		[
		Description("Specifies the location of the selected folder." ),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public string DirectoryPath 
		{ 
			get
			{
				//new FileIOPermission(FileIOPermissionAccess.PathDiscovery,this.directoryPath).Demand();
				return this.directoryPath;
			}
		}
        
		/// <summary>
		/// Gets or sets the start location for the folder browser dialog.
		/// </summary>
		/// <remarks>
		/// The StartLocation property is the functional equivalent of the Win32 PIDLs.
		/// </remarks>
		/// <value>A <see cref="FolderBrowserFolder"/> value.</value>
		[
		Description("The location of the root folder from which to start browsing. Functional equivalent of setting the PIDL value." ),
		Category("Behavior"),
        DefaultValue(typeof(FolderBrowserFolder), "Desktop"),
		RefreshProperties(RefreshProperties.All)
		]
		public FolderBrowserFolder StartLocation 
		{ 
			get { return this.startLocation; } 
			set
			{
				new UIPermission(UIPermissionWindow.AllWindows).Demand();
				this.startLocation = value;
			}
		}

		/// <summary>
		/// Gets or sets custom start location for showing dialog.
		/// </summary>
		[
			Description( "Get or set custom start location for showing dialog." ),
			Category( "Behavior" ),
			DefaultValue( "" )
		]
		public string CustomStartLocation
		{
			get
			{
				return m_sCustomStartLocation;
			}
			set
			{
				if( m_sCustomStartLocation != value )
				{
					m_sCustomStartLocation = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets selected location for showing dialog.
		/// </summary>
		[
			Description( "Get or set selected location for showing dialog." ),
			Category( "Behavior" ),
			DefaultValue( "" )
		]
		public string SelectLocation
		{
			get
			{
				return m_sSelectLocation;
			}
			set
			{
				if( value != m_sSelectLocation )
				{
					m_sSelectLocation = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the text displayed above the tree control in the folder browser dialog.
		/// </summary>
		/// <value>A String value.</value>
		[
		Description("Specifies the string displayed above the tree control in the folder browser dialog." ),
		DefaultValue(""),
		Category("Appearance")
		]
		public string Description 
		{ 
			get	{ return this.descriptionText; } 
			set
			{
				new UIPermission(UIPermissionWindow.AllWindows).Demand();
				this.descriptionText = value==null ? "" : value;
			}
		}

		
		/// <summary>
		/// Overloaded. Creates a new instance of the <see cref="FolderBrowser"/> component.
		/// </summary>	
		public FolderBrowser()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(FolderBrowser));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="FolderBrowser"/> and initializes it with the container.
		/// </summary>
		/// <param name="container">An object implementing IContainer that will host this instance 
		/// of the FolderBrowser component.</param>
		public FolderBrowser(IContainer container)
		{
			container.Add(this);
		}

        /// <summary>
        /// Validates the procedure.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="uMsg"></param>
        /// <param name="lParam"></param>
        /// <param name="lpData"></param>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool ValidateProc(IntPtr hwnd, int uMsg, IntPtr lParam, IntPtr lpData)
		{
			if (Enum.IsDefined(typeof(FolderBrowserMessage), uMsg))
			{
				NativeWindowSubclass window = new NativeWindowSubclass();
				window.AssignHandleCustom(hwnd);

				string path = String.Empty;
				FolderBrowserMessage bffm = (FolderBrowserMessage)uMsg;

				if( bffm == FolderBrowserMessage.Initialized && this.SelectLocation != String.Empty )
				{
					NativeMethods.PostMessage( hwnd, NativeMethods.BFFM_SETSELECTIONW, new IntPtr(1), Marshal.StringToHGlobalAuto(SelectLocation) );
				}
				else if(bffm == FolderBrowserMessage.SelChanged)
				{
					IntPtr pszpath = Marshal.AllocHGlobal(FolderBrowser.MAX_PATH);
					NativeMethods.SHGetPathFromIDList(lParam,pszpath);
					path = Marshal.PtrToStringAuto(pszpath);					
					Marshal.FreeHGlobal(pszpath);
				}
				else if(bffm == FolderBrowserMessage.ValidateFailed)
				{
					path = Marshal.PtrToStringAuto(lParam);
				}

				bool noDismissDialog = true;
				try
				{
					FolderBrowserCallbackEventArgs fbe = new FolderBrowserCallbackEventArgs(window, bffm, path);
					this.OnFolderBrowserCallback(fbe);					
					noDismissDialog = !fbe.Dismiss;
				}
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;
				}				
				finally
				{
					window.ReleaseHandleCustom();
				}

				return noDismissDialog;
			}

			return false;
		}

		/// <summary>
		/// Raises the <see cref="FolderBrowser.FolderBrowserCallback"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="FolderBrowserCallbackEventArgs"/> value that contains the event data.</param>
		protected virtual void OnFolderBrowserCallback(FolderBrowserCallbackEventArgs arg)
		{
			if(this.FolderBrowserCallback != null)
			{
				this.FolderBrowserCallback(this, arg);

				if(arg.FolderBrowserCallbackSetState != FolderBrowserCallbackSetState.None)
				{
					IntPtr lpdata = IntPtr.Zero;
					if(	(arg.FolderBrowserCallbackSetState != FolderBrowserCallbackSetState.DisableOK) && 
						(arg.FolderBrowserCallbackSetState != FolderBrowserCallbackSetState.EnableOK) )
						lpdata = Marshal.StringToCoTaskMemUni(arg.BrowseCallbackText);

					switch(arg.FolderBrowserCallbackSetState)
					{
						case FolderBrowserCallbackSetState.DisableOK:
							Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(arg.Window.Handle, 
								NativeMethods.BFFM_ENABLEOK, IntPtr.Zero, (IntPtr)0);
							break;
						case FolderBrowserCallbackSetState.EnableOK:
							Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(arg.Window.Handle, 
								NativeMethods.BFFM_ENABLEOK, IntPtr.Zero, (IntPtr)1);
							break;
						case FolderBrowserCallbackSetState.SetExpanded:
							Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(arg.Window.Handle, 
								NativeMethods.BFFM_SETEXPANDED, (IntPtr)1, lpdata); 
							break;
						case FolderBrowserCallbackSetState.SetOKText:
							Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(arg.Window.Handle, 
								NativeMethods.BFFM_SETOKTEXT, IntPtr.Zero, lpdata);
							break;
						case FolderBrowserCallbackSetState.SetSelection:
							Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(arg.Window.Handle, 
								NativeMethods.BFFM_SETSELECTIONW, (IntPtr)1, lpdata); 
							break;
						case FolderBrowserCallbackSetState.SetStatusText:
							Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage(arg.Window.Handle, 
								NativeMethods.BFFM_SETSTATUSTEXTW, IntPtr.Zero, lpdata); 
							break;
					}
					if(lpdata != IntPtr.Zero)
						Marshal.FreeCoTaskMem(lpdata);
				}
			}
		}

		private static NativeMethods.IMalloc GetSHMalloc()
		{
			NativeMethods.IMalloc[] malloc = new NativeMethods.IMalloc[1];
			NativeMethods.SHGetMalloc(malloc);
			return malloc[0];
		}
        
		/// <summary>
		/// Overloaded. Displays the folder browser dialog with a default owner.
		/// </summary>
		/// <returns>DialogResult.OK if the user clicks OK in the folder dialog; DialogResult.Cancel otherwise.</returns>
		public DialogResult ShowDialog()
		{
			return this.ShowDialog(null);
		}
        
		/// <summary>
		/// Displays the folder browser dialog with the specified owner.
		/// </summary>
		/// <param name="owner">A top-level window that will serve as the owner for the dialog.</param>
		/// <returns>DialogResult.OK if the user clicks OK in the folder dialog; DialogResult.Cancel otherwise.</returns>
		public DialogResult ShowDialog(IWin32Window owner)
		{
			IntPtr hwnd;
			if (owner != null) 
				hwnd = owner.Handle;
			else 
				hwnd = NativeMethods.GetActiveWindow();

			IntPtr spidl = IntPtr.Zero;
			if( this.startLocation == FolderBrowserFolder.CustomStartLocation )
			{
				string csl = this.CustomStartLocation.Trim();
				int cslLength = c_sFileProtocolPrefix.Length;

				if( csl.Length > cslLength )
				{
					string sl = csl.Substring( 0, cslLength ).ToLower();

					if( sl != c_sFileProtocolPrefix )
					{
						csl = c_sFileProtocolPrefix + csl;
					}
				}

				spidl = NativeMethods.SHSimpleIDListFromPath( Marshal.StringToHGlobalAuto( csl ) );
			}
			else
			{
				NativeMethods.SHGetSpecialFolderLocation(hwnd,(int)this.startLocation,ref spidl);
			}

			if ((option & FolderBrowserStyles.NewDialogStyle) != 0) 
				Application.OleRequired();
			
			IntPtr pidl = IntPtr.Zero;
			try
			{
				NativeMethods.BROWSEINFO browseInfo = new NativeMethods.BROWSEINFO();
				browseInfo.pidlRoot = spidl;
				browseInfo.hwndOwner = hwnd;
				browseInfo.pszDisplayName = Marshal.AllocHGlobal(FolderBrowser.MAX_PATH);
				browseInfo.lpszTitle = this.descriptionText;
				browseInfo.ulFlags = (int)option;
				browseInfo.lpfn = new NativeMethods.BrowserFolderCallback(ValidateProc);
				browseInfo.lParam = IntPtr.Zero;
				browseInfo.iImage = 0;
				pidl = NativeMethods.SHBrowseForFolder(browseInfo);
				if (pidl==IntPtr.Zero) 
				{
					Marshal.FreeHGlobal(browseInfo.pszDisplayName);
					return DialogResult.Cancel;
				}
				IntPtr pszpath = Marshal.AllocHGlobal(FolderBrowser.MAX_PATH);
				if(NativeMethods.SHGetPathFromIDList(pidl, pszpath) == true)
					this.directoryPath = Marshal.PtrToStringAuto(pszpath);
				else
					this.directoryPath = Marshal.PtrToStringAuto(browseInfo.pszDisplayName);
				Marshal.FreeHGlobal(pszpath);
				Marshal.FreeHGlobal(browseInfo.pszDisplayName);
			}  
			finally
			{
				NativeMethods.IMalloc pv = FolderBrowser.GetSHMalloc();
                if (Environment.OSVersion.Version.Major < 6 && System.IntPtr.Size != 8)
                {
                    if (spidl != IntPtr.Zero)
                        pv.Free(spidl);
                    if (pidl != IntPtr.Zero)
                        pv.Free(pidl);
                }
                Marshal.ReleaseComObject(pv);
			}  
			return DialogResult.OK;
		}
    
		#region ICustomTypeDescriptor implementation
		object ICustomTypeDescriptor.GetPropertyOwner( PropertyDescriptor pd )
		{
			return this;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes( this, true );
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName( this, true );
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName( this, true );
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter( this, true );
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent( this, true );
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty( this, true );
		}

		object ICustomTypeDescriptor.GetEditor( Type editorBaseType )
		{
			return TypeDescriptor.GetEditor( this, editorBaseType, true );
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents( Attribute[] attributes )
		{
			return TypeDescriptor.GetEvents( this, attributes, true );
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents( this, true );
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return TypeDescriptor.GetProperties( this, true );
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties( Attribute[] attributes )
		{
			PropertyDescriptorCollection originalProps = TypeDescriptor.GetProperties( this, attributes, true );
			PropertyDescriptorCollection props = originalProps;

			if( null != props && props.Count > 0 )
			{
				PropertyDescriptor pdStartLocation = props[c_sStartLocation];

				if( null != pdStartLocation )
				{
					FolderBrowserFolder fbfLocation = (FolderBrowserFolder)pdStartLocation.GetValue(this);

					props = new PropertyDescriptorCollection(null);

					for( int iProp = 0, nProps = originalProps.Count; iProp < nProps; ++iProp ) 
					{
						PropertyDescriptor prop = originalProps[iProp];

						if( FolderBrowserFolder.CustomStartLocation == fbfLocation || c_sCustomStartLocation != prop.Name )
						{
							props.Add(prop);
						}
					}
				}
			}

			return props;
		}

		#endregion
	}

	/// <summary>
	/// Defines constants used by the <see cref="FolderBrowser"/> component.
	/// </summary>
	/// <remarks>
	/// The FolderBrowserMessage enumeration specifies constants that define the event that 
	/// triggered the <see cref="FolderBrowser.FolderBrowserCallback"/> event to occur.
	/// </remarks>
	public enum FolderBrowserMessage
	{
		/// <summary>
		/// Indicates that the browse dialog box has finished initializing.
		/// </summary>
		Initialized        = 1,
		/// <summary>
		/// Indicates that the selection has changed.
		/// </summary>
		SelChanged         = 2,
		/// <summary>
		/// Indicates that the user typed an invalid name into the edit box.
		/// </summary>
		ValidateFailed     = 4
	}

	/// <summary>
	/// Handles the <see cref="FolderBrowser"/> component's <see cref="FolderBrowser.FolderBrowserCallback"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="FolderBrowserCallbackEventArgs"/> value that contains the event data.</param>
	public delegate void FolderBrowserCallbackEventHandler(object sender, FolderBrowserCallbackEventArgs e);

	
	/// <summary>
	/// Specifies constants that defines the folderbrowser dialog state.
	/// </summary>
	/// <remarks>
	/// Enumeration used for setting the folderbrowser dialog's state from the <see cref="FolderBrowser.FolderBrowserCallback"/> event handler.
	/// For detailed information on the folder browser callback function, refer to the Platform SDK 
	/// documentation on the BrowseCallbackProc function.
	/// </remarks>
	public enum FolderBrowserCallbackSetState
	{
		/// <summary>
		/// Default State.
		/// </summary>
		None = 0,
		/// <summary>
		/// Enables the OK button.
		/// </summary>
		EnableOK = 1,
		/// <summary>
		/// Disables the OK button.
		/// </summary>
		DisableOK,
		/// <summary>
		/// Specifies a path to expand in the Browse dialog box. The path can be set through the 
		/// FolderBrowserCallbackEventArgs.BrowseCallbackText property.
		/// </summary>
		SetExpanded,
		/// <summary>
		/// Sets the text to be displayed on the OK button. The text can be set through the 
		/// FolderBrowserCallbackEventArgs.BrowseCallbackText property.
		/// </summary>
		SetOKText,
		/// <summary>
		/// Selects the specified folder. The folder's path can be set through the 
		/// FolderBrowserCallbackEventArgs.BrowseCallbackText property.
		/// </summary>
		SetSelection,
		/// <summary>
		/// Sets the text to be displayed on the OK button. The text can be set through the 
		/// FolderBrowserCallbackEventArgs.BrowseCallbackText property. This state is inapplicable 
		/// with the new dialog style.
		/// </summary>
		SetStatusText	
	}
    
    /// <summary>
    /// FolderBrowser Designer
    /// </summary>
    public class FolderBrowserDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public FolderBrowserDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new FolderBrowserActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
	/// <summary>
	/// Provides data for the <see cref="FolderBrowser"/> component's FolderBrowserCallback event. 
 	/// </summary>	
	/// <remarks>
	/// The <see cref="FolderBrowser.FolderBrowserCallback"/> event occurs when an event within the folder browser dialog 
	/// triggers a call to the validation callback. Handling this event allows you to implement the 
	/// functional equivalent of the BrowseCallbackProc application-defined callback function. 
	/// The FolderBrowserCallbackEventArgs properties provide information specific to the event.
	/// </remarks>
	public class FolderBrowserCallbackEventArgs : EventArgs
	{
		private NativeWindow window;
		private string path;
		private FolderBrowserMessage browseForFolderMessage;
		private bool dismissDialog = false;

		// Callback dialog state.
		private FolderBrowserCallbackSetState callbackState = FolderBrowserCallbackSetState.None;
		private string callbackText = String.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="FolderBrowserCallbackEventArgs"/> class.
		/// </summary>
		/// <param name="window">A NativeWindow value that represents the window handle of the folderbrowser dialog.</param>
		/// <param name="browseForFolderMessage">A <see cref="FolderBrowserMessage"/> value that identifies the event.</param>
		/// <param name="path">A String value that specifies the valid / invalid folder name.</param>
		public FolderBrowserCallbackEventArgs(NativeWindow window, FolderBrowserMessage browseForFolderMessage, string path)
		{
			this.window = window;
			this.path = path;
			this.browseForFolderMessage = browseForFolderMessage;
		}

		/// <summary>
		/// Returns the window handle of the browse dialog box.
		/// </summary>
		/// <value>A NativeWindow value.</value>
		public NativeWindow Window
		{
			get { return window; }
		}
		
		/// <summary>
		/// Returns the valid / invalid folder name.
		/// </summary>
		/// <value>A String value.</value>
		public string Path
		{
			get { return path; }
		}
		
		/// <summary>
		/// Returns a value identifying the event.
		/// </summary>
		/// <value>A <see cref="FolderBrowserMessage"/> value.</value>
		public FolderBrowserMessage FolderBrowserMessage
		{
			get { return browseForFolderMessage; }
		}
		
		/// <summary>
		/// Indicates whether the dialog is either dismissed or retained depending on this value.
		/// </summary>
		public bool Dismiss
		{
			get { return dismissDialog;	}
			set { dismissDialog = value; }
		}

		/// <summary>
		/// Gets / sets the folder browser dialog's state.
		/// </summary>
		/// <value>A <see cref="FolderBrowserCallbackSetState"/> value.</value>
		public FolderBrowserCallbackSetState FolderBrowserCallbackSetState
		{
			get { return this.callbackState; }
			set { this.callbackState = value; }
		}

		/// <summary>
		/// Gets / sets the contextual string depending upon the value of the <see cref="FolderBrowserCallbackEventArgs.FolderBrowserCallbackSetState"/> property.
		/// </summary>
		/// <value>A String value.</value>
		public string BrowseCallbackText
		{
			get { return this.callbackText; }
			set { this.callbackText = value; }
		}
	}


	/// <summary>
	/// Specifies constants that define the location of the root folder in the folder browser dialog.
	/// </summary>
	/// <remarks>
	/// The FolderBrowserFolder enumeration specifies the location of the root folder from which 
	/// the <see cref="FolderBrowser"/> component will start browsing. Only the specified folder 
	/// and folders beneath it in the namespace hierarchy will appear in the dialog. This 
	/// enumerator provides a simple way to set the ITEMIDLIST structure (PIDL) for the folder 
	/// browser dialog. For more information on the functional significance of these values
	/// please refer to the Platform SDK documentation on the Shell API and the CSIDL Values.
	/// </remarks>
	public enum FolderBrowserFolder 
	{
		/// <summary>
		/// Windows desktop virtual folder that is the root of the name space.
		/// </summary>
		Desktop = 0 /*0x0000*/, 		
		/// <summary>
		/// Virtual folder that represents the Internet.
		/// </summary>
		Internet = 1 /*0x0001*/,		
		/// <summary>
		/// File system directory that contains the user's program groups.
		/// </summary>
		Programs = 2 /*0x0002*/,
		/// <summary>
		/// Virtual folder that contains icons for Control Panel applications. 
		/// </summary>
		Controls = 3 /*0x0003*/,		
		/// <summary>
		/// Virtual folder that contains installed printers. 
		/// </summary>
		Printers = 4 /*0x0004*/, 		
		/// <summary>
		/// File system directory that serves as a common repository for documents.
		/// </summary>
		Personal = 5 /*0x0005*/, 		
		/// <summary>
		/// File system directory that serves as a common repository for the user's favorite items.
		/// </summary>
		Favorites = 6 /*0x0006*/,
		/// <summary>
		/// File system directory that corresponds to the user's Startup program group.
		/// </summary>
		Startup = 7 /*0x0007*/,		
		/// <summary>
		/// File system directory that contains the user's most recently used documents.
		/// </summary>
		Recent = 8 /*0x0008*/, 		
		/// <summary>
		/// File system directory that contains Send To menu items.
		/// </summary>
		SendTo = 9 /*0x0009*/,
		/// <summary>
		/// Virtual folder that contains the objects in the user's Recycle Bin.
		/// </summary>
		BitBucket = 10 /*0x000A*/,		
		/// <summary>
		/// File system directory that contains Start Menu items.
		/// </summary>
		StartMenu = 11 /*0x000B*/, 		
		/// <summary>
		/// Virtual folder that contains the objects in the user's My Documents folder.
		/// </summary>
		MyDocuments = 12 /*0x000C*/,
		/// <summary>
		/// File system directory that serves as a common repository for music files.
		/// </summary>
		MyMusic = 13 /*0x000D*/,
		/// <summary>
		/// File system directory that serves as a common repository for video files.
		/// </summary>
		MyVideo = 14 /*0x000E*/,
		/// <summary>
		/// File system directory used to physically store file objects on the desktop.
		/// </summary>
		DesktopDirectory = 16 /*0x0010*/,		
		/// <summary>
		/// My Computer virtual folder that contains everything on the local computer: storage devices, printers and Control Panel.
		/// </summary>
		MyComputer = 17 /*0x0011*/, 		
		/// <summary>
		/// Network Neighborhood virtual folder that represents the root of the network namespace hierarchy. 
		/// </summary>
		NetworkNeighborhood = 18 /*0x0012*/, 		
		/// <summary>
		/// A file system folder that contains the link objects that can exist in the My Network Places virtual folder. 
		/// </summary>
		NetHood = 19 /*0x0013*/,
		/// <summary>
		/// Virtual folder that contains fonts. 
		/// </summary>
		Fonts = 20 /*0x0014*/,		
		/// <summary>
		/// File system directory that serves as a common repository for document templates. 
		/// </summary>
		Templates = 21 /*0x0015*/, 		
		/// <summary>
		/// My Pictures folder.
		/// </summary>
		MyPictures = 39 /*0x0027*/, 		
        /// <summary>
        /// Program files folder
        /// </summary>
        ProgramFiles = 42 /*0x002a*/,
		/// <summary>
		/// File system directory that contains documents that are common to all users.
		/// </summary>
		CommonDocuments = 46 /*0x002E*/,
		/// <summary>
		/// File system directory that contains administrative tools for all users.
		/// </summary>
		CommonAdminTools = 47 /*0x002F*/,
		/// <summary>
		///  File system directory used to store administrative tools for an individual user.
		/// </summary>
		AdminTools = 48 /*0x0030*/,				
		/// <summary>
		/// Virtual folder that contains network and dial-up connections.
		/// </summary>
		NetAndDialUpConnections = 49 /*0x0031*/, 
		/// <summary>
		/// My Music folder for all users.
		/// </summary>
		CommonMusic = 53 /*0x0035*/,
		/// <summary>
		/// My Pictures folder for all users.
		/// </summary>
		CommonPictures = 54 /*0x0036*/,
		/// <summary>
		/// My Video folder for all users.
		/// </summary>
		CommonVideo = 55 /*0x0037*/,
		/// <summary>
		/// System resource directory.
		/// </summary>
		Resources = 56 /*0x0038*/,
		/// <summary>
		/// Localized resource directory.
		/// </summary>
		ResourcesLocalized = 57 /*0x0039*/,
		/// <summary>
		/// Folder containing links to OEM specific applications for all users.
		/// </summary>
		CommonOemLinks = 58 /*0x003A*/,
		/// <summary>
		/// File system folder used to hold data for burning to a CD.
		/// </summary>
		CDBurnArea = 59 /*0x003B*/,
		/// <summary>
		/// Computers Near Me folder.
		/// </summary>
		ComputersNearMe = 60 /*0x003D*/,
		/// <summary>
		/// Use custom start folder (<see cref="FolderBrowser.CustomStartLocation"/>).
		/// </summary>
		CustomStartLocation = 0x00FF,
		/// <summary>
		/// Combine this flag with the desired CSIDL_ value to indicate per-user initialization. 
		/// </summary>
		FlagPerUserInit = 2048 /*0x0800*/,
		/// <summary>
		/// Combine this flag with the desired CSIDL_ value to force a non-alias version of the PIDL. 
		/// </summary>
		FlagNoAlias = 4096 /*0x1000*/,
		/// <summary>
		/// Combine this flag with the desired CSIDL_ value to return an unverified folder path. 
		/// </summary>
		FlagDontVerify = 16384 /*0x4000*/,
		/// <summary>
		/// Combine this flag with the desired CSIDL_ value to force the creation of the associated folder. 
		/// </summary>
		FlagCreate = 32768 /*0x8000*/,
		/// <summary>
		/// Mask for all possible CSIDL flag values.
		/// </summary>
		FlagMask = 65280 /*0xFF00*/
	}

	
	/// <summary>
	/// Specifies constants that define the styles for the folder browser dialog.
	/// </summary>
	/// <remarks>
	/// The <see cref="FolderBrowserStyles"/> enumeration specifies the options for the folder browser dialog. 
	/// For more detailed information, refer to the Platform SDK documentation on the 
	/// Win32 BROWSEINFO structure.
	/// <para>This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.</para>
	/// </remarks>
	[
	Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(System.Drawing.Design.UITypeEditor)),
	Serializable, 
	Flags()
	]
	public enum FolderBrowserStyles 
	{
		/// <summary>
		/// Restricts selection to file system directories.
		/// </summary>
		RestrictToFilesystem = 1 /*0x0001*/, 
		/// <summary>
		/// Excludes network folders below the domain level.
		/// </summary>
		RestrictToDomain = 2 /*0x0002*/,
		/// <summary>
		/// Includes a status area in the dialog box. The status text can be specified in the FolderBrowserCallback event handler.
		/// This style does not apply to the new style dialog.
		/// </summary>
		StatusText = 4	/*0x0004*/,
		/// <summary>
		/// Returns only file system ancestors.
		/// </summary>
		RestrictToSubfolders = 8 /*0x0008*/, 
		/// <summary>
		/// Displays a textbox control in the folder browser dialog.
		/// </summary>
		ShowTextBox = 16 /*0x0010*/,
		/// <summary>
		/// Typing an invalid name in the textbox will trigger the FolderBrowser's FolderBrowserCallback event.
		/// </summary>
		Validate = 32 /*0x0020*/, 
		/// <summary>
		/// Uses the new resizable folder selection dialog.
		/// </summary>
		NewDialogStyle = 64 /*0x0040*/, 
		/// <summary>
		/// Adds a usage hint to the folder dialog. Valid only with the NewDialogStyle flag.
		/// </summary>
		UAHint = 256 /*0x0100*/,
		/// <summary>
		/// Displays URLs. The NewDialogStyle and BrowseForEverything values must be set along 
		/// with this flag. 
		/// </summary>
		AllowUrls = 128 /*0x0080*/, 
		/// <summary>
		/// Displays only computers.
		/// </summary>
		BrowseForComputer = 4096 /*0x1000*/, 
		/// <summary>
		/// Displays only printers.
		/// </summary>
		BrowseForPrinter = 8192 /*0x2000*/, 
		/// <summary>
		/// Displays files as well as folders.
		/// </summary>
		BrowseForEverything = 16384 /*0x4000*/, 
		/// <summary>
		/// Displays shareable resources existing on remote systems.
		/// </summary>
		[Obsolete("Please, use ShowAdministrativeShares style instead.")]
		ShowShares = 32768 /*0x8000*/,
		/// <summary>
		/// Displays administrative shares existing on remote systems.
		/// </summary>
		ShowAdministrativeShares = 0x8000,
	}

} // end of namespace
