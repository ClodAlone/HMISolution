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

using System.Collections;
using System;
using System.Drawing;

using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Formatting
{
	/// <summary>
	/// Summary description for DynamicFormatManager.
	/// </summary>
	public class DynamicFormatManager
	{
		#region Class Members
		/// <summary>
		/// Hashtable with layers.
		/// Keys - names of the layers, Values - Layers.
		/// If some layer is added to this hashtable, it must be added to the m_layersList.
		/// </summary>
		private Hashtable m_layers = new Hashtable();
		/// <summary>
		/// List of the layers, that stores all layers and preserves their order.
		/// This list should contains all layers that are present in m_layers hashtable.
		/// </summary>
		private ArrayList m_layersList = new ArrayList();
		/// <summary>
		/// Temporary layer used to store all formats merged into one layer.
		/// </summary>
		private DynamicFormatLayer m_tempLayer;
		/// <summary>
		/// List of layers that should be disposed.
		/// They are disposed in UpdateFormats method -
		/// they can't be disposed in the point of adding to list because ParsePointManager is locked then.
		/// </summary>
		private IList m_arrLayersToDispose = new ArrayList();
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets list of dynamic formatting, that intersepts with given range.
		/// </summary>
		public IList this[ CoordinatePoint start, CoordinatePoint end ]
		{
			get
			{
				return TempLayer[ start, end ];
			}
		}
		/// <summary>
		/// Gets collection of layers.
		/// </summary>
		public ICollection Layers
		{
			get
			{
				return m_layersList;
			}
		}

		/// <summary>
		/// Gets collection of layer names.
		/// </summary>
		public ICollection Names
		{
			get
			{
				return m_layers.Keys;
			}
		}

		/// <summary>
		/// Gets registered dynamic formatting layer by name.
		/// </summary>
		public IDynamicFormatsLayer this[ string name ]
		{
			get
			{
				return ( IDynamicFormatsLayer )m_layers[ name ];
			}
		}
		#endregion

		#region Class Private Properties
		/// <summary>
		/// Gets temporary common layer. Creates it if it doesn't exist.
		/// </summary>
		private DynamicFormatLayer TempLayer
		{
			get
			{
				if( m_tempLayer == null )
				{
					m_tempLayer = new DynamicFormatLayer();

					foreach( IDynamicFormatsLayer layer in m_layersList )
					{
						if( !layer.Hidden )
						{
							for( int iFormat = 0; iFormat < layer.Count; iFormat++ )
							{
								IDynamicFormat format = layer[ iFormat ] as IDynamicFormat;

								MergeInFormat( layer, format );
							}
						}
					}
				}

				return m_tempLayer;
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Creates new layer and adds it to the list.
		/// </summary>
		/// <param name="name">Name of the layer.</param>
		/// <returns>Newly created layer.</returns>
		public IDynamicFormatsLayer RegisterLayer( string name )
		{
			return RegisterLayer( name, false );
		}
		/// <summary>
		/// Creates new layer and adds it to the list.
		/// </summary>
		/// <param name="name">Name of the layer.</param>
		/// <param name="hidden">Specifies whether this layer is hidden.</param>
		/// <returns>Newly created layer.</returns>
		public IDynamicFormatsLayer RegisterLayer( string name, bool hidden )
		{
			DynamicFormatLayer layer = new DynamicFormatLayer( hidden );
			layer.DataChanged += new EventHandler( layer_DataChanged );
			m_layers.Add( name, layer );
			m_layersList.Add( layer );

			return layer;
		}
		/// <summary>
		/// Gets list of dynamic formats that finish at specified point.
		/// </summary>
		/// <param name="endPoint">Specified finish point.</param>
		/// <returns>List of dynamic formats that finish at specified point.</returns>
		public IList GetFormatsWithEndAt( CoordinatePoint endPoint )
		{
			if( null == endPoint )
				return null;

			IList result = new ArrayList( 2 );

			foreach( IDynamicFormatsLayer layer in m_layersList )
			{
				for( int iFormat = 0; iFormat < layer.Count; iFormat++ )
				{
					IDynamicFormat format = layer[ iFormat ] as IDynamicFormat;

					if( endPoint == format.End ) result.Add( format );
				}
			}

			return result;
		}
		/// <summary>
		/// Updates state of start and end points of each dynamic formatting (if needed) and disposes deleted layers.
		/// </summary>
		public void UpdateFormats()
		{
			for( int i = 0, len = m_layersList.Count; i < len; i++ )
			{
				DynamicFormatLayer layer = ( DynamicFormatLayer )m_layersList[ i ];
				layer.UpdateFormats();
			}

			for( int i = 0, len = m_arrLayersToDispose.Count; i < len; i++ )
			{
				DynamicFormatLayer layer = ( DynamicFormatLayer )m_arrLayersToDispose[ i ];
				layer.Dispose();
			}

			m_arrLayersToDispose.Clear();
		}
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Merges two formats into the new one.
		/// </summary>
		/// <param name="formatOld">Basic format.</param>
		/// <param name="formatNew">Formats that overrides basic format.</param>
		/// <returns>New format instance that contains baisc format settings
		/// overriden with settings from the new format.</returns>
		protected ISnippetFormat MergeFormats( ISnippetFormat formatOld, ISnippetFormat formatNew )
		{
			Format formatResult = new Format();
			formatResult.Parent = ( ( Format )formatOld ).Parent;
			formatResult.BackColor = ( formatNew.BackColor != Color.Empty )
				? formatNew.BackColor : formatOld.BackColor;

			if( formatNew.FontColor != Color.Empty )
			{
				formatResult.FontColor = formatNew.FontColor;
				formatResult.FontBold = formatNew.Font.Bold;
				formatResult.FontFamily = formatNew.Font.FontFamily;
				formatResult.FontItalic = formatNew.Font.Italic;
				formatResult.FontSize = formatNew.Font.Size;
			}
			else
			{
				formatResult.FontColor = formatOld.FontColor;
				formatResult.FontBold = formatOld.Font.Bold;
				formatResult.FontFamily = formatOld.Font.FontFamily;
				formatResult.FontItalic = formatOld.Font.Italic;
				formatResult.FontSize = formatOld.Font.Size;
			}

			if( FrameBorderStyle.None != formatNew.BorderStyle )
			{
				formatResult.BorderStyle = formatNew.BorderStyle;
				formatResult.BorderColor = formatNew.BorderColor;
				formatResult.BorderWeight = formatNew.BorderWeight;
			}
			else
			{
				formatResult.BorderStyle = formatOld.BorderStyle;
				formatResult.BorderColor = formatOld.BorderColor;
				formatResult.BorderWeight = formatOld.BorderWeight;
			}

			formatResult.HatchStyle = ( formatNew.UseHatchFill )
				? formatNew.HatchStyle : formatOld.HatchStyle;

			formatResult.UseHatchFill = ( formatNew.UseHatchFill )
				? true : formatOld.UseHatchFill;

			formatResult.LineColor =
				( formatNew.UnderlineStyle != UnderlineStyle.None && formatNew.LineColor != Color.Empty )
				? formatNew.LineColor
				: formatOld.LineColor;

			formatResult.UnderlineStyle = ( formatNew.UnderlineStyle != UnderlineStyle.None )
				? formatNew.UnderlineStyle : formatOld.UnderlineStyle;

			formatResult.UnderlineWeight = ( formatNew.UnderlineStyle != UnderlineStyle.None )
				? formatNew.UnderlineWeight : formatOld.UnderlineWeight;

			formatResult.UseCustomControl = formatOld.UseCustomControl;

			return formatResult;
		}
		/// <summary>
		/// Merges dynamic format into the specified dynamic formats layer.
		/// </summary>
		/// <param name="layer">Layer the specified dynamic format should be merged into.</param>
		/// <param name="formatToMergeIn">Dynamic format to be merged in.</param>
		protected void MergeInFormat( IDynamicFormatsLayer layer, IDynamicFormat formatToMergeIn )
		{
			if( null == layer )
				throw new ArgumentNullException( "layer" );

			if( null == formatToMergeIn )
				throw new ArgumentNullException( "formatToMergeIn" );

			CoordinatePoint pointMergeInFormatEnd = formatToMergeIn.End;
			CoordinatePoint pointMergeInFormatStart = formatToMergeIn.Start;
			IList list = m_tempLayer[ pointMergeInFormatStart, pointMergeInFormatEnd ];
			CoordinatePoint pointLastStart = pointMergeInFormatStart;

			for( int i = 0, len = list.Count; i < len; i++ )
			{
				IDynamicFormat format = ( IDynamicFormat )list[ i ];
				ISnippetFormat formatOld = format.Format;
				ISnippetFormat formatNew = MergeFormats( formatOld, formatToMergeIn.Format );
				CoordinatePoint pointStart =
					( format.Start < pointMergeInFormatStart ) ? pointMergeInFormatStart : format.Start;
				CoordinatePoint pointEnd = ( format.End > pointMergeInFormatEnd ) ? pointMergeInFormatEnd : format.End;

				if( pointLastStart != pointStart )
					m_tempLayer.Add( pointLastStart, pointStart, formatToMergeIn.Format );

				m_tempLayer.Add( pointStart, pointEnd, formatNew );
				pointLastStart = pointEnd;
			}

			if( pointLastStart != pointMergeInFormatEnd )
				m_tempLayer.Add( pointLastStart, pointMergeInFormatEnd, formatToMergeIn.Format );
		}
		#endregion

		#region Class Event Handlers
		/// <summary>
		/// Resets temporary layer.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		public void layer_DataChanged( object sender, EventArgs e )
		{
			if( m_tempLayer != null )
			{
				m_arrLayersToDispose.Add( m_tempLayer );
				m_tempLayer = null;
			}
		}
		#endregion
	}
}