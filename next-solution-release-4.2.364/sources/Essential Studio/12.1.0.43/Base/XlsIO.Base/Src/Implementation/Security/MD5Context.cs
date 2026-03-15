#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO.Implementation.Security
{
  /// <summary>
  /// Summary description for MD5_CTX.
  /// </summary>
  [CLSCompliant( false )]
  public class MD5Context
  {
    #region Class static members
    private static byte[] PADDING = new byte[]{
                                                0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                                              };
    #endregion

    #region Class constants
    private const uint DEF_MAGIC_1 = 0x67452301;
    private const uint DEF_MAGIC_2 = 0xefcdab89;
    private const uint DEF_MAGIC_3 = 0x98badcfe;
    private const uint DEF_MAGIC_4 = 0x10325476;

    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private uint[] m_uiI = new uint[ 2 ];
    /// <summary>
    /// scratch buffer
    /// </summary>
    private uint[] m_buf = new uint[ 4 ];
    /// <summary>
    /// input buffer
    /// </summary>
    private byte[] m_in = new byte[ 64 ];
    /// <summary>
    /// actual digest after MD5Final call
    /// </summary>
    private byte[] m_digest = new byte[ 16 ];
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets the I.
    /// </summary>
    /// <value>The I.</value>
    public uint[] I
    {
      get
      {
        return m_uiI;
      }
      set
      {
        m_uiI = value;
      }
    }
    /// <summary>
    /// Gets or sets the buffer.
    /// </summary>
    /// <value>The buffer.</value>
    public uint[] Buffer
    {
      get
      {
        return m_buf;
      }
      set
      {
        m_buf = value;
      }
    }
    /// <summary>
    /// Gets or sets the input buffer.
    /// </summary>
    /// <value>The input buffer.</value>
    public byte[] InBuffer
    {
      get
      {
        return m_in;
      }
      set
      {
        m_in = value;
      }
    }
    /// <summary>
    /// Gets or sets the digest.
    /// </summary>
    /// <value>The digest.</value>
    public byte[] Digest
    {
      get
      {
        return m_digest;
      }
      set
      {
        m_digest = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MD5Context"/> class.
    /// </summary>
    public MD5Context()
    {
      m_uiI[ 0 ] = m_uiI[ 1 ] = ( uint )0;
      /* Load magic initialization constants.*/
      m_buf[ 0 ] = DEF_MAGIC_1;
      m_buf[ 1 ] = DEF_MAGIC_2;
      m_buf[ 2 ] = DEF_MAGIC_3;
      m_buf[ 3 ] = DEF_MAGIC_4;
    }

    #endregion

    #region Class Public methods
    /// <summary>
    /// Updates
    /// </summary>
    /// <param name="inBuf">The input buf.</param>
    /// <param name="inLen">The buffer length.</param>
    public void Update( byte[] inBuf, uint inLen )
    {
      uint[] uia_in = new uint[ 16 ];
      int mdi = ( int )( ( m_uiI[ 0 ] >> 3 ) & 0x3F );

      if( ( m_uiI[ 0 ] + ( inLen << 3 ) ) < m_uiI[ 0 ] )
        m_uiI[ 1 ]++;

      m_uiI[ 0 ] += inLen << 3;
      m_uiI[ 1 ] += inLen >> 29;

      for( uint i = 0; i < inLen; i++ )
      {
        m_in[ mdi++ ] = inBuf[ i ];

        if( mdi == 0x40 )
        {
          for( uint j = 0, jj = 0; j < 16; j++, jj += 4 )
          {
            uia_in[ j ] = ( ( ( uint )m_in[ jj + 3 ] << 24 ) |
              ( ( uint )m_in[ jj + 2 ] << 16 ) |
              ( ( uint )m_in[ jj + 1 ] << 8 ) |
              m_in[ jj ] );
          }

          Transform( uia_in );
          mdi = 0;
        }
      }
    }

    /// <summary>
    /// Finals this instance.
    /// </summary>
    public void Final()
    {
      uint[] uia_in = new uint[ 16 ];
      uia_in[ 14 ] = m_uiI[ 0 ];
      uia_in[ 15 ] = m_uiI[ 1 ];
      uint mdi = ( m_uiI[ 0 ] >> 3 ) & 0x3F;
      uint padLen = ( mdi < 56 ) ? 56 - mdi : 120 - mdi;
      Update( PADDING, padLen );

      for( uint i = 0, ii = 0; i < 14; i++, ii += 4 )
      {
        uia_in[ i ] = ( ( ( uint )m_in[ ii + 3 ] ) << 24 ) |
          ( ( ( uint )m_in[ ii + 2 ] ) << 16 ) |
          ( ( ( uint )m_in[ ii + 1 ] ) << 8 ) |
          m_in[ ii ];
      }

      Transform( uia_in );
      StoreDigest();
    }
    /// <summary>
    /// Stores the digest.
    /// </summary>
    public void StoreDigest()
    {
      for( uint i = 0, ii = 0; i < 4; i++, ii += 4 )
      {
        m_digest[ ii ] = ( byte )( m_buf[ i ] & 0xFF );
        m_digest[ ii + 1 ] = ( byte )( ( m_buf[ i ] >> 8 ) & 0xFF );
        m_digest[ ii + 2 ] = ( byte )( ( m_buf[ i ] >> 16 ) & 0xFF );
        m_digest[ ii + 3 ] = ( byte )( ( m_buf[ i ] >> 24 ) & 0xFF );
      }
    }
    #endregion

    #region Class helper mephods
    /// <summary>
    /// F(x, y, z)
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="z">The z.</param>
    /// <returns></returns>
    private uint F( uint x, uint y, uint z )
    {
      return ( ( x & y ) | ( ( ~x ) & z ) );
    }

    /// <summary>
    /// G(x, y, z)
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="z">The z.</param>
    /// <returns></returns>
    private uint G( uint x, uint y, uint z )
    {
      return ( ( x & z ) | ( y & ( ~z ) ) );
    }
    /// <summary>
    /// H(x, y, z)
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="z">The z.</param>
    /// <returns></returns>
    private uint H( uint x, uint y, uint z )
    {
      return ( x ^ y ^ z );
    }
    /// <summary>
    /// I(x, y, z)
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="z">The z.</param>
    /// <returns></returns>
    private uint III( uint x, uint y, uint z )
    {
      return ( y ^ ( x | ( ~z ) ) );
    }
    /// <summary>
    /// ROTATE_LEFT
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    private uint ROTATE_LEFT( uint x, byte n )
    {
      return ( ( x << n ) | ( x >> ( 32 - n ) ) );
    }

    /// <summary>
    /// FF
    /// </summary>
    /// <param name="a">A.</param>
    /// <param name="b">The b.</param>
    /// <param name="c">The c.</param>
    /// <param name="d">The d.</param>
    /// <param name="x">The x.</param>
    /// <param name="s">The s.</param>
    /// <param name="ac">The ac.</param>
    private void FF( ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac )
    {
      a += F( b, c, d ) + x + ac;
      a = ROTATE_LEFT( a, s );
      a += b;
    }
    /// <summary>
    /// GG
    /// </summary>
    /// <param name="a">A.</param>
    /// <param name="b">The b.</param>
    /// <param name="c">The c.</param>
    /// <param name="d">The d.</param>
    /// <param name="x">The x.</param>
    /// <param name="s">The s.</param>
    /// <param name="ac">The ac.</param>
    private void GG( ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac )
    {
      a += G( b, c, d ) + x + ac;
      a = ROTATE_LEFT( a, s );
      a += b;
    }
    /// <summary>
    /// HH
    /// </summary>
    /// <param name="a">A.</param>
    /// <param name="b">The b.</param>
    /// <param name="c">The c.</param>
    /// <param name="d">The d.</param>
    /// <param name="x">The x.</param>
    /// <param name="s">The s.</param>
    /// <param name="ac">The ac.</param>
    private void HH( ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac )
    {
      a += H( b, c, d ) + x + ac;
      a = ROTATE_LEFT( a, s );
      a += b;
    }
    /// <summary>
    /// II
    /// </summary>
    /// <param name="a">A.</param>
    /// <param name="b">The b.</param>
    /// <param name="c">The c.</param>
    /// <param name="d">The d.</param>
    /// <param name="x">The x.</param>
    /// <param name="s">The s.</param>
    /// <param name="ac">The ac.</param>
    private void II( ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac )
    {
      a += III( b, c, d ) + x + ac;
      a = ROTATE_LEFT( a, s );
      a += b;
    }
    /// <summary>
    /// Transforms the specified inn.
    /// </summary>
    /// <param name="inn">The inn.</param>
    private void Transform( uint[] inn )
    {
      uint a = m_buf[ 0 ];
      uint b = m_buf[ 1 ];
      uint c = m_buf[ 2 ];
      uint d = m_buf[ 3 ];
      /* Round 1 */
      const byte S11 = 7;
      const byte S12 = 12;
      const byte S13 = 17;
      const byte S14 = 22;
      FF( ref a, b, c, d, inn[ 0 ], S11, 3614090360 ); /* 1  */
      FF( ref d, a, b, c, inn[ 1 ], S12, 3905402710 ); /* 2  */
      FF( ref c, d, a, b, inn[ 2 ], S13, 606105819 ); /* 3  */
      FF( ref b, c, d, a, inn[ 3 ], S14, 3250441966 ); /* 4  */
      FF( ref a, b, c, d, inn[ 4 ], S11, 4118548399 ); /* 5  */
      FF( ref d, a, b, c, inn[ 5 ], S12, 1200080426 ); /* 6  */
      FF( ref c, d, a, b, inn[ 6 ], S13, 2821735955 ); /* 7  */
      FF( ref b, c, d, a, inn[ 7 ], S14, 4249261313 ); /* 8  */
      FF( ref a, b, c, d, inn[ 8 ], S11, 1770035416 ); /* 9  */
      FF( ref d, a, b, c, inn[ 9 ], S12, 2336552879 ); /* 10 */
      FF( ref c, d, a, b, inn[ 10 ], S13, 4294925233 ); /* 11 */
      FF( ref b, c, d, a, inn[ 11 ], S14, 2304563134 ); /* 12 */
      FF( ref a, b, c, d, inn[ 12 ], S11, 1804603682 ); /* 13 */
      FF( ref d, a, b, c, inn[ 13 ], S12, 4254626195 ); /* 14 */
      FF( ref c, d, a, b, inn[ 14 ], S13, 2792965006 ); /* 15 */
      FF( ref b, c, d, a, inn[ 15 ], S14, 1236535329 ); /* 16 */
      /* Round 2 */
      const byte S21 = 5;
      const byte S22 = 9;
      const byte S23 = 14;
      const byte S24 = 20;
      GG( ref a, b, c, d, inn[ 1 ], S21, 4129170786 );	/* 17 */
      GG( ref d, a, b, c, inn[ 6 ], S22, 3225465664 );	/* 18 */
      GG( ref c, d, a, b, inn[ 11 ], S23, 643717713 );	/* 19 */
      GG( ref b, c, d, a, inn[ 0 ], S24, 3921069994 );	/* 20 */
      GG( ref a, b, c, d, inn[ 5 ], S21, 3593408605 );	/* 21 */
      GG( ref d, a, b, c, inn[ 10 ], S22, 38016083 );	/* 22 */
      GG( ref c, d, a, b, inn[ 15 ], S23, 3634488961 );	/* 23 */
      GG( ref b, c, d, a, inn[ 4 ], S24, 3889429448 );	/* 24 */
      GG( ref a, b, c, d, inn[ 9 ], S21, 568446438 );	/* 25 */
      GG( ref d, a, b, c, inn[ 14 ], S22, 3275163606 );	/* 26 */
      GG( ref c, d, a, b, inn[ 3 ], S23, 4107603335 );	/* 27 */
      GG( ref b, c, d, a, inn[ 8 ], S24, 1163531501 );	/* 28 */
      GG( ref a, b, c, d, inn[ 13 ], S21, 2850285829 );	/* 29 */
      GG( ref d, a, b, c, inn[ 2 ], S22, 4243563512 );	/* 30 */
      GG( ref c, d, a, b, inn[ 7 ], S23, 1735328473 );	/* 31 */
      GG( ref b, c, d, a, inn[ 12 ], S24, 2368359562 );	/* 32 */
      /* Round 3 */
      const byte S31 = 4;
      const byte S32 = 11;
      const byte S33 = 16;
      const byte S34 = 23;
      HH( ref a, b, c, d, inn[ 5 ], S31, 4294588738 );	/* 33 */
      HH( ref d, a, b, c, inn[ 8 ], S32, 2272392833 );	/* 34 */
      HH( ref c, d, a, b, inn[ 11 ], S33, 1839030562 );	/* 35 */
      HH( ref b, c, d, a, inn[ 14 ], S34, 4259657740 );	/* 36 */
      HH( ref a, b, c, d, inn[ 1 ], S31, 2763975236 );	/* 37 */
      HH( ref d, a, b, c, inn[ 4 ], S32, 1272893353 );	/* 38 */
      HH( ref c, d, a, b, inn[ 7 ], S33, 4139469664 );	/* 39 */
      HH( ref b, c, d, a, inn[ 10 ], S34, 3200236656 );	/* 40 */
      HH( ref a, b, c, d, inn[ 13 ], S31, 681279174 );	/* 41 */
      HH( ref d, a, b, c, inn[ 0 ], S32, 3936430074 );	/* 42 */
      HH( ref c, d, a, b, inn[ 3 ], S33, 3572445317 );	/* 43 */
      HH( ref b, c, d, a, inn[ 6 ], S34, 76029189 );	/* 44 */
      HH( ref a, b, c, d, inn[ 9 ], S31, 3654602809 );	/* 45 */
      HH( ref d, a, b, c, inn[ 12 ], S32, 3873151461 );	/* 46 */
      HH( ref c, d, a, b, inn[ 15 ], S33, 530742520 );	/* 47 */
      HH( ref b, c, d, a, inn[ 2 ], S34, 3299628645 );	/* 48 */
      /* Round 4 */
      const byte S41 = 6;
      const byte S42 = 10;
      const byte S43 = 15;
      const byte S44 = 21;
      II( ref a, b, c, d, inn[ 0 ], S41, 4096336452 );	/* 49 */
      II( ref d, a, b, c, inn[ 7 ], S42, 1126891415 );	/* 50 */
      II( ref c, d, a, b, inn[ 14 ], S43, 2878612391 );	/* 51 */
      II( ref b, c, d, a, inn[ 5 ], S44, 4237533241 );	/* 52 */
      II( ref a, b, c, d, inn[ 12 ], S41, 1700485571 );	/* 53 */
      II( ref d, a, b, c, inn[ 3 ], S42, 2399980690 );	/* 54 */
      II( ref c, d, a, b, inn[ 10 ], S43, 4293915773 );	/* 55 */
      II( ref b, c, d, a, inn[ 1 ], S44, 2240044497 );	/* 56 */
      II( ref a, b, c, d, inn[ 8 ], S41, 1873313359 );	/* 57 */
      II( ref d, a, b, c, inn[ 15 ], S42, 4264355552 );	/* 58 */
      II( ref c, d, a, b, inn[ 6 ], S43, 2734768916 );	/* 59 */
      II( ref b, c, d, a, inn[ 13 ], S44, 1309151649 );	/* 60 */
      II( ref a, b, c, d, inn[ 4 ], S41, 4149444226 );	/* 61 */
      II( ref d, a, b, c, inn[ 11 ], S42, 3174756917 );	/* 62 */
      II( ref c, d, a, b, inn[ 2 ], S43, 718787259 );	/* 63 */
      II( ref b, c, d, a, inn[ 9 ], S44, 3951481745 );	/* 64 */

      m_buf[ 0 ] += a;
      m_buf[ 1 ] += b;
      m_buf[ 2 ] += c;
      m_buf[ 3 ] += d;
    }

    #endregion
  }
}
