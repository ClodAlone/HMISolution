/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Runtime.InteropServices;
using OpcRcw.Comn;

namespace Opc.Ua.Com.Client
{
	/// <summary>
	/// A wrapper for the COM IEnumString interface.
	/// </summary>
	public class EnumString : IDisposable
	{	
		#region Constructors
		/// <summary>
		/// Initializes the object with an enumerator.
		/// </summary>
		public EnumString(object enumerator, int bufferLength)
		{
			m_enumerator = enumerator as IEnumString;

            if (m_enumerator == null)
            {
                throw new ArgumentNullException("enumerator", "Object does not support IEnumString interface.");
            }

            m_buffer   = new string[bufferLength];
            m_position = 0;
            m_fetched  = 0;
		}
		#endregion
		
		#region IDisposable Members
        /// <summary>
        /// The finializer implementation.
        /// </summary>
        ~EnumString() 
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {   
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            object enumerator = System.Threading.Interlocked.Exchange(ref m_enumerator, null);

            if (enumerator != null)
            {
			    ComUtils.ReleaseServer(enumerator);
            }
        }
        #endregion

		#region IEnumString Members
        /// <summary>
        /// Fetches the next string (returns null if no more data).
        /// </summary>
        public string Next()
        {
            // return null if at end of list.
            if (m_fetched == -1)
            {
                return null;
            }

            // see if next item is available.
            if (m_position < m_fetched)
            {
                return m_buffer[m_position++];
            }

            m_position = 0;

			try
			{
                // fetch next batch.
				m_fetched = 0;

                IntPtr pBuffer = Marshal.AllocCoTaskMem(IntPtr.Size*m_buffer.Length);
                
                try
                {
				    int error = m_enumerator.RemoteNext(m_buffer.Length, pBuffer, out m_fetched);

				    if (error < 0 || m_fetched == 0)
				    {
                        return null;
				    }

                    IntPtr[] pStrings = new IntPtr[m_fetched];
                    Marshal.Copy(pBuffer, pStrings, 0, m_fetched);

                    for (int ii = 0; ii < m_fetched; ii++)
		            {
                        m_buffer[ii] = Marshal.PtrToStringUni(pStrings[ii]);
                        Marshal.FreeCoTaskMem(pStrings[ii]);
		            }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pBuffer);
                }

                // check if end of list.
                if (m_fetched == 0)
                {
                    m_fetched = -1;
                    return null;
                }
                
                // return first item.
                return m_buffer[m_position++];
			}
			catch (Exception e)
			{
                if (ComUtils.IsUnknownError(e, ResultIds.E_FAIL))
                {
                    throw ComUtils.CreateException(e, "IEnumString.RemoteNext");
                }
				                    
				// some (incorrect) implementations return E_FAIL at the end of the list.
                m_fetched = -1;
                return null;
			}
        }

		/// <summary>
		/// Fetches the next group of strings. 
		/// </summary>
		public int Next(string[] buffer, int count)
		{
            // can't use simple interface after calling this method.
            m_fetched = -1;

			try
			{
				int fetched = 0;

                IntPtr pBuffer = Marshal.AllocCoTaskMem(IntPtr.Size*count);
                
                try
                {
				    int error = m_enumerator.RemoteNext(
					    count,
                        pBuffer,
					    out fetched);

                    if (error >= 0 && fetched > 0)
				    {
                        IntPtr[] pStrings = new IntPtr[m_fetched];
                        Marshal.Copy(pBuffer, pStrings, 0, fetched);

                        for (int ii = 0; ii < fetched; ii++)
		                {
                            m_buffer[ii] = Marshal.PtrToStringUni(pStrings[ii]);
                            Marshal.FreeCoTaskMem(pStrings[ii]);
		                }
                    }

                    return fetched;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pBuffer);
                }
			}
			catch (Exception e)
			{
				// some (incorrect) implementations return E_FAIL at the end of the list.
                if (Marshal.GetHRForException(e) == ResultIds.E_FAIL)
                {
				    return 0;
                }

                throw ComUtils.CreateException(e, "IEnumString.RemoteNext");
			}
		}

		/// <summary>
		/// Skips a number of strings.
		/// </summary>
		public void Skip(int count)
		{
			m_enumerator.Skip(count);

            // can't use simple interface after calling this method.
            m_fetched = -1;
		}

		/// <summary>
		/// Sets pointer to the start of the list.
		/// </summary>
		public void Reset()
		{
			m_enumerator.Reset();

            // can't use simple interface after calling this method.
            m_fetched = -1;
		}

		/// <summary>
		/// Clones the enumerator.
		/// </summary>
		public EnumString Clone()
		{
			IEnumString enumerator = null;
			m_enumerator.Clone(out enumerator);
			return new EnumString(enumerator, m_buffer.Length);
		}
		#endregion

		#region Private Members
		private OpcRcw.Comn.IEnumString m_enumerator;
        private string[] m_buffer;
        private int m_position;
        private int m_fetched;
		#endregion
	}
}
