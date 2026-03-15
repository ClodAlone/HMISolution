using System;
using System.Text;
using System.Net;
using System.IO;

#if !XNA
using System.Threading;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
#endif
#endif

#if SILVERLIGHT
using System.Net.Browser;
#elif !XNA
#endif

#if XNA || WINDOWS_PHONE
using Microsoft.Xna.Framework.Graphics;
#endif

namespace MjpegProcessor
{
    public class MjpegDecoder : IDisposable
	{
#if !SILVERLIGHT && !XNA && !NET_STANDARD
        // WinForms & WPF
        BitmapImage bi;
#endif
        object operationsLock = new object();
#if !NET_STANDARD
        DispatcherOperation dpNewFrame;
#endif
        byte[] lastFrame;

        // magic 2 byte header for JPEG images
        private byte[] JpegHeader = new byte[] { 0xff, 0xd8 };

		// pull down 1024 bytes at a time
		private const int ChunkSize = 1024;

		// used to cancel reading the stream
		private bool _streamActive;

		// current encoded JPEG image
		public byte[] CurrentFrame { get; private set; }
#if !NET_STANDARD
        public Dispatcher Dispatcher
        {
            get;
            set;
        }
#endif
        ManualResetEvent allDone = new ManualResetEvent(false);
        RequestState myRequestState = new RequestState();
        bool bAborting = false;
        bool bSkipNextFrame = false;
        int nextFrameFaultToleranceFactor = 5;
        TimeSpan defaultClosingTimeout = new TimeSpan(0, 0, 1);
        string sourceId;
#if !XNA && !NET_STANDARD
        // WPF and Silverlight
        // event to get the buffer above handed to you
        public event EventHandler<FrameReadyEventArgs> FrameReady;
		public event EventHandler<ErrorEventArgs> Error;
#endif
#if NET_STANDARD
        public event EventHandler<WebHMIFrameReadyEventArgs> WebHMIFrameReady;
        public event EventHandler<WebHMIErrorEventArgs> Error;
#endif

        public MjpegDecoder(TimeSpan? closeTimeout = null, string sourceId = null)
		{
            this.sourceId = sourceId;

            if (closeTimeout != null)
                defaultClosingTimeout = (TimeSpan)closeTimeout;
        }

		public void ParseStream(Uri uri)
		{
			ParseStream(uri, null, null);
		}

