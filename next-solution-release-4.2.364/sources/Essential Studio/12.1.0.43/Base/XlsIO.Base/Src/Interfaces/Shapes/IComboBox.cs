#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// This interface provides access to the combo box shape properties.
  /// </summary>
  public interface IComboBoxShape : IShape
  {
    #region Properites
    /// <summary>
    /// Gets or sets the worksheet range used to fill the specified list box.
    /// </summary>
    IRange ListFillRange
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets the worksheet range linked to the control's value.
    /// </summary>
    IRange LinkedCell
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets selected item index of the combo box.
    /// </summary>
    int SelectedIndex
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets the number of list lines displayed in the drop-down portion of a combo box.
    /// </summary>
    int DropDownLines
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or sets value indicating whether 3D shadow is present.
    /// </summary>
    bool Display3DShading
    {
      get;
      set;
    }
    /// <summary>
    /// Gets value selected in combobox.
    /// </summary>
    string SelectedValue
    {
      get;
    }
    #endregion
  }
}
