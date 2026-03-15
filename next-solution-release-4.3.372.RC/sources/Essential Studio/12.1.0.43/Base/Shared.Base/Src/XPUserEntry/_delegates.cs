#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
  
  public class ValueChangedEventArgs : EventArgs
  {
    #region ValueChangedEventArgs Class members
  
		private object m_old;
    private object m_new;

    #endregion

    #region ValueChangedEventArgs Class properties
    
    public object newValue
    {
      get
      {
        return m_new;
      }
    }
    
    public object oldValue
    {
      get
      {
        return m_old;
      }
    }
    #endregion

    #region ValueChangedEventArgs Class initialize/finalize methods
    
    private ValueChangedEventArgs()
    {
    }
    
    public  ValueChangedEventArgs( object old, object newValue )
    {
      m_old = old;
      m_new = newValue;
    }
    #endregion
  }

  
  public delegate void ValueChangedEventHandler( object sender, ValueChangedEventArgs e );

	
	public class PasswordEnterEventArgs : CancelEventArgs
	{
		private string m_username;
		private string m_password;

		public string UserName
		{
			get
			{
				return m_username;
			}
		}

		public string Password
		{
			get
			{
				return m_password;
			}
		}

		public PasswordEnterEventArgs( string username, string password )
		{
			m_username = username;
			m_password = password;
		}
	}
	
	public delegate void PasswordEnterEventHandler( object sender, PasswordEnterEventArgs e );
}