		public void ParseStream(Uri uri, string username, string password)
		{
#if SILVERLIGHT
			HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
#endif
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
			if(!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
				request.Credentials = new NetworkCredential(username, password);

#if SILVERLIGHT
			// start the stream immediately
			request.AllowReadStreamBuffering = false;
#endif
            myRequestState.request = request;

            // asynchronously get a response
            allDone.Reset();
			request.BeginGetResponse(OnGetResponse, myRequestState);
		}

#if XNA || WINDOWS_PHONE
		public Texture2D GetMjpegFrame(GraphicsDevice graphicsDevice)
		{
			// create a Texture2D from the current byte buffer
			if(CurrentFrame != null)
				return Texture2D.FromStream(graphicsDevice, new MemoryStream(CurrentFrame, 0, CurrentFrame.Length));
			return null;
		}
#endif
		private void OnGetResponse(IAsyncResult asyncResult)
        {
            if (bAborting)
            {
                try
                {
                    allDone.Set();
                }
                catch (Exception)
                {

                }
                return;
            }
            byte[] imageBuffer = new byte[1024 * 1024 * 5];
            Stream s;

            try
            {
                // get the response
                RequestState req = (RequestState)asyncResult.AsyncState;
                myRequestState.response = (HttpWebResponse)req.request.EndGetResponse(asyncResult);

                // find our magic boundary value
                string contentType = myRequestState.response.Headers["Content-Type"];
                if (!string.IsNullOrEmpty(contentType) && !contentType.Contains("="))
                    throw new Exception("Invalid content-type header.  The camera is likely not returning a proper MJPEG stream.");
                string boundary = myRequestState.response.Headers["Content-Type"].Split('=')[1].Replace("\"", "");
                byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

                s = myRequestState.response.GetResponseStream();
                BinaryReader br = new BinaryReader(s);

                _streamActive = true;

                byte[] buff = br.ReadBytes(ChunkSize);

                while (_streamActive)
                {
                    int size;

                    // find the JPEG header
                    int imageStart = buff.Find(JpegHeader);

                    if (imageStart != -1)
                    {
                        // copy the start of the JPEG image to the imageBuffer
                        size = buff.Length - imageStart;
                        Array.Copy(buff, imageStart, imageBuffer, 0, size);

                        while (true)
                        {
                            buff = br.ReadBytes(ChunkSize);

                            // find the boundary text
                            int imageEnd = buff.Find(boundaryBytes);
                            if (imageEnd != -1)
                            {
                                if (bSkipNextFrame)
                                {
                                    bSkipNextFrame = false;
                                    var byteRead = 0;
                                    var byteTolerance = imageBuffer.Length * nextFrameFaultToleranceFactor;
                                    while (buff.Find(JpegHeader) == -1 || byteRead >= byteTolerance)
                                    {
                                        buff = br.ReadBytes(ChunkSize);
                                        byteRead += ChunkSize;
                                    }
                                    if (byteRead >= byteTolerance)
                                        throw new Exception("Frame stream was interrupted: next frame header not found");
                                    break;
                                }
                                // copy the remainder of the JPEG to the imageBuffer
                                Array.Copy(buff, 0, imageBuffer, size, imageEnd);
                                size += imageEnd;

                                // create a single JPEG frame
                                byte[] frame = new byte[size];
                                Array.Copy(imageBuffer, 0, frame, 0, size);
#if !XNA
                                ProcessFrame(frame);
#endif
                                // copy the leftover data to the start
                                Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

                                // fill the remainder of the buffer with new data and start over
                                byte[] temp = br.ReadBytes(imageEnd);

                                Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
                                break;
                            }

                            // copy all of the data to the imageBuffer
                            try
                            {
                                Array.Copy(buff, 0, imageBuffer, size, buff.Length);
                                size += buff.Length;
                            }
                            catch (ArgumentException ex) //frame buffer (imageBuffer) too small
                            {
                                if (imageEnd == -1 && !bSkipNextFrame) //frame scheduled for being skipped
                                    bSkipNextFrame = true; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if !NET_STANDARD
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action<ErrorEventArgs>) delegate (ErrorEventArgs errMsg) {
                    Error?.Invoke(this, errMsg);
                }, new ErrorEventArgs(ex));
#else
                Error?.Invoke(this, new WebHMIErrorEventArgs(ex, sourceId));
#endif
            }
            finally
            {
                if (myRequestState.response != null)
                    myRequestState.response.Close();
                try
                {
                    allDone.Set();
                }
                catch (Exception)
                {

                }
            }
		}

		private void ProcessFrame(byte[] newFrame)
		{
            // get it on the UI thread
            bool bForceDispatcherOperation = false;
            lock (operationsLock)
            {
                bForceDispatcherOperation = lastFrame == null;
                lastFrame = newFrame;
            }
#if !NET_STANDARD
            if (dpNewFrame == null || bForceDispatcherOperation ||
                    dpNewFrame.Status == DispatcherOperationStatus.Completed ||
                    dpNewFrame.Status == DispatcherOperationStatus.Aborted)
            {
                dpNewFrame = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) delegate {
#endif
                    if (!_streamActive)
                            return;

                        byte[] lastFrameCopy = null;
                        lock (operationsLock)
                        {
                            lastFrameCopy = new byte[lastFrame.Length];
                            lastFrame.CopyTo(lastFrameCopy, 0);
                            lastFrame = null;
                        }
#if !NET_STANDARD
                    bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = new MemoryStream(lastFrameCopy);
                        bi.EndInit();
                        bi.Freeze();

                        FrameReady?.Invoke(this, new FrameReadyEventArgs { BitmapImage = bi });
                });
            }
#else
            WebHMIFrameReady?.Invoke(this, new WebHMIFrameReadyEventArgs { sourceId = sourceId, FrameBase64 = Convert.ToBase64String(lastFrameCopy) });
#endif
        }

        public void Dispose()
        {
            _streamActive = false;
            if (!allDone.WaitOne(defaultClosingTimeout))
            {
                if (myRequestState.request != null)
                {
                    bAborting = true; //Abort() calls synchronously the "BeginGetResponse" callback
                    myRequestState.request.Abort();
                }
            }
            if (myRequestState.response != null)
                myRequestState.response.Close();
            allDone.Close();
        }
    }

    public class RequestState
    {
        // This class stores the State of the request.
        public HttpWebRequest request;
        public HttpWebResponse response;
    }

    public static class Extensions
	{
		public static int Find(this byte[] buff, byte[] search)
		{
			// enumerate the buffer but don't overstep the bounds
			for(int start = 0; start < buff.Length - search.Length; start++)
			{
				// we found the first character
				if(buff[start] == search[0])
				{
					int next;

					// traverse the rest of the bytes
					for(next = 1; next < search.Length; next++)
					{
						// if we don't match, bail
						if(buff[start+next] != search[next])
							break;
					}

					if(next == search.Length)
						return start;
				}
			}
			// not found
			return -1;	
		}
	}

#if !NET_STANDARD
    public class FrameReadyEventArgs : EventArgs
	{
#if !XNA
		public BitmapImage BitmapImage;
#endif
	}
#else
    public class WebHMIFrameReadyEventArgs : EventArgs
    {
        public string FrameBase64;
        public string sourceId;
    }

    public class WebHMIErrorEventArgs : ErrorEventArgs
    {
        public string sourceId;
        public WebHMIErrorEventArgs(Exception exception, string sourceId) : base(exception)
        {
            this.sourceId = sourceId;
        }
    }
#endif
}
