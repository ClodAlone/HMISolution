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
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Scripting.Design
{
  /// <summary>
  /// Interface to script editors.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This interface is implemented by classes that edit script objects.
  /// </para>
  /// </remarks>
  public interface IScriptEditor
  {
    /// <summary>
    /// Loads the data from the script into the editor.
    /// </summary>
    /// <param name="script">Script to load into the editor</param>
    void UpdateEditor( Script script );

    /// <summary>
    /// Updates the script object with the current values in the editor.
    /// </summary>
    /// <param name="script">Script to update</param>
    void UpdateScript( Script script );

    /// <summary>
    /// The Form object that implements the user interface.
    /// </summary>
    Form Form { get; }
  }

  /// <summary>
  /// Implements a UITypeEditor for editing Script objects.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class is the design-time editor for the Script property of
  /// the <see cref="Syncfusion.Scripting.Design.ScriptingManager"/>
  /// class.
  /// </para>
  /// </remarks>
  public class ScriptUITypeEditor : UITypeEditor
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ScriptUITypeEditor()
    {
    }

    /// <summary>
    /// Returns the editor style.
    /// </summary>
    /// <param name="context">Designer host context</param>
    /// <returns>Always returns UITypeEditorEditStyle.Modal as the editor style</returns>
    public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
    {
      return UITypeEditorEditStyle.Modal;
    }

    /// <summary>
    /// Called by the designer to edit a script.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="provider"></param>
    /// <param name="value"></param>
    /// <returns>Returns the updated value of the Script</returns>
    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
    {
      Script script = value as Script;
      IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) );

      if( script != null && edSvc != null )
      {
        IScriptEditor scriptEditor = new ScriptEditForm();
        ScriptingManager manager = context.Instance as ScriptingManager;

        if( manager != null )
        {
          ( scriptEditor as ScriptEditForm ).SetManager( manager );
        }

        scriptEditor.UpdateEditor( script );

        if( edSvc.ShowDialog( scriptEditor.Form ) == DialogResult.OK )
        {
          scriptEditor.UpdateScript( script );
        }
      }

      return value;
    }

    /// <summary>
    /// Determines if the editor manually draws the value in the property grid.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Always returns false</returns>
    public override bool GetPaintValueSupported( ITypeDescriptorContext context )
    {
      return false;
    }

    /// <summary>
    /// Handles painting the value for the editor.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// <para>
    /// Does nothing except call the base class PaintValue method.
    /// </para>
    /// </remarks>
    public override void PaintValue( PaintValueEventArgs e )
    {
      base.PaintValue( e );
    }
  }
}