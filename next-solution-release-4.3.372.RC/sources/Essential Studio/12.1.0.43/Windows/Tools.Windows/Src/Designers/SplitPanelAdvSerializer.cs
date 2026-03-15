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

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

#region file using directives
using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using Syncfusion.Windows.Forms.Tools;
#endregion

namespace Syncfusion.Windows.Forms.Design.Serialization

{
	/// <summary>
	/// Implements split panel designer code generation.
	/// In fact, this class is fix to VS2003 CodeDom serialziation
	/// for nested controls.
	/// </summary>
	public class SplitContainerAdvSerializer: CodeDomSerializer
	{
		#region Class constants
		/// <summary>
		/// First split panel property name in container.
		/// </summary>
		protected static internal readonly string DEF_PANEL1_NAME = "Panel1";
		/// <summary>
		/// Second split panel property name in container.
		/// </summary>
		protected internal readonly string DEF_PANEL2_NAME = "Panel2";
		#endregion

		#region Class overrides
		public override object Serialize( IDesignerSerializationManager manager, object value )
		{
		  //MyTracer.WriteLine( ( ( SplitContainerAdv )value ).BackgroundColor.BackColor.ToString( ), "SerBackColorAfter" );     
			CodeDomSerializer serializer = ( CodeDomSerializer )manager.GetSerializer( typeof( System.Windows.Forms.Control ), typeof( CodeDomSerializer ) );
			
			CodeStatementCollection statements = null;

			// clear all collection because of VS2003 design-code generation for 
			// container & nested conrols in it is redundant
			
			statements = serializer.Serialize( manager, value ) as CodeStatementCollection;
//			CodeStatement statement = null;
//
//			if( statements != null && statements.Count > 0 )
//			{
//				// search for Add methods and remove them
//				for( int i = 0, len = statements.Count; i < len; i++ )
//				{
//					statement = statements[ i ];
//
//					bool bIsValidStatement = ( IsValidExpressionStatement( statement ) || 
//						IsValidCodeAssignStatement( statement ) || 
//						IsValidAttachEventStatement( statement ) );
//
//					if( !bIsValidStatement )
//					{
//						statements.RemoveAt( i );
//						i--;
//						len--;
//					}
//					
//				}
//			}			
			return statements;

		}
        
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer serializer = ( CodeDomSerializer  )manager.GetSerializer( typeof( System.Windows.Forms.Control ), typeof( CodeDomSerializer ) );
			object res = serializer.Deserialize( manager, codeObject );
		  
		  /*
		  if( res is SplitContainerAdv )
		  {
		    SplitContainerAdv adv = ( SplitContainerAdv )res;
		    //MyTracer.WriteLine( adv.BackgroundColor.BackColor.ToString( ), "DES_BC" );
		  }
		  */
		  
		  return res;
		}
		#endregion

		// these functions are used to eliminate redundant code statemens
		// VS2003 designer is generating.
		#region Code Statement Validation
		private bool IsValidExpressionStatement( CodeStatement statement )
		{
			if( statement == null )
				throw new ArgumentNullException( "statement" );

			bool bIsValid = false;

			CodeExpressionStatement expressionStatement = statement as CodeExpressionStatement;
			if( expressionStatement != null )
			{
				CodeMethodInvokeExpression methodExpression = expressionStatement.Expression as CodeMethodInvokeExpression;
				if( methodExpression != null )
				{
					bIsValid = IsValidMethodInvokeExpression( methodExpression );
				}
			}

			return bIsValid;
		}

		private bool IsValidMethodInvokeExpression( CodeExpression expression )
		{
			if( expression == null )
				throw new ArgumentNullException( "expression" );

			bool bIsValid = false;

			CodeMethodInvokeExpression methodExpression = expression as CodeMethodInvokeExpression;
			if( methodExpression != null )
			{
				CodePropertyReferenceExpression parentProperty = methodExpression.Method.TargetObject as CodePropertyReferenceExpression;
				if( parentProperty != null )
				{
					bIsValid = IsValidPanelProperty( parentProperty );
				}
			}
			return bIsValid;
		}

