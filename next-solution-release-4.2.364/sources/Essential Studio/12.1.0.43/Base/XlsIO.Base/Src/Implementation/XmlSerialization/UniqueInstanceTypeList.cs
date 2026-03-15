#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Implementation.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// This class contains unique pair of shape instance and shape type.
  /// </summary>
  class UniqueInstanceTypeList
  {
    private Dictionary<int, Dictionary<Type, object>> m_dictItems = new Dictionary<int,Dictionary<Type,object>>();

    public void AddShape( ShapeImpl shape )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      int instance = shape.Instance;
      Dictionary<Type, object> dictTypes;

      if( !m_dictItems.TryGetValue( instance, out dictTypes ) )
      {
        dictTypes = new Dictionary<Type, object>();
        m_dictItems[ instance ] = dictTypes;
      }

      dictTypes[ shape.GetType() ] = null;
    }
    public IEnumerable UniquePairs()
    {
      foreach( int instance in m_dictItems.Keys )
      {
        Dictionary<Type, object> dictTypes = m_dictItems[ instance ];

        foreach( Type shapeType in dictTypes.Keys )
        {
          yield return new KeyValuePair<int, Type>( instance, shapeType );
        }
      }
    }
  }
}
