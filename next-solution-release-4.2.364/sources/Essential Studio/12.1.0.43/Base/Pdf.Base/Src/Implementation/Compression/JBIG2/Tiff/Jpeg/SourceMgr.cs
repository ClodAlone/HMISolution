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
using System.Globalization;

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>
    /// Data source object for decompression.
    /// </summary>
    abstract class SourceMgr
    {
        private byte[] m_next_input_byte;
        private int m_bytes_in_buffer;
        private int m_position;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void init_source();

        /// <summary>
        /// Fills input buffer
        /// </summary>
        public abstract bool fill_input_buffer();

        /// <summary>
        /// Initializes the internal buffer.
        /// </summary>
        protected void initInternalBuffer(byte[] buffer, int size)
        {
            m_bytes_in_buffer = size;
            m_next_input_byte = buffer;
            m_position = 0;
        }

        /// <summary>
        /// Skip data - used to skip over a potentially large amount of
        /// uninteresting data (such as an APPn marker).
        /// </summary>
        public virtual void skip_input_data(int num_bytes)
        {
            if (num_bytes > 0)
            {
                while (num_bytes > m_bytes_in_buffer)
                {
                    num_bytes -= m_bytes_in_buffer;
                    fill_input_buffer();
                }

                m_position += num_bytes;
                m_bytes_in_buffer -= num_bytes;
            }
        }
        
        /// <summary>
        /// This is the default resync_to_restart method for data source 
        /// managers to use if they don't have any better approach.
        /// </summary>
        public virtual bool resync_to_restart(DecompressStruct cinfo, int desired)
        {
            int action = 1;
            for ( ; ; )
            {
                if (cinfo.m_unread_marker < (int)JPEG_MARKER.SOF0)
                {
                    action = 2;
                }
                else if (cinfo.m_unread_marker < (int)JPEG_MARKER.RST0 ||
                    cinfo.m_unread_marker > (int)JPEG_MARKER.RST7)
                {
                    action = 3;
                }
                else
                {
                    if (cinfo.m_unread_marker == ((int)JPEG_MARKER.RST0 + ((desired + 1) & 7))
                        || cinfo.m_unread_marker == ((int)JPEG_MARKER.RST0 + ((desired + 2) & 7)))
                    {
                        action = 3;
                    }
                    else if (cinfo.m_unread_marker == ((int)JPEG_MARKER.RST0 + ((desired - 1) & 7)) ||
                        cinfo.m_unread_marker == ((int)JPEG_MARKER.RST0 + ((desired - 2) & 7)))
                    {
                        action = 2;
                    }
                    else
                    {
                        action = 1;
                    }
                }

                switch (action)
                {
                    case 1:
                        cinfo.m_unread_marker = 0;
                        return true;
                    case 2:
                        if (!cinfo.m_marker.next_marker())
                            return false;
                        break;
                    case 3:
                        return true;
                }
            }
        }

        /// <summary>
        /// Terminate source - called by jpeg_finish_decompress
        /// after all data has been read.  Often a no-op.
        /// </summary>
        public virtual void term_source()
        {
        }

        /// <summary>
        /// Reads two bytes interpreted as an unsigned 16-bit integer.
        /// </summary>
        public virtual bool GetTwoBytes(out int V)
        {
            if (!MakeByteAvailable())
            {
                V = 0;
                return false;
            }

            m_bytes_in_buffer--;
            V = m_next_input_byte[m_position] << 8;
            m_position++;

            if (!MakeByteAvailable())
                return false;

            m_bytes_in_buffer--;
            V += m_next_input_byte[m_position];
            m_position++;
            return true;
        }

        /// <summary>
        /// Read a byte into variable V.
        /// If must suspend, take the specified action (typically "return false").
        /// </summary>
        public virtual bool GetByte(out int V)
        {
            if (!MakeByteAvailable())
            {
                V = 0;
                return false;
            }

            m_bytes_in_buffer--;
            V = m_next_input_byte[m_position];
            m_position++;
            return true;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        public virtual int GetBytes(byte[] dest, int amount)
        {
            int avail = amount;
            if (avail > m_bytes_in_buffer)
                avail = m_bytes_in_buffer;

            for (int i = 0; i < avail; i++)
            {
                dest[i] = m_next_input_byte[m_position];
                m_position++;
                m_bytes_in_buffer--;
            }

            return avail;
        }

        /// <summary>
        /// Functions for fetching data from the data source module.
        /// </summary>
        public virtual bool MakeByteAvailable()
        {
            if (m_bytes_in_buffer == 0)
            {
                if (!fill_input_buffer())
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The progress monitor object.
    /// </summary>
    class ProgressMgr
    {
        private int m_passCounter;
        private int m_passLimit;
        private int m_completedPasses;
        private int m_totalPasses;

        /// <summary>
        /// Occurs when progress is changed.
        /// </summary>
        public event EventHandler OnProgress;

        /// <summary>
        /// Gets or sets the number of work units completed in this pass.
        /// </summary>
        public int Pass_counter
        {
            get { return m_passCounter; }
            set { m_passCounter = value; }
        }

        /// <summary>
        /// Gets or sets the total number of work units in this pass.
        /// </summary>
        public int Pass_limit
        {
            get { return m_passLimit; }
            set { m_passLimit = value; }
        }

        /// <summary>
        /// Gets or sets the number of passes completed so far.
        /// </summary>
        public int Completed_passes
        {
            get { return m_completedPasses; }
            set { m_completedPasses = value; }
        }

        /// <summary>
        /// Gets or sets the total number of passes expected.
        /// </summary>
        public int Total_passes
        {
            get { return m_totalPasses; }
            set { m_totalPasses = value; }
        }

        /// <summary>
        /// Indicates that progress was changed.
        /// </summary>
        public void Updated()
        {
            if (OnProgress != null)
                OnProgress(this, new EventArgs());
        }
    }
}