		private bool IsValidAttachEventStatement( CodeStatement statement )
		{
			if( statement == null )
				throw new ArgumentNullException( "statement" );
            
			bool bIsValid = false;

			CodeAttachEventStatement eventStatement = statement as CodeAttachEventStatement;
			if( eventStatement != null )
			{
				CodePropertyReferenceExpression parentProperty = eventStatement.Event.TargetObject as CodePropertyReferenceExpression;
				if( parentProperty != null )
				{
					bIsValid = IsValidPanelProperty( parentProperty );
				}
			}
            
			return bIsValid;
		}

		private bool IsValidCodeAssignStatement( CodeStatement statement )
		{
			if( statement == null )
				throw new ArgumentNullException( "statement" );

			bool bIsValid = false;
            
			CodeAssignStatement assignStatement = statement as CodeAssignStatement;
			if( assignStatement != null )
			{
				CodePropertyReferenceExpression property = assignStatement.Left as CodePropertyReferenceExpression;
				bIsValid = IsValidPanelProperty( property );
			}

			return bIsValid;
		}

		private bool IsValidPanelProperty( CodePropertyReferenceExpression property )
		{
			if( property == null )
				throw new ArgumentNullException( "property" );

			CodePropertyReferenceExpression panelProperty = null;

			bool bIsValid = ( IsPanelProperty( property.PropertyName ) && 
				( property.TargetObject as CodeFieldReferenceExpression != null ) );

			if( bIsValid )
			{
				panelProperty = property;
			}
			else
			{
				CodePropertyReferenceExpression parentProperty = 
					property.TargetObject as CodePropertyReferenceExpression;

				if( parentProperty != null )
				{
					bIsValid = IsValidPanelProperty( parentProperty );
				}
			}

			return bIsValid;
		}
        
		/// <summary>
		/// Check, if specified property name is split panel property name
		/// </summary>
		private bool IsPanelProperty( string propertyName )
		{
			if( propertyName == null )
				throw new ArgumentNullException( "propertyName" );

			bool bIsPanel = ( string.Compare( propertyName, DEF_PANEL1_NAME, true ) == 0 ||
				string.Compare( propertyName, DEF_PANEL2_NAME, true ) == 0 );

			return bIsPanel;
		}

