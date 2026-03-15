#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using System.Windows.Data;
using Syncfusion.Windows.Chart;
using Microsoft.Windows.Design;

namespace Syncfusion.Chart.Wpf.VisualStudio.Design
{
  public class ForbiddenPropertyEditor : PropertyValueEditor
  {
    #region Members
    private EditorResources editorResources = new EditorResources();
    #endregion

    #region Constructor
    public ForbiddenPropertyEditor()
    {
      InlineEditorTemplate = (DataTemplate)editorResources["ForbiddenPopertyEditor"];
    }
    #endregion
  }



}
