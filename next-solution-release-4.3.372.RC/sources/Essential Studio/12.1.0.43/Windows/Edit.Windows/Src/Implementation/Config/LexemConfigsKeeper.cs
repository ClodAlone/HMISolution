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

#define NO_GLOBAL_REGEX

#region file using directives
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Utils;

#endregion


namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Keeps configuration of Config lexems.
	/// Used to search configuration for some token.
	/// </summary>
	public class LexemConfigsKeeper
	{
		#region Class constants
		/// <summary>
		/// Regex group names prefix.
		/// </summary>
		private const string DEF_REGEX_GROUP_PREFIX = "g_";
		#endregion

		#region Private Structures
		/// <summary>
		/// Structure, used to keep information about one priority.
		/// </summary>
		private struct OnePriorityConfigs
		{
			/// <summary>
			/// Hashtable of non-regex lexem configs.
			/// Key - string, value - IConfigLexem or ICollection.
			/// </summary>
			public Hashtable NonRegExps;
			/// <summary>
			/// Regular expression, that contains all reg-exps.
			/// </summary>
			public Regex GroupedRegExps;
#if NO_GLOBAL_REGEX
			/// <summary>
			/// List of the configurations that belong to the current priority group.
			/// </summary>
			public ArrayList Configs;
#else
      /// <summary>
      /// Hash table of the regular expression groups.
      /// Key - Group name, value - IConfigLexem or ICollection.
      /// </summary>
      public Hashtable RegExpGroups;
#endif
		}
		#endregion

		#region Class members
		/// <summary>
		/// Table of configuration parts, grouped by priorities.
		/// Key - Priority. Value - OnePriorityConfigs instance.
		/// </summary>
		private SortedListEx m_Priorities = new SortedListEx();
		/// <summary>
		/// Lexem Configuration, the keeper is related to.
		/// </summary>
		private IConfigLexem m_KeepedConfig;
		/// <summary>
		/// Hashtable of the configs for token, already known.
		/// </summary>
		private Hashtable m_KnownConfigs = new Hashtable();
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Adds configuration to the hashtable.
		/// </summary>
		/// <param name="table">Destination hashtable.</param>
		/// <param name="key">Key in hashtable.</param>
		/// <param name="config">Configuration.</param>
		/// <remarks>If such key is not present, configuration will be added as a new item, 
		/// if configuration is already present, it will be changed to ArrayList with two configurations,
		/// If item with specified key is present and it is ArrayList, configuration will be added to that list.</remarks>
		private bool AddConfigToHashtable( Hashtable table, object key, IConfigLexem config )
		{
			if( table == null )
				throw new ArgumentNullException( "table" );

			if( config == null )
				throw new ArgumentNullException( "config" );

			object currentValue = table[ key ];
			bool bNewAdded = true;

			if( currentValue == null )
				table[ key ] = config;
			else
			{
				bNewAdded = false;

				if( currentValue is IConfigLexem )
				{
					object firstConfig = currentValue;
					ArrayList collection = new ArrayList( 4 );
					collection.Add( firstConfig );
					collection.Add( config );
					table[ key ] = collection;
				}
				else
				{
					ArrayList list = currentValue as ArrayList;
					list.Add( config );
				}
			}

			return bNewAdded;
		}
		/// <summary>
		/// Creates and initializes new instance of the class by config`s data.
		/// </summary>
		/// <param name="parentLexem">Owner of the LexemConfigsKeeper.</param>
		public LexemConfigsKeeper( IConfigLexem parentLexem )
		{
			if( parentLexem == null )
				throw new ArgumentNullException( "parentLexem" );

			m_KeepedConfig = parentLexem;

			// Key - priority, Value - IList
			SortedListEx byPriority = new SortedListEx();

			// Just sort
			foreach( IConfigLexem subConfig in parentLexem.SubLexems )
			{
				if( !byPriority.Contains( subConfig.Priority ) )
				{
					byPriority[ subConfig.Priority ] = new ArrayList();
				}

				( byPriority[ subConfig.Priority ] as IList ).Add( subConfig );
			}

			for( int i = 0; i < byPriority.Count; i++ )
			{
				int priority = ( int )byPriority.GetKey( i );
				IList configs = ( byPriority.GetByIndex( i ) as IList );

				OnePriorityConfigs currentConfigs = new OnePriorityConfigs();
				currentConfigs.NonRegExps = new Hashtable();
				StringBuilder regExpStr = new StringBuilder();
#if NO_GLOBAL_REGEX
				currentConfigs.Configs = new ArrayList();
#else
        currentConfigs.RegExpGroups = new Hashtable();
#endif

				foreach( IConfigLexem config in configs )
				{
					if( !config.IsBeginRegex )
					{
						string begin = ( parentLexem.Language.CaseInsensitive )
							? config.BeginBlock.ToLower()
							: config.BeginBlock;
						AddConfigToHashtable( currentConfigs.NonRegExps, begin, config );
					}
					else
					{
#if NO_GLOBAL_REGEX
						currentConfigs.Configs.Add( config );

#else
            string groupIndex = DEF_REGEX_GROUP_PREFIX + currentConfigs.RegExpGroups.Count.ToString();
            AddConfigToHashtable( currentConfigs.RegExpGroups, groupIndex, config );

						if( regExpStr.Length > 0 )
							regExpStr.Append( '|' );

						string str = config.BeginBlock;
						str = ConfigLexem.ReplaceNewLine( str );

						regExpStr.AppendFormat( "(?<{0}>{1})", groupIndex, str );
#endif
					}
				}

				if( regExpStr.Length > 0 )
				{
					RegexOptions options = RegexOptions.ExplicitCapture | Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX;

					if( parentLexem.Language.CaseInsensitive )
						options |= RegexOptions.IgnoreCase;

					currentConfigs.GroupedRegExps = new Regex( regExpStr.ToString(),
						options );
				}

				m_Priorities[ priority ] = currentConfigs;
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Looks for lexem configuration by the token.
		/// </summary>
		/// <remarks>
		/// Search process is entirely based on rules, specified by owner lexem configuration.
		/// If owner is complex, search is done within it`s sub-lexems first, then it is done for 
		/// its parent and so on. If there is no appropriate configuration for given token found, 
		/// languages configuration is used, or the configuration of the first parent with 
		/// "OnlyLocalSubLexems" set to true.
		/// </remarks>
		/// <param name="token">Token, configuration is needed for.</param>
		/// <returns>Lexem configuration for given token.</returns>
		public IList GetConfigs( string token )
		{
			if( token == null ) throw new ArgumentNullException( "token" );
			if( token.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_174 );

			if( m_KeepedConfig.Language.CaseInsensitive )
			{
				token = token.ToLower();
			}

#if !DEBUG
      if( m_KnownConfigs.Contains( token ) )
        return ( IList )m_KnownConfigs[ token ];
#endif

			ArrayList result = new ArrayList();

			if( m_KeepedConfig.IsComplex )
			{
				for( int i = 0, count = m_Priorities.Count; i < count; i++ )
				{
					OnePriorityConfigs configs = ( OnePriorityConfigs )m_Priorities.GetByIndex( i );

					if( configs.NonRegExps.Contains( token ) )
					{
						object configTemp = configs.NonRegExps[ token ];

						if( configTemp is IConfigLexem )
							result.Add( configTemp );
						else if( configTemp is ICollection )
							result.AddRange( configTemp as ICollection );

						continue;
					}

#if !NO_GLOBAL_REGEX
          if( configs.GroupedRegExps != null )
          {
						Match match = configs.GroupedRegExps.Match( token );
						
						if( match.Success )
						{
							for( int grpIndex = 1; grpIndex < match.Groups.Count; grpIndex++ )
							{
								if( match.Groups[ grpIndex ].Success && match.Groups[ grpIndex ].Value == token )
								{
									object configTemp = configs.RegExpGroups[ configs.GroupedRegExps.GroupNameFromNumber( grpIndex ) ];
            
									if( configTemp is IConfigLexem )
										result.Add( configTemp );
									else if( configTemp is ICollection )
										result.AddRange( configTemp as ICollection );
									else
									{
										if( configTemp != null )
											throw new NotSupportedException( string.Format( "{0} type is not supported.", configTemp.GetType().FullName ) );
										else
											throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_175 );
									}
								}
							}
						}
					}
#else
					ArrayList list = configs.Configs;


					for( int iGroup = 0, iGroupCount = list.Count; iGroup < iGroupCount; iGroup++ )
					{
						IConfigLexem config = ( IConfigLexem )list[ iGroup ];

						if( config.IsEqualToBegin( token ) )
						{
							result.Add( config );
						}
					}
#endif
				}

				for( int i = 0, count = m_KeepedConfig.References.Count; i < count; i++ )
				{
					IReferenceConfig refConfig = m_KeepedConfig.References[ i ] as IReferenceConfig;
					result.AddRange( refConfig.ReferencedLexem.FindConfigs( token ) );
				}

			}

			if( !m_KeepedConfig.OnlyLocalSublexems || !m_KeepedConfig.IsComplex )
				result.AddRange( m_KeepedConfig.ParentConfig.FindConfigs( token ) );

			result.Sort();

			m_KnownConfigs[ token ] = result;
			return result;
		}
		#endregion
	}
}