		#endregion
	}

	/// <summary>
	/// Implements split panel designer code generation.
	/// In fact, this class is fix to VS2003 CodeDom serialziation
	/// for nested controls.
	/// </summary>
	public class SplitPanelAdvSerializer: CodeDomSerializer
	{
		#region Class constants
		/// <summary>
		/// First split panel property name in container.
		/// </summary>
		protected static internal readonly string DEF_PANEL1_NAME = "Panel1";
		/// <summary>
		/// Sacond split panel property name in container.
		/// </summary>
		protected internal readonly string DEF_PANEL2_NAME = "Panel2";
		#endregion

		#region Class overrides
		public override object Serialize( IDesignerSerializationManager manager, object value )
		{
			CodeDomSerializer serializer = ( CodeDomSerializer )manager.GetSerializer( this.GetType().BaseType, typeof( CodeDomSerializer ) );
			
			CodeStatementCollection statements = null;

			// clear all collection because of VS2003 design-code generation for 
			// container & nested conrols in it is redundant
			
			statements = serializer.Serialize( manager, value ) as CodeStatementCollection;
			CodeStatement statement = null;

			if( statements != null && statements.Count > 0 )
			{
				// search for Add methods and remove them
				for( int i = 0, len = statements.Count; i < len; i++ )
				{
					statement = statements[ i ];

					bool bIsValidStatement = ( IsValidExpressionStatement( statement ) || 
						IsValidCodeAssignStatement( statement ) || 
						IsValidAttachEventStatement( statement ) );

					if( !bIsValidStatement )
					{
						statements.RemoveAt( i );
						i--;
						len--;
					}
					
				}
			}			

			return statements;

		}
        
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer serializer = ( CodeDomSerializer  )manager.GetSerializer( this.GetType().BaseType, typeof( CodeDomSerializer ) );
			return serializer.Deserialize( manager, codeObject );
		}
		#endregion

		// these functions are used to eliminate redundant code statemens
		// VS2003 designer is generating.
		#region Code Statement Validation
		private bool IsValidExpressionStatement( CodeStatement statement )
		{
			if( statement == null )
				throw new ArgumentNullException( "statement" );

			bool bIsValid = false;

			CodeExpressionStatement expressionStatement = statement as CodeExpressionStatement;
			if( expressionStatement != null )
			{
				CodeMethodInvokeExpression methodExpression = expressionStatement.Expression as CodeMethodInvokeExpression;
				if( methodExpression != null )
				{
					bIsValid = IsValidMethodInvokeExpression( methodExpression );
				}
			}

			return bIsValid;
		}

		private bool IsValidMethodInvokeExpression( CodeExpression expression )
		{
			if( expression == null )
				throw new ArgumentNullException( "expression" );

			bool bIsValid = false;

			CodeMethodInvokeExpression methodExpression = expression as CodeMethodInvokeExpression;
			if( methodExpression != null )
			{
				CodePropertyReferenceExpression parentProperty = methodExpression.Method.TargetObject as CodePropertyReferenceExpression;
				if( parentProperty != null )
				{
					bIsValid = IsValidPanelProperty( parentProperty );
				}
			}
			return bIsValid;
		}

		private bool IsValidAttachEventStatement( CodeStatement statement )
		{
			if( statement == null )
				throw new ArgumentNullException( "statement" );
            
			bool bIsValid = false;

			CodeAttachEventStatement eventStatement = statement as CodeAttachEventStatement;
			if( eventStatement != null )
			{
				CodePropertyReferenceExpression parentProperty = eventStatement.Event.TargetObject as CodePropertyReferenceExpression;
				if( parentProperty != null )
				{
                    bIsValid = IsValidPanelProperty( parentProperty );
				}
			}
            
				return bIsValid;
		}

		private bool IsValidCodeAssignStatement( CodeStatement statement )
		{
			if( statement == null )
				throw new ArgumentNullException( "statement" );

            bool bIsValid = false;
            
			CodeAssignStatement assignStatement = statement as CodeAssignStatement;
			if( assignStatement != null )
			{
				CodePropertyReferenceExpression property = assignStatement.Left as CodePropertyReferenceExpression;
				bIsValid = IsValidPanelProperty( property );
			}

			return bIsValid;
		}

		private bool IsValidPanelProperty( CodePropertyReferenceExpression property )
		{
			if( property == null )
				throw new ArgumentNullException( "property" );

			CodePropertyReferenceExpression panelProperty = null;

			bool bIsValid = ( IsPanelProperty( property.PropertyName ) && 
				( property.TargetObject as CodeFieldReferenceExpression != null ) );

			if( bIsValid )
			{
				panelProperty = property;
			}
			else
			{
				CodePropertyReferenceExpression parentProperty = 
					property.TargetObject as CodePropertyReferenceExpression;

				if( parentProperty != null )
				{
					bIsValid = IsValidPanelProperty( parentProperty );
				}
			}

			return bIsValid;
		}
        
		/// <summary>
		/// Check, if specified property name is split panel property name
		/// </summary>
		private bool IsPanelProperty( string propertyName )
		{
			if( propertyName == null )
				throw new ArgumentNullException( "propertyName" );

			bool bIsPanel = ( string.Compare( propertyName, DEF_PANEL1_NAME, true ) == 0 ||
				string.Compare( propertyName, DEF_PANEL2_NAME, true ) == 0 );

			return bIsPanel;
		}

		#endregion
	}
}

#endif