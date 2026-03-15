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
using System.Xml;
using Syncfusion.XlsIO.Implementation.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  class VmlFormControlsSerializator : ShapeSerializator
  {
    #region Members
    /// <summary>
    /// Contains dictionary with supported serializator types.
    /// </summary>
    private Dictionary<Type, ShapeSerializator> m_dictShapeSerializators = new Dictionary<Type, ShapeSerializator>();
    #endregion

    #region Methods
    public VmlFormControlsSerializator()
    {
      m_dictShapeSerializators.Add( typeof( CheckBoxShapeImpl ), new CheckBoxShapeSerializator() );
      m_dictShapeSerializators.Add( typeof( ComboBoxShapeImpl ), new ComboBoxShapeSerializator() );
	  m_dictShapeSerializators.Add( typeof( OptionButtonShapeImpl ), new OptionButtonShapeSerializator() );
    }
    internal void ClearAll()
    {
        m_dictShapeSerializators.Clear();
        m_dictShapeSerializators = null;
    }
    public override void Serialize( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection vmlRelations )
    {
      ShapeSerializator serializator;

      if( !m_dictShapeSerializators.TryGetValue( shape.GetType(), out serializator ) )
      {
        serializator = new UnknownShapeSerializator( null );
      }

      serializator.Serialize( writer, shape, holder, vmlRelations );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public override void SerializeShapeType( XmlWriter writer, Type shapeType )
    {
      ShapeSerializator serializator;

      if( !m_dictShapeSerializators.TryGetValue( shapeType, out serializator ) )
      {
        serializator = new UnknownShapeSerializator( null );
      }
       if(shapeType .Name != "ComboBoxShapeImpl") 
      serializator.SerializeShapeType( writer, shapeType );
    }
    #endregion
  }
}
