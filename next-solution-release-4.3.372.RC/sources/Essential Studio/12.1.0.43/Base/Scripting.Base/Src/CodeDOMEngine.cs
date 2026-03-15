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

using Microsoft.Vsa;

namespace Syncfusion.Scripting
{
	/// <summary>
	/// Base class implementation IVsaEngine using the CodeDOM.
	/// </summary>
	public class CodeDOMEngine : Microsoft.Vsa.IVsaEngine
	{
		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public CodeDOMEngine()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region IVsaEngine Members

		public virtual bool GenerateDebugInfo
		{
			get
			{
				// TODO:  Add CodeDOMEngine.GenerateDebugInfo getter implementation
				return false;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.GenerateDebugInfo setter implementation
			}
		}

		public virtual string RootNamespace
		{
			get
			{
				// TODO:  Add CodeDOMEngine.RootNamespace getter implementation
				return null;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.RootNamespace setter implementation
			}
		}

		public virtual bool IsCompiled
		{
			get
			{
				// TODO:  Add CodeDOMEngine.IsCompiled getter implementation
				return false;
			}
		}

		public virtual void Reset()
		{
			// TODO:  Add CodeDOMEngine.Reset implementation
		}

		public virtual void SaveSourceState(IVsaPersistSite site)
		{
			// TODO:  Add CodeDOMEngine.SaveSourceState implementation
		}

		public virtual void SaveCompiledState(out byte[] pe, out byte[] pdb)
		{
			// TODO:  Add CodeDOMEngine.SaveCompiledState implementation
			pe = null;
			pdb = null;
		}

		public virtual object GetOption(string name)
		{
			// TODO:  Add CodeDOMEngine.GetOption implementation
			return null;
		}

		public virtual void InitNew()
		{
			// TODO:  Add CodeDOMEngine.InitNew implementation
		}

		public virtual string RootMoniker
		{
			get
			{
				// TODO:  Add CodeDOMEngine.RootMoniker getter implementation
				return null;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.RootMoniker setter implementation
			}
		}

		public virtual int LCID
		{
			get
			{
				// TODO:  Add CodeDOMEngine.LCID getter implementation
				return 0;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.LCID setter implementation
			}
		}

		public virtual System.Reflection.Assembly Assembly
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Assembly getter implementation
				return null;
			}
		}

		public virtual void RevokeCache()
		{
			// TODO:  Add CodeDOMEngine.RevokeCache implementation
		}

		public virtual void Close()
		{
			// TODO:  Add CodeDOMEngine.Close implementation
		}

		public virtual bool Compile()
		{
			// TODO:  Add CodeDOMEngine.Compile implementation
			return false;
		}

		public virtual IVsaItems Items
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Items getter implementation
				return null;
			}
		}

		public virtual System.Security.Policy.Evidence Evidence
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Evidence getter implementation
				return null;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.Evidence setter implementation
			}
		}

		public virtual string Name
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Name getter implementation
				return null;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.Name setter implementation
			}
		}

		public virtual void LoadSourceState(IVsaPersistSite site)
		{
			// TODO:  Add CodeDOMEngine.LoadSourceState implementation
		}

		public virtual void Run()
		{
			// TODO:  Add CodeDOMEngine.Run implementation
		}

		public virtual string Version
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Version getter implementation
				return null;
			}
		}

		public virtual bool IsRunning
		{
			get
			{
				// TODO:  Add CodeDOMEngine.IsRunning getter implementation
				return false;
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				// TODO:  Add CodeDOMEngine.IsDirty getter implementation
				return false;
			}
		}

		public virtual bool IsValidIdentifier(string identifier)
		{
			// TODO:  Add CodeDOMEngine.IsValidIdentifier implementation
			return false;
		}

		public virtual string Language
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Language getter implementation
				return null;
			}
		}

		public virtual void SetOption(string name, object value)
		{
			// TODO:  Add CodeDOMEngine.SetOption implementation
		}

		public virtual IVsaSite Site
		{
			get
			{
				// TODO:  Add CodeDOMEngine.Site getter implementation
				return null;
			}
			set
			{
				// TODO:  Add CodeDOMEngine.Site setter implementation
			}
		}

		#endregion
	}
}
