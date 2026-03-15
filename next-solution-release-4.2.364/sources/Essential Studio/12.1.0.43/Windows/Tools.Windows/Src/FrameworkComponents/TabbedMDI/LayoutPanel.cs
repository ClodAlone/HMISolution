#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Panel that contains layout info about hosts contained in MDIClient.
	/// </summary>
	[Serializable]
	internal class LayoutPanel : ISerializable
	{
		#region Constants
		/// <summary>
		/// A default coefficient for panels dividing.
		/// </summary>
		private float COEFFICIENT = 0.5f;
		/// <summary>
		/// 
		/// </summary>
		private int MINIMUM_TABHOST_HEIGHT = 5;
		/// <summary>
		/// 
		/// </summary>
		private int MINIMUM_TABHOST_WIDTH = 5;
		/// <summary>
		/// 
		/// </summary>
		private int OFFSET = 3;
		/// <summary>
		/// 
		/// </summary>
		private int SPLITTER_WIDTH = 5;
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		public LayoutPanel()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		public LayoutPanel( Size size, Point location, bool horizontal )
		{
			m_szPanel = size;
			m_ptLocation = location;
			m_bHorizontal = horizontal;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hostOne"></param>
		public LayoutPanel( TabHost hostOne, Size size, Point location, bool horizontal ) :
			this( size, location, horizontal )
		{
			m_oComponentOne = hostOne;
			m_oComponentTwo = null;

			if( hostOne != null )
			{
				hostOne.LayoutPanel = this;
				m_sUniqueName = hostOne.Name;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hostOne"></param>
		/// <param name="hostTwo"></param>
		public LayoutPanel( TabHost hostOne, TabHost hostTwo, Size size, Point location, bool horizontal ) :
			this( size, location, horizontal )
		{
			m_oComponentOne = hostOne;
			m_oComponentTwo = hostTwo;

			// Set Layout panels to TabHosts.
			if( hostOne != null )
			{
				hostOne.LayoutPanel = this;
			}
			if( hostTwo != null )
			{
				hostTwo.LayoutPanel = this;
				m_sUniqueName = hostTwo.Name;
			}

			// Set TabHost.
			m_thTabHost = hostTwo;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets objects allocation inside Panel.
		/// </summary>
		internal bool Horizontal
		{
			get
			{
				return m_bHorizontal;
			}
			set
			{
				m_bHorizontal = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Size Size
		{
			get
			{
				return m_szPanel;
			}
			set
			{
				m_szPanel = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Point Location
		{
			get
			{
				return m_ptLocation;
			}
			set
			{
				m_ptLocation = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Point PanelOneLocation
		{
			get
			{
				return m_ptPanelOne;
			}
			set
			{
				m_ptPanelOne = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Point PanelTwoLocation
		{
			get
			{
				return m_ptPanelTwo;
			}
			set
			{
				m_ptPanelTwo = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Size PanelOneSize
		{
			get
			{
				return m_szPanelOne;
			}
			set
			{
				m_szPanelOne = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Size PanelTwoSize
		{
			get
			{
				return m_szPanelTwo;
			}
			set
			{
				m_szPanelTwo = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal object ComponentOne
		{
			get
			{
				return m_oComponentOne;
			}
			set
			{
				m_oComponentOne = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal object ComponentTwo
		{
			get
			{
				return m_oComponentTwo;
			}
			set
			{
				m_oComponentTwo = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Rectangle SplitterBounds
		{
			get
			{
				return m_rcSplitterBounds;
			}
			set
			{
				m_rcSplitterBounds = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal LayoutPanel PreviousPanel
		{
			get
			{
				return m_lpPreviousLayoutPanel;
			}
			set
			{
				m_lpPreviousLayoutPanel = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal TabHost TabHost
		{
			get
			{
				return m_thTabHost;
			}
			set
			{
				m_thTabHost = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal float Coefficient
		{
			get
			{
				return m_fCoefficient;
			}
			set
			{
				if( value < 0.0f )
				{
					value = 0.0f;
				}

				m_fCoefficient = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal string TabHostOneName
		{
			get
			{
				return m_sTabHostOneName;
			}
			set
			{
				m_sTabHostOneName = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal string TabHostTwoName
		{
			get
			{
				return m_sTabHostTwoName;
			}
			set
			{
				m_sTabHostTwoName = value;
			}
		}
		/// <summary>
		/// Gets or sets unique name to indentify Tab hosts and layout panels.
		/// </summary>
		internal string UniqueName
		{
			get
			{
				return m_sUniqueName;
			}
			set
			{
				m_sUniqueName = value;
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void InitializePanelOne( TabHost pTabHost )
		{
			// PanelOne Location and Size.
			m_ptPanelOne = m_ptLocation;
			m_szPanelOne = m_szPanel;

			// As we have only one panel, so its SplitterHost must be invisible.
			if( pTabHost != null )
			{
				if( pTabHost.SplitterHost != null )
				{
					pTabHost.SplitterBounds = Rectangle.Empty;
					pTabHost.SplitterHost.Visible = false;
				}
			}
		}
		/// <summary>
		/// Divides panel on two parts, sets locations and sizes for panel.
		/// </summary>
		public void DividePanelOnTwoParts()
		{
			if( m_oComponentOne == null || m_oComponentTwo == null )
			{
				m_ptPanelOne = m_ptLocation;
				m_szPanelOne = m_szPanel;
				m_ptPanelTwo = m_ptLocation;
				m_szPanelTwo = m_szPanel;

				m_rcSplitterBounds = Rectangle.Empty;
			}
			else
			{
				if( m_bHorizontal )
				{
					int iPanelHeight = ( int ) ( ( m_szPanel.Height - TabbedMDIManager.SPLITTER_WIDTH ) * m_fCoefficient );

					// PanelOne Location and Size.
					m_ptPanelOne = m_ptLocation;
					m_szPanelOne = new Size( m_szPanel.Width, iPanelHeight );

					// PanelTwo Location and Size.
					int iPanelTwoY = m_ptLocation.Y + iPanelHeight + TabbedMDIManager.SPLITTER_WIDTH;
					m_ptPanelTwo = new Point( m_ptLocation.X, iPanelTwoY );
					m_szPanelTwo = new Size( m_szPanel.Width, m_ptLocation.Y + m_szPanel.Height - iPanelTwoY );

					// Splitter bounds.
					int iSplitterX = m_ptLocation.X;
					int iSplitterY = m_ptLocation.Y + iPanelHeight;
					m_rcSplitterBounds = new Rectangle( iSplitterX, iSplitterY, m_szPanel.Width, TabbedMDIManager.SPLITTER_WIDTH );
				}
				else
				{
					int iPanelWidth = ( int ) ( ( m_szPanel.Width - TabbedMDIManager.SPLITTER_WIDTH ) * m_fCoefficient );

					// PanelOne Location and Size.
					m_ptPanelOne = m_ptLocation;
					m_szPanelOne = new Size( iPanelWidth, m_szPanel.Height );

					// PanelTwo Location and Size.
					int iPanelTwoX = m_ptLocation.X + iPanelWidth + TabbedMDIManager.SPLITTER_WIDTH;
					m_ptPanelTwo = new Point( iPanelTwoX, m_ptLocation.Y );
					m_szPanelTwo = new Size( m_ptLocation.X + m_szPanel.Width - iPanelTwoX, m_szPanel.Height );

					// Splitter bounds.
					int iSplitterX = m_ptLocation.X + iPanelWidth;
					int iSplitterY = m_ptLocation.Y;
					m_rcSplitterBounds = new Rectangle( iSplitterX, iSplitterY, TabbedMDIManager.SPLITTER_WIDTH, m_szPanel.Height );
				}
			}

			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.m_szPanel = ( i == 0 ) ? m_szPanelOne : m_szPanelTwo;
					lpPanel.m_ptLocation = ( i == 0 ) ? m_ptPanelOne : m_ptPanelTwo;
					lpPanel.DividePanelOnTwoParts();
				}
			}

			if( m_thTabHost != null )
			{
				m_thTabHost.SplitterBounds = m_rcSplitterBounds;
			}
		}
		/// <summary>
		/// Method finds TabHost equal to previous TabHost and creates new LayoutPanel 
		/// on its place with TabHost and previous TabHost.
		/// </summary>
		/// <param name="pPrevTabHost"> Previous TabHost. </param>
		/// <param name="pTabHost"> Newly created TabHost. </param>
		/// <param name="horizontal"> Horizontal alignment for LayoutPanel. </param>
		/// <returns></returns>
		public void DivideTabHost( TabHost pPrevTabHost, TabHost pTabHost, bool horizontal )
		{
			bool bIsComponentOne = true;
			bool bTabHostFound = false;

			if( m_oComponentOne is TabHost && ( TabHost ) m_oComponentOne == pPrevTabHost ||
				m_oComponentTwo is TabHost && ( TabHost ) m_oComponentTwo == pPrevTabHost )
			{
				bTabHostFound = true;

				if( m_oComponentTwo is TabHost && ( TabHost ) m_oComponentTwo == pPrevTabHost )
				{
					bIsComponentOne = false;
				}
			}

			if( bTabHostFound )
			{
				// Create layout panel and divide it on two parts for pPrevTabHost and pTabHost.
				LayoutPanel layoutPanel = new LayoutPanel( pPrevTabHost, pTabHost,
					bIsComponentOne ? m_szPanelOne : m_szPanelTwo,
					bIsComponentOne ? m_ptPanelOne : m_ptPanelTwo, horizontal );
				layoutPanel.DividePanelOnTwoParts();

				// Set splitter bounds and alignment. Also set LayoutPanel.
				pTabHost.SplitterBounds = layoutPanel.SplitterBounds;
				pTabHost.SplitterHost.Horizontal = horizontal;
				pTabHost.SplitterHost.LayoutPanel = this;

				// Set PreviousPanel.
				layoutPanel.m_lpPreviousLayoutPanel = this;

				if( bIsComponentOne )
				{
					m_oComponentOne = layoutPanel;
				}
				else
				{
					m_oComponentTwo = layoutPanel;
				}
			}
		}
		/// <summary>
		/// Method finds TabHost and delete it reference from LayoutPanel.
		/// </summary>
		/// <param name="pPreviousPanel"> Previous LayoutPanel. </param>
		/// <param name="pTabHost"> TabHost to remove reference. </param>
		/// <returns></returns>
		public void FindAndDeleteTabHost( LayoutPanel pPreviousPanel, TabHost pTabHost )
		{
			bool bIsComponentOne = true;
			bool bTabHostFound = false;

			if( m_oComponentOne is TabHost && ( TabHost ) m_oComponentOne == pTabHost ||
				m_oComponentTwo is TabHost && ( TabHost ) m_oComponentTwo == pTabHost )
			{
				bTabHostFound = true;

				if( m_oComponentTwo is TabHost && ( TabHost ) m_oComponentTwo == pTabHost )
				{
					bIsComponentOne = false;
				}
			}

			if( bTabHostFound )
			{
				if( bIsComponentOne )
				{
					m_oComponentOne = null;
				}
				else
				{
					m_oComponentTwo = null;
				}

				if( pPreviousPanel != null )
				{
					object[] lpPanels = new object[] { pPreviousPanel.m_oComponentOne, pPreviousPanel.m_oComponentTwo };

					for( int i = 0; i < lpPanels.Length; i++ )
					{
						if( lpPanels[ i ] is LayoutPanel )
						{
							LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;

							if( this == lpPanel )
							{
								object oComponent = bIsComponentOne ? m_oComponentTwo : m_oComponentOne;

								if( oComponent is TabHost )
								{
									TabHost host = oComponent as TabHost;
									host.LayoutPanel = pPreviousPanel;

									if( i == 0 )
									{
										pPreviousPanel.m_oComponentOne = host;
									}
									else
									{
										pPreviousPanel.m_oComponentTwo = host;
									}
								}
								else if( oComponent is LayoutPanel )
								{
									LayoutPanel previousPanel = oComponent as LayoutPanel;
									previousPanel.m_lpPreviousLayoutPanel = pPreviousPanel;

									if( i == 0 )
									{
										pPreviousPanel.m_oComponentOne = previousPanel;
									}
									else
									{
										pPreviousPanel.m_oComponentTwo = previousPanel;
									}
								}
								else
								{
									if( i == 0 )
									{
										pPreviousPanel.m_oComponentOne = null;
									}
									else
									{
										pPreviousPanel.m_oComponentTwo = null;
									}
								}

								break;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void SetSplitterHosts()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.SetSplitterHosts();
				}
			}

			//Set SplitterHost bounds and direction.
			if( m_thTabHost != null )
			{
				m_thTabHost.SplitterBounds = m_rcSplitterBounds;

				if( m_thTabHost.SplitterHost != null ) // Set SplitterHost's LayoutPanel.
				{
					m_thTabHost.SplitterHost.LayoutPanel = this;
				}
			}
		}
		/// <summary>
		/// Set LayoutPanel's TabHost.
		/// </summary>
		public void SetTabHosts()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.SetTabHosts();
				}
			}

			m_thTabHost = GetTabHost( false );
		}
		/// <summary>
		/// Gets TabHost which belong to Panel.
		/// </summary>
		/// <param name="bComponentOne"> Indicates that TabHost should be get from ComponentOne, otherwise from ComponentTwo. </param>
		/// <returns></returns>
		public TabHost GetTabHost( bool bComponentOne )
		{
			TabHost tabHost = null;
			object oComponent = ( bComponentOne ) ? m_oComponentOne : m_oComponentTwo;

			if( oComponent is LayoutPanel )
			{
				LayoutPanel layoutPanel = oComponent as LayoutPanel;
				tabHost = layoutPanel.GetTabHost( true );
			}
			else if( oComponent is TabHost )
			{
				tabHost = oComponent as TabHost;
			}

			return tabHost;
		}
		/// <summary>
		/// Set LayoutPanel's unique name based on this TabHost's name.
		/// </summary>
		public void SetUniqueNames()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.SetUniqueNames();
				}
			}

			if( m_thTabHost != null )
			{
				m_sUniqueName = m_thTabHost.Name;
			}
		}
		/// <summary>
		/// Uses in deserialization. Set LayoutPanel's components to null if LayoutPanel is nullable.
		/// </summary>
		/// <returns></returns>
		public void SetNullablePanels()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.SetNullablePanels();
				}
			}

			RemoveNullablePanels();
		}
		/// <summary>
		/// Uses in deserialization. Some panels can contain ComponentOne and 
		/// ComponentTwo with null values.
		/// </summary>
		public void RemoveNullablePanels()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					if( lpPanel.IsNullablePanel() )
					{
						if( i == 0 )
						{
							m_oComponentOne = null;
						}
						else
						{
							m_oComponentTwo = null;
						}

					}
				}
			}
		}
		/// <summary>
		/// Indicates if panel ComponentOne and ComponentTwo have null references.
		/// </summary>
		/// <returns></returns>
		public bool IsNullablePanel()
		{
			if( m_oComponentOne == null && m_oComponentTwo == null )
			{
				return true;
			}

			return false;
		}
		/// <summary>
		/// Set LayoutPanel for own TabHosts.
		/// </summary>
		public void SetLayoutPanels()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.SetLayoutPanels();
				}

				if( lpPanels[ i ] is TabHost )
				{
					TabHost thTabHost = lpPanels[ i ] as TabHost;
					thTabHost.LayoutPanel = this;
				}
			}
		}
        internal void BalanceEqualWeights()
        {
            object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };
            for (int i = 0; i < lpPanels.Length; i++)
            {
                if (lpPanels[i] is LayoutPanel)
                {
                    LayoutPanel lpPanel = lpPanels[i] as LayoutPanel;
                    lpPanel.BalanceEqualWeights();
                }
            }
            m_fCoefficient = (float)1 / 2;
        }
        /// <summary>
        /// Sets equal weights for all the tab hosts.
        /// </summary>
        public void SetEqualWeights()
        {
            object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

            for (int i = 0; i < lpPanels.Length; i++)
            {
                if (lpPanels[i] is LayoutPanel)
                {
                    LayoutPanel lpPanel = lpPanels[i] as LayoutPanel;
                    lpPanel.SetEqualWeights();
                }
            }

            int count = 0;
            this.GetTabHostsCount(ref count, true);
            if (count != 1)
                m_fCoefficient = (float)1 / count; 
        }

		/// <summary>
		/// Sets coefficient to default value.
		/// </summary>
		public void SetCustomWeights()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.SetCustomWeights();
				}
			}

			m_fCoefficient = COEFFICIENT;
		}
		/// <summary>
		/// Toggles LayoutPanel alignment.
		/// </summary>
		public void AlignmentToggle()
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.AlignmentToggle();
				}
			}

			m_bHorizontal = !m_bHorizontal;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pTabHost"></param>
		public void MaximizeTabGroup( TabHost pTabHost, LayoutPanel pMainLayoutPanel )
		{
			LayoutPanel lpPanel = pTabHost.LayoutPanel;
			bool bTabHostOnComponentOne = true;

			bTabHostOnComponentOne = IsTabHostOnComponentOne( pTabHost, pMainLayoutPanel );
			pMainLayoutPanel.MinimizePanel( bTabHostOnComponentOne );

			while( pTabHost.LayoutPanel != pMainLayoutPanel )
			{
				pMainLayoutPanel = bTabHostOnComponentOne ?
					pMainLayoutPanel.ComponentOne as LayoutPanel :
					pMainLayoutPanel.ComponentTwo as LayoutPanel;

				bTabHostOnComponentOne = IsTabHostOnComponentOne( pTabHost, pMainLayoutPanel );
				pMainLayoutPanel.MinimizePanel( bTabHostOnComponentOne );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bTabHostOnComponentOne"></param>
		/// <param name="pMainLayoutPanel"></param>
		private void MinimizePanel( bool bTabHostOnComponentOne )
		{
			if( bTabHostOnComponentOne )
			{
				if( m_oComponentTwo is TabHost )
				{
					RecalculateCoefficient( false, 1 );
				}
				else if( m_oComponentTwo is LayoutPanel )
				{
					int iTabHostCount = 0;
					LayoutPanel lpPanelTwo = m_oComponentTwo as LayoutPanel;

					RecalculateCoefficient( false, lpPanelTwo.GetTabHostsCount( ref iTabHostCount, true ) );
					lpPanelTwo.DivideAndMinimize( iTabHostCount );
				}
			}
			else
			{
				if( m_oComponentOne is TabHost )
				{
					RecalculateCoefficient( true, 1 );
				}
				else if( m_oComponentOne is LayoutPanel )
				{
					int iTabHostCount = 0;
					LayoutPanel lpPanelOne = m_oComponentOne as LayoutPanel;

					RecalculateCoefficient( true, lpPanelOne.GetTabHostsCount( ref iTabHostCount, true ) );
					lpPanelOne.DivideAndMinimize( iTabHostCount );
				}
			}

			DividePanelOnTwoParts();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="TabHostCount"></param>
		private void DivideAndMinimize( int pTabHostCount )
		{
			if( m_oComponentOne is TabHost && m_oComponentTwo is TabHost )
			{
				m_fCoefficient = COEFFICIENT;
			}
			else if( m_oComponentOne is TabHost && m_oComponentTwo is LayoutPanel )
			{
				LayoutPanel lpPanelTwo = m_oComponentTwo as LayoutPanel;

				m_fCoefficient = ( float ) 1 / pTabHostCount;
				lpPanelTwo.DivideAndMinimize( pTabHostCount-- );
			}
			else if( m_oComponentOne is LayoutPanel && m_oComponentTwo is TabHost )
			{
				LayoutPanel lpPanelOne = m_oComponentOne as LayoutPanel;

				m_fCoefficient = ( float ) 1 - ( float ) 1 / pTabHostCount;
				lpPanelOne.DivideAndMinimize( pTabHostCount-- );
			}
			else if( m_oComponentOne is LayoutPanel && m_oComponentTwo is LayoutPanel )
			{
				LayoutPanel lpPanelOne = m_oComponentOne as LayoutPanel;
				LayoutPanel lpPanelTwo = m_oComponentTwo as LayoutPanel;

				m_fCoefficient = COEFFICIENT;

				lpPanelOne.DivideAndMinimize( pTabHostCount );
				lpPanelTwo.DivideAndMinimize( pTabHostCount );
			}
			else
			{
				SetCustomWeights();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pIsComponentOne"></param>
		/// <param name="pTabHostCount"></param>
		private void RecalculateCoefficient( bool pMinimizeComponentOne, int pTabHostCount )
		{
			if( m_bHorizontal )
			{
				int iTabHostHeight = MINIMUM_TABHOST_HEIGHT;

				if( m_thTabHost != null && m_thTabHost.MDITabPanel != null )
				{
					iTabHostHeight = m_thTabHost.MDITabPanel.Height + OFFSET;
				}

				m_fCoefficient = pMinimizeComponentOne ?
					( float ) ( iTabHostHeight * pTabHostCount + ( pTabHostCount - 1 ) * SPLITTER_WIDTH ) / this.Size.Height :
					( float ) 1 - ( float ) ( iTabHostHeight * pTabHostCount + ( pTabHostCount - 1 ) * SPLITTER_WIDTH ) / this.Size.Height;
			}
			else
			{
				int iTabHostWidth = MINIMUM_TABHOST_WIDTH;

				if( m_thTabHost != null && m_thTabHost.MDITabPanel != null )
				{
					if( m_thTabHost.MDITabPanel.IsVerticalAlignment )
					{
						iTabHostWidth = m_thTabHost.MDITabPanel.Width + OFFSET;
					}
				}

				m_fCoefficient = pMinimizeComponentOne ?
					( float ) ( iTabHostWidth * pTabHostCount + ( pTabHostCount - 1 ) * SPLITTER_WIDTH ) / this.Size.Width :
					( float ) 1 - ( float ) ( iTabHostWidth * pTabHostCount + ( pTabHostCount - 1 ) * SPLITTER_WIDTH ) / this.Size.Width;
			}

			if( m_fCoefficient <= 0.0f || m_fCoefficient >= 1.0f )
			{
				m_fCoefficient = COEFFICIENT;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pTabHost"></param>
		/// <param name="pLayoutPanel"></param>
		/// <returns></returns>
		private bool IsTabHostOnComponentOne( TabHost pTabHost, LayoutPanel pLayoutPanel )
		{
			bool bIsComponentOne = false;
			LayoutPanel lpPanel = pTabHost.LayoutPanel;

			if( lpPanel == pLayoutPanel )
			{
				if( pLayoutPanel.ComponentOne is TabHost )
				{
					TabHost tabHost = pLayoutPanel.ComponentOne as TabHost;

					if( tabHost == pTabHost )
					{
						bIsComponentOne = true;
					}
				}
				if( pLayoutPanel.ComponentTwo is TabHost )
				{
					TabHost tabHost = pLayoutPanel.ComponentTwo as TabHost;

					if( tabHost == pTabHost )
					{
						bIsComponentOne = false;
					}
				}
			}
			else
			{
				while( lpPanel.PreviousPanel != pLayoutPanel )
				{
					lpPanel = lpPanel.PreviousPanel;
				}

				if( pLayoutPanel.ComponentOne is LayoutPanel )
				{
					LayoutPanel lpPanelOne = pLayoutPanel.ComponentOne as LayoutPanel;

					if( lpPanelOne == lpPanel )
					{
						bIsComponentOne = true;
					}
				}
				if( pLayoutPanel.ComponentTwo is LayoutPanel )
				{
					LayoutPanel lpPanelTwo = pLayoutPanel.ComponentTwo as LayoutPanel;

					if( lpPanelTwo == lpPanel )
					{
						bIsComponentOne = false;
					}
				}
			}

			return bIsComponentOne;
		}
		/// <summary>
		/// Get quantity of TabHost controls in a current LayoutPanel.
		/// </summary>
		/// <param name="pTabHostCount"> The quantity of TabHost controls in a current LayoutPanel. </param>
		/// <param name="pForMaximize"> Indicates whether additonal tab host must be added. </param>
		/// <returns></returns>
		internal int GetTabHostsCount( ref int pTabHostCount, bool pForMaximize )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					if( !pForMaximize )
					{
						pTabHostCount++;
					}

					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.GetTabHostsCount( ref pTabHostCount, pForMaximize );
				}

				if( lpPanels[ i ] is TabHost )
				{
					pTabHostCount++;
				}
			}

			return pTabHostCount;
		}
		/// <summary>
		/// Divides TabHost controls which are situated before current TabHost control and
		/// after it and fills corresponding collections. 
		/// </summary>
		/// <param name="pTabHost"> Current TabHost control. </param>
		/// <param name="pPreviousHosts"> Collection with TabHost controls which are 
		/// situated before current TabHost control. </param>
		/// <param name="pNextHosts"> Collection with TabHost controls which are
		/// situated before current TabHost control. </param>
		/// <param name="bIsPrevious"> Indicates to which collection TabHost control must be added. </param>
		internal void GetNextAndPreviousTabHosts( TabHost pTabHost, ref ArrayList pPreviousHosts, ref ArrayList pNextHosts, ref bool bIsPrevious )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.GetNextAndPreviousTabHosts( pTabHost, ref pPreviousHosts, ref pNextHosts, ref bIsPrevious );
				}
				else if( lpPanels[ i ] is TabHost )
				{
					TabHost thHost = lpPanels[ i ] as TabHost;

					if( thHost == pTabHost )
					{
						bIsPrevious = false;
					}

					if( bIsPrevious )
					{
						pPreviousHosts.Add( thHost );
					}
					else if( thHost != pTabHost )
					{
						pNextHosts.Add( thHost );
					}
				}
			}
		}
		/// <summary>
		/// Gets size of current TabHost.
		/// Used to change TabHost size when visual style of TabbedMDIManager is changed.
		/// </summary>
		/// <param name="pHost"> The current TabHost. </param>
		internal Size GetTabHostSize( TabHost pHost )
		{
			Size szResult = Size.Empty;

			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is TabHost )
				{
					TabHost thHost = lpPanels[ i ] as TabHost;

					if( thHost == pHost )
					{
						szResult = ( i == 0 ) ? m_szPanelOne : m_szPanelTwo;
					}
				}
			}

			return szResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iRight"></param>
		internal void GetDistanceToRight( ref int iDistance, int iRight )
		{
			if( m_oComponentTwo is TabHost )
			{
				iDistance = m_szPanel.Width - m_szPanelOne.Width - SPLITTER_WIDTH - 1;
			}
			else if( m_oComponentTwo is LayoutPanel )
			{
				LayoutPanel lpPanelTwo = m_oComponentTwo as LayoutPanel;

				iDistance = lpPanelTwo.m_szPanel.Width;
				lpPanelTwo.GetDistancesToRight( ref iDistance, iRight );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iRight"></param>
		private void GetDistancesToRight( ref int iDistance, int iRight )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.GetDistancesToRight( ref iDistance, iRight );
				}
			}

			int iDistanceOne = m_ptPanelOne.X + m_szPanelOne.Width - iRight;
			int iDistanceTwo = m_ptPanelTwo.X + m_szPanelTwo.Width - iRight;

			if( iDistanceOne < iDistance )
			{
				iDistance = iDistanceOne;
			}

			if( iDistanceTwo < iDistance )
			{
				iDistance = iDistanceTwo;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iLeft"></param>
		internal void GetDistanceToLeft( ref int iDistance, int iLeft )
		{
			if( m_oComponentOne is TabHost )
			{
				iDistance = m_szPanelOne.Width - 1;
			}
			else if( m_oComponentOne is LayoutPanel )
			{
				LayoutPanel lpPanelOne = m_oComponentOne as LayoutPanel;

				iDistance = lpPanelOne.m_szPanel.Width;
				lpPanelOne.GetDistancesToLeft( ref iDistance, iLeft );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iLeft"></param>
		private void GetDistancesToLeft( ref int iDistance, int iLeft )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.GetDistancesToLeft( ref iDistance, iLeft );
				}
			}

			int iDistanceOne = iLeft - m_ptPanelOne.X;
			int iDistanceTwo = iLeft - m_ptPanelTwo.X;

			if( iDistanceOne < iDistance )
			{
				iDistance = iDistanceOne;
			}

			if( iDistanceTwo < iDistance )
			{
				iDistance = iDistanceTwo;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iDown"></param>
		internal void GetDistanceToDown( ref int iDistance, int iDown )
		{
			if( m_oComponentTwo is TabHost )
			{
				iDistance = m_szPanel.Height - m_szPanelOne.Height - SPLITTER_WIDTH - 1;
			}
			else if( m_oComponentTwo is LayoutPanel )
			{
				LayoutPanel lpPanelTwo = m_oComponentTwo as LayoutPanel;

				iDistance = lpPanelTwo.m_szPanel.Height;
				lpPanelTwo.GetDistancesToDown( ref iDistance, iDown );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iDown"></param>
		private void GetDistancesToDown( ref int iDistance, int iDown )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.GetDistancesToDown( ref iDistance, iDown );
				}
			}

			int iDistanceOne = m_ptPanelOne.Y + m_szPanelOne.Height - iDown;
			int iDistanceTwo = m_ptPanelTwo.Y + m_szPanelTwo.Height - iDown;

			if( iDistanceOne < iDistance )
			{
				iDistance = iDistanceOne;
			}

			if( iDistanceTwo < iDistance )
			{
				iDistance = iDistanceTwo;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDistance"></param>
		/// <param name="iRight"></param>
		internal void GetDistanceToTop( ref int iDistance, int iTop )
		{
			if( m_oComponentOne is TabHost )
			{
				iDistance = m_szPanelOne.Height - 1;
			}
			else if( m_oComponentOne is LayoutPanel )
			{
				LayoutPanel lpPanelOne = m_oComponentOne as LayoutPanel;

				iDistance = lpPanelOne.m_szPanel.Height;
				lpPanelOne.GetDistancesToTop( ref iDistance, iTop );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lstRight"></param>
		private void GetDistancesToTop( ref int iDistance, int iTop )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.GetDistancesToTop( ref iDistance, iTop );
				}
			}

			int iDistanceOne = iTop - m_ptPanelOne.Y;
			int iDistanceTwo = iTop - m_ptPanelTwo.Y;

			if( iDistanceOne < iDistance )
			{
				iDistance = iDistanceOne;
			}

			if( iDistanceTwo < iDistance )
			{
				iDistance = iDistanceTwo;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDelta"></param>
		internal void SplitterMoved( int iDelta )
		{
			if( m_thTabHost != null )
			{
				RecalculateCoefficientForTwoTabHosts( iDelta );
				
				if( m_oComponentOne is TabHost && m_oComponentTwo is LayoutPanel )
				{
					LayoutPanel lpPanel = m_oComponentTwo as LayoutPanel;
					lpPanel.ChangeBelongedTabHostsRightOrBottom( iDelta, m_rcSplitterBounds, m_bHorizontal );
				}
				else if( m_oComponentOne is LayoutPanel && m_oComponentTwo is TabHost )
				{
					LayoutPanel lpPanel = m_oComponentOne as LayoutPanel;
					lpPanel.ChangeBelongedTabHostsLeftOrTop( iDelta, m_rcSplitterBounds, m_bHorizontal );
				}
				else if( m_oComponentOne is LayoutPanel && m_oComponentTwo is LayoutPanel )
				{
					LayoutPanel lpPanelOne = m_oComponentOne as LayoutPanel;
					lpPanelOne.ChangeBelongedTabHostsLeftOrTop( iDelta, m_rcSplitterBounds, m_bHorizontal );
					LayoutPanel lpPanelTwo = m_oComponentTwo as LayoutPanel;
					lpPanelTwo.ChangeBelongedTabHostsRightOrBottom( iDelta, m_rcSplitterBounds, m_bHorizontal );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDelta"></param>
		private void RecalculateCoefficientForTwoTabHosts( int iDelta )
		{
			if( m_bHorizontal )
			{
				m_fCoefficient = ( float ) ( m_szPanelOne.Height + iDelta ) / ( m_szPanel.Height - SPLITTER_WIDTH );
			}
			else
			{
				m_fCoefficient = ( float ) ( m_szPanelOne.Width + iDelta ) / ( m_szPanel.Width - SPLITTER_WIDTH );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDelta"></param>
		/// <param name="rcSplitterBounds"></param>
		/// <param name="bHorizontal"></param>
		private void ChangeBelongedTabHostsRightOrBottom( int iDelta, Rectangle rcSplitterBounds, bool bHorizontal )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.ChangeBelongedTabHostsRightOrBottom( iDelta, rcSplitterBounds, bHorizontal );
				}
			}

			if( bHorizontal )
			{
				if( m_ptPanelOne.Y == rcSplitterBounds.Bottom )
				{
					m_szPanel.Height -= iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelOne.Height -= iDelta;
						m_fCoefficient = ( float ) ( m_szPanelOne.Height ) / ( m_szPanel.Height - SPLITTER_WIDTH );
					}
				}

				if( m_ptPanelTwo.Y == rcSplitterBounds.Bottom )
				{
					m_szPanel.Height -= iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelTwo.Height -= iDelta;
						m_fCoefficient = ( float ) 1 - ( float ) ( m_szPanelTwo.Height ) / ( m_szPanel.Height - SPLITTER_WIDTH );
					}
				}
			}
			else
			{
				if( m_ptPanelOne.X == rcSplitterBounds.Right )
				{
					m_szPanel.Width -= iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelOne.Width -= iDelta;
						m_fCoefficient = ( float ) ( m_szPanelOne.Width ) / ( m_szPanel.Width - SPLITTER_WIDTH );
					}
				}

				if( m_ptPanelTwo.X == rcSplitterBounds.Right )
				{
					m_szPanel.Width -= iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelTwo.Width -= iDelta;
						m_fCoefficient = ( float ) 1 - ( float ) ( m_szPanelTwo.Width ) / ( m_szPanel.Width - SPLITTER_WIDTH );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDelta"></param>
		/// <param name="rcSplitterBounds"></param>
		/// <param name="bHorizontal"></param>
		private void ChangeBelongedTabHostsLeftOrTop( int iDelta, Rectangle rcSplitterBounds, bool bHorizontal )
		{
			object[] lpPanels = new object[] { m_oComponentOne, m_oComponentTwo };

			for( int i = 0; i < lpPanels.Length; i++ )
			{
				if( lpPanels[ i ] is LayoutPanel )
				{
					LayoutPanel lpPanel = lpPanels[ i ] as LayoutPanel;
					lpPanel.ChangeBelongedTabHostsLeftOrTop( iDelta, rcSplitterBounds, bHorizontal );
				}
			}

			if( bHorizontal )
			{
				if( m_ptPanelTwo.Y + m_szPanelTwo.Height == rcSplitterBounds.Top )
				{
					m_szPanel.Height += iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelTwo.Height += iDelta;
						m_fCoefficient = ( float ) 1 - ( float ) ( m_szPanelTwo.Height ) / ( m_szPanel.Height - SPLITTER_WIDTH );
					}
				}

				if( m_ptPanelOne.Y + m_szPanelOne.Height == rcSplitterBounds.Top )
				{
					m_szPanel.Height += iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelOne.Height += iDelta;
						m_fCoefficient = ( float ) ( m_szPanelOne.Height ) / ( m_szPanel.Height - SPLITTER_WIDTH );
					}
				}
			}
			else
			{
				if( m_ptPanelTwo.X + m_szPanelTwo.Width == rcSplitterBounds.Left )
				{
					m_szPanel.Width += iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelTwo.Width += iDelta;
						m_fCoefficient = ( float ) 1 - ( float ) ( m_szPanelTwo.Width ) / ( m_szPanel.Width - SPLITTER_WIDTH );
					}
				}

				if( m_ptPanelOne.X + m_szPanelOne.Width == rcSplitterBounds.Left )
				{
					m_szPanel.Width += iDelta;

					if( m_bHorizontal == bHorizontal )
					{
						m_szPanelOne.Width += iDelta;
						m_fCoefficient = ( float ) ( m_szPanelOne.Width ) / ( m_szPanel.Width - SPLITTER_WIDTH );
					}
				}
			}
		}
		#endregion

		#region Serialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public LayoutPanel( SerializationInfo info, StreamingContext context )
		{
			// Restore unique name.
			m_sUniqueName = info.GetString( "UniqueName" );
			// Restore coefficient.
			m_fCoefficient = info.GetSingle( "Coefficient" );
			// Restore horizontal value.
			m_bHorizontal = info.GetBoolean( "Horizontal" );

			foreach( SerializationEntry entry in info )
			{
				if( entry.Name == "TabHost1" + m_sUniqueName )
				{
					m_sTabHostOneName = info.GetString( "TabHost1" + m_sUniqueName );
				}
				else if( entry.Name == "TabHost2" + m_sUniqueName )
				{
					m_sTabHostTwoName = info.GetString( "TabHost2" + m_sUniqueName );
				}
				else if( entry.Name == "LayoutPanel1" + m_sUniqueName )
				{
					m_oComponentOne = info.GetValue( "LayoutPanel1" + m_sUniqueName, typeof( LayoutPanel ) );
				}
				else if( entry.Name == "LayoutPanel2" + m_sUniqueName )
				{
					m_oComponentTwo = info.GetValue( "LayoutPanel2" + m_sUniqueName, typeof( LayoutPanel ) );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			// Add TabHostOne name to SerializationInfo.
			if( m_oComponentOne is TabHost )
			{
				TabHost tabHost = m_oComponentOne as TabHost;
				info.AddValue( "TabHost1" + m_sUniqueName, tabHost.Name );
			}
			// Add LayoutPanel to SerializationInfo, 
			// it must invoke recursive call GeObjectData  method with m_oComponentTwo as LayoutPanel.
			else if( m_oComponentOne is LayoutPanel )
			{
				info.AddValue( "LayoutPanel1" + m_sUniqueName,
					m_oComponentOne, typeof( LayoutPanel ) );
			}

			// Add TabHostTwo name to SerializationInfo.
			if( m_oComponentTwo is TabHost )
			{
				TabHost tabHost = m_oComponentTwo as TabHost;
				info.AddValue( "TabHost2" + m_sUniqueName, tabHost.Name );
			}
			// Add LayoutPanel to SerializationInfo, 
			// it must invoke recursive call GeObjectData  method with m_oComponentTwo as LayoutPanel.
			else if( m_oComponentTwo is LayoutPanel )
			{
				info.AddValue( "LayoutPanel2" + m_sUniqueName,
					m_oComponentTwo, typeof( LayoutPanel ) );
			}

			// Add coefficient.
			info.AddValue( "Coefficient", m_fCoefficient );
			// Add unique name.
			info.AddValue( "UniqueName", m_sUniqueName );
			// Add Horizontal value.
			info.AddValue( "Horizontal", m_bHorizontal );
		}
		#endregion

		#region Fields
		/// <summary>
		/// Indicates how Panel divides itself on two parts. True - horizontally, false - vertically.
		/// </summary>
		private bool m_bHorizontal = false;
		/// <summary>
		/// 
		/// </summary>
		[NonSerialized]
		private object m_oComponentOne = null;
		/// <summary>
		/// 
		/// </summary>
		[NonSerialized]
		private object m_oComponentTwo = null;
		/// <summary>
		/// LayoutPanel's size.
		/// </summary>
		private Size m_szPanel;
		/// <summary>
		/// LayoutPanel's location.
		/// </summary>
		private Point m_ptLocation;
		/// <summary>
		/// 
		/// </summary>
		private Point m_ptPanelOne;
		/// <summary>
		/// 
		/// </summary>
		private Point m_ptPanelTwo;
		/// <summary>
		/// 
		/// </summary>
		private Size m_szPanelOne;
		/// <summary>
		/// 
		/// </summary>
		private Size m_szPanelTwo;
		/// <summary>
		/// 
		/// </summary>
		private Rectangle m_rcSplitterBounds;
		/// <summary>
		/// 
		/// </summary>
		private LayoutPanel m_lpPreviousLayoutPanel = null;
		/// <summary>
		/// TabHost that contain info about SplitterHost.
		/// </summary>
		[NonSerialized]
		private TabHost m_thTabHost = null;
		/// <summary>
		/// Coefficient uses in panel dividing.
		/// </summary>
		private float m_fCoefficient = 0.5f;
		/// <summary>
		/// Unique name to indentify Tab hosts and layout panels.
		/// Used in serialization and deserialization.
		/// </summary>
		private string m_sUniqueName = String.Empty;
		/// <summary>
		/// Name of first Tab host.
		/// </summary>
		private string m_sTabHostOneName = String.Empty;
		/// <summary>
		/// Name of second Tab host.
		/// </summary>
		private string m_sTabHostTwoName = String.Empty;
		#endregion
	}
}