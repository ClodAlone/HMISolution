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

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Diagnostics
{
	/// <summary>
	/// Provides various diagnostic utilities for tracing methods, exception and more.
	/// </summary>
	public sealed class TraceUtil
	{
		private TraceUtil()
		{
		}

		static TextWriterTraceListener textListener;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public static void StartTraceFile(string fileName)
		{
			FileStream file;
			try
			{
				file = File.Create(fileName);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null, ex))
					throw;
				return;
			}

			/* Create a new text writer using the output stream and add it to
								   the trace listeners. */
			textListener = new TextWriterTraceListener(file);
			Trace.Listeners.Add(textListener);
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public static void CloseTraceFile()
		{
			if (textListener != null)
			{
				Trace.Listeners.Remove(textListener);
				textListener.Flush();
				textListener.Close();
				textListener = null;
			}
		}


		/// <summary>
		/// Writes a trace log for the given exception together with information where the exception was caught.
		/// </summary>
		/// <param name="e">An Exception.</param>
		[DebuggerStepThrough()]
		public static void TraceExceptionCatched(Exception e)
		{
			StackTrace st = new StackTrace(1, true);
			StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
			if (sf != null)
			{
				MethodBase mb = sf.GetMethod();
				Trace.WriteLine(e.ToString());
				StringBuilder sb = new StringBuilder();
				sb.Append("catched at ");
				sb.Append(mb.ReflectedType.FullName);
				sb.Append(".");
				sb.Append(mb.Name);
				sb.Append("(");
				bool first = true;
				foreach (ParameterInfo pi in mb.GetParameters())
				{
					if (!first)
						sb.Append(", ");
					first = false;
					sb.Append(pi.ParameterType.Name);
					sb.Append(" ");
					sb.Append(pi.Name);
				}   
				sb.Append(") in ");
				sb.Append(sf.GetFileName());
				sb.Append(":line ");
				sb.Append(sf.GetFileLineNumber());
				Console.WriteLine(sb.ToString());
				// throw e;
				// Debug.Assert(false, e.Message, sb.ToString());
			}
		}

		/// <summary>
		///  Indicates whether a given procedure is being called by another procedure.
		/// </summary>
		/// <param name="method">The method to be looked up in the stack trace.</param>
		/// <returns>True if method was found; false otherwise.</returns>
		/// <example>
		/// <code lang="C#">
		/// if (TraceUtil.IsCalledFrom(typeof(Form1).GetMethod("Form1_Load", BindingFlags.NonPublic|BindingFlags.Instance)))
		///		Debugger.Break()
		///	</code>
		/// </example>
		public static bool IsCalledFrom(MethodBase method)
		{
			StackTrace st = new StackTrace(true);
			for (int i = 1; i < st.FrameCount; i++)
			{
				StackFrame sf = st.GetFrame(i);
				if (sf.GetMethod().Equals(method))
					return true;
			}
			return false;
		}

		/// <overload>
		/// Writes a trace log of the current stack.
		/// </overload>
		/// <summary>
		/// Writes a trace log of the current stack.
		/// </summary>
		[Conditional("DEBUG")] 
		[DebuggerStepThrough()]
		public static void TraceCalledFrom()
		{
			TraceCalledFrom(3);
		}

		/// <summary>
		/// Writes a trace log with information about current class and method name and
		/// string representations of any method arguments if the condition is true.
		/// </summary>
		/// <param name="condition">Indicates whether to skip or write the log.</param>
		/// <param name="args">An array of method arguments.</param>
		/// <param name="levels">The number of levels to check in call stack.</param>
		/// <example>The following method shows typical usage of this diagnostic method.
		/// <code lang="C#">
		/// private void OnTimerElapsed(object source, ElapsedEventArgs e)
		/// {
		/// 	TraceUtil.TraceCalledFromIf(Switches.Timers.TraceVerbose, 3);
		/// }
		/// </code>
		/// </example>
		[Conditional("DEBUG")] 
		[DebuggerStepThrough()]
		public static void TraceCalledFromIf(bool condition, int levels, params object[] args)
		{
			if (condition)
			{
				if (args != null && args.Length > 0)
				{
					StackTrace st = new StackTrace(1, true);
					StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
					if (sf != null)
					{
						MethodBase mb = sf.GetMethod();
						TraceMethodCall(mb, args);
					}
				}
				_TraceCalledFrom(levels);
			}		
		}


		
		/// <summary>
		/// Writes a trace log of the current stack.
		/// </summary>
		/// <param name="levels">The number of method on the stack to trace.</param>
		[Conditional("DEBUG")] 
		[DebuggerStepThrough()]
		public static void TraceCalledFrom(int levels)
		{
			_TraceCalledFrom(levels);
		}
		
		[Conditional("DEBUG")] 
		[DebuggerStepThrough()]
		static void _TraceCalledFrom(int levels)
		{
			//levels++;
			StackTrace st = new StackTrace(3, true);
			for (int n = 0; n < levels+1; n++)
			{
				StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP+n);
				if (sf != null)
				{
					MethodBase mb = sf.GetMethod();
					StringBuilder sb = new StringBuilder();
					if (levels > 1)
						sb.Append(n.ToString());
					sb.Append(": called from ");
					sb.Append(mb.ReflectedType.Name);
					sb.Append(".");
					sb.Append(mb.Name);
					sb.Append("(");
					bool first = true;
					foreach (ParameterInfo pi in mb.GetParameters())
					{
						if (!first)
							sb.Append(", ");
						first = false;
						sb.Append(pi.ParameterType.Name);
						//					sb.Append(" ");
						//					sb.Append(pi.Name);
					}   
					sb.Append(") in ");
					sb.Append(Path.GetFileName(sf.GetFileName()));
					sb.Append(":line ");
					sb.Append(sf.GetFileLineNumber());
					Trace.WriteLine(sb.ToString());
					// throw e;
					// Debug.Assert(false, e.Message, sb.ToString());
				}
			}
		}

		/// <summary>
		/// Writes a trace log with information about current class and method name and
		/// string representations of any method arguments.
		/// </summary>
		/// <param name="args">An array of method arguments.</param>
		[Conditional("DEBUG")] 
		[DebuggerStepThrough()]
		public static void TraceCurrentMethodInfo(params object[] args)
		{
			StackTrace st = new StackTrace(1, true);
			StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
			if (sf != null)
			{
				MethodBase mb = sf.GetMethod();
				TraceMethodCall(mb, args);
			}
		}

		/// <summary>
		/// Writes a trace log with information about current class and method name and
		/// string representations of any method arguments if the condition is true.
		/// </summary>
		/// <param name="condition">Indicates whether to skip or write the log.</param>
		/// <param name="args">An array of method arguments.</param>
		/// <example>The following method shows typical usage of this diagnostic method.
		/// <code lang="C#">
		/// private void OnTimerElapsed(object source, ElapsedEventArgs e)
		/// {
		/// 	TraceUtil.TraceCurrentMethodInfoIf(Switches.Timers.TraceVerbose);
		/// }
		/// </code>
		/// </example>
		[Conditional("DEBUG")] 
		[DebuggerStepThrough()]
		public static void TraceCurrentMethodInfoIf(bool condition, params object[] args)
		{
			if (condition)
			{
				StackTrace st = new StackTrace(1, true);
				StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
				if (sf != null)
				{
					MethodBase mb = sf.GetMethod();
					TraceMethodCall(mb, args);
				}
			}		
		}

		static Hashtable switchTable = new Hashtable();

		internal static void TraceMethodCall(Type type, string methodName, params object[] args)
		{
			int n = methodName.LastIndexOf(".");
			if (n != -1)
			{
				string typeName = type.Name;
				methodName = methodName.Substring(n+1);

				if (type != null)
				{
					Type[] argumentTypes = new Type[args.Length];
					for (int i = 0; i < args.Length; i++)
						argumentTypes[i] = args[i] == null ? typeof(object) : args[i].GetType();
					MethodInfo mi = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic, null, argumentTypes, null);
					if (mi != null)
					{
						TraceMethodCall(mi, args);
						return;
					}
					Trace.WriteLine(typeName + ":" + methodName);
					return;
				}
			}
			Trace.WriteLine(methodName);
		}

		[DebuggerStepThrough()]
		internal static void TraceMethodCall(MethodBase mb, params object[] args)
		{
			TraceMethodCallWithNames(mb, false, args);
		}
		
		[DebuggerStepThrough()]
		internal static void TraceMethodCallWithNames(MethodBase mb, bool showNames, params object[] args)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(mb.ReflectedType.Name);
			sb.Append(".");
			sb.Append(mb.Name);

			string methodName = sb.ToString();
			BooleanSwitch methodSwitch = switchTable[methodName] as BooleanSwitch;
			if (methodSwitch == null)
			{
				methodSwitch = new BooleanSwitch(methodName, "");
				switchTable.Add(methodName, methodSwitch);
			}
			if (methodSwitch.Enabled)
				return;


			sb.Append("(");
			ParameterInfo[] parameters = mb.GetParameters();
			for (int n = 0; n < parameters.Length; n++)
			{
				ParameterInfo pi = parameters[n];
				if (n > 0)
					sb.Append(", ");
				if (n < args.Length)
				{
					if (showNames)
						sb.Append(pi.Name + " = ");
					sb.Append(args[n] != null ? args[n].ToString().Trim() : "null");
				}
			}   

			for (int n = parameters.Length; n < args.Length; n++)
			{
//				if (sb.Length > 50)
//				{
//					Trace.WriteLine(sb.ToString());
//					sb = new StringBuilder();
//				}
				if (n > 0)
					sb.Append(", ");
				if (args[n] != null)
				{
					if (showNames)
						sb.Append(args[n].GetType().Name + " = ");
					sb.Append(args[n] != null ? args[n].ToString().Trim() : "null");
				}
				else
					sb.Append("null");
			}
			sb.Append(")");
			Trace.WriteLine(sb.ToString());
		}


		[DebuggerStepThrough()]
		internal static void TraceStack()
		{
			StackTrace st = new StackTrace(1, true);
			StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
			if (sf != null)
			{
				MethodBase mb = sf.GetMethod();
				StringBuilder sb = new StringBuilder();
				sb.Append("File ");
				sb.Append(sf.GetFileName());
				sb.Append(", line ");
				sb.Append(sf.GetFileLineNumber());
				Trace.WriteLine(sb.ToString());
				sb = new StringBuilder();
				sb.Append("in method ");
				sb.Append(mb.ReflectedType.Name);
				sb.Append(".");
				sb.Append(mb.Name);
				sb.Append("(");
				bool first = true;
				foreach (ParameterInfo pi in mb.GetParameters())
				{
					if (!first)
						sb.Append(", ");
					first = false;
					sb.Append(pi.ParameterType.Name);
					sb.Append(" ");
					sb.Append(pi.Name);
				}   
				sb.Append(");");
				Trace.WriteLine(sb.ToString());
				Trace.WriteLine(sf.ToString());
			}
		}
	}

	/// <summary>
	/// Provides a diagnostic utility for measuring performance. 
	/// </summary>
	/// <example>The following example display a message box with performance info.
	/// <code lang="C#">
	/// 		private void button1_Click(object sender, System.EventArgs e)
	/// 		{
	/// 			int rows = (int) this.numericUpDown1.Value;
	/// 
	/// 			this.gridControl1.theData = new VirtGrid.VirtData(rows, 20);
	/// 			this.gridControl1.Refresh();
	/// 
	/// 			using (Syncfusion.Diagnostics.MeasureTime.Measure("gridControl1.TopRowIndex = 500000"))
	/// 			{
	/// 				this.gridControl1.TopRowIndex = 5000000;
	/// 			}
	/// 
	/// 			MessageBox.Show(Syncfusion.Diagnostics.MeasureTime.DumpTimes());
	/// 		}
	/// </code>
	/// </example>
	public class MeasureTime : IDisposable
	{
		static Hashtable times = new Hashtable();
        #region Externals
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern int QueryPerformanceFrequency(ref Int64 lpFrequency);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern int QueryPerformanceCounter(ref Int64 lpPerformanceCount);
        #endregion

        
        /// <summary>
        /// Returns the time since Reset in microseconds.
        /// </summary>
        public Int64 TickCount
        {
            get
            {
                Int64 m_LastCount = 0;
                QueryPerformanceCounter(ref m_LastCount);
                return m_LastCount;
            }
        }


		Int64 ticks = 0;
		string id = "";

		MeasureTime(string id)
		{
			this.id = id;
			ticks = TickCount;
		}

		/// <summary>
		///     Insert this before a code block that should be measured.
		/// </summary>
		public static MeasureTime Measure(string id)
		{
			return new MeasureTime(id);
		}

		/// <summary>
		///     Ends the code block that should be measured and increments the associated performance counter.
		/// </summary>
		public void Dispose()
		{
            Int64 time = TickCount - ticks;
			if (times.ContainsKey(id))
				times[id] = ((Int64) times[id]) + time;
			else
				times[id] = time;
		}

        private static long GetFrequency()
        {
            long ret = 0;
            if (QueryPerformanceFrequency(ref ret) == 0)
                throw new NotSupportedException("Error while querying the performance counter frequency.");
            return ret;
        }

		/// <summary>
		///     Prints all performance counters into a string and sorts it by ids. All counters will be reset afterwards.
		/// </summary>
		public static string DumpTimes()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("MeasureTime results:");
			ArrayList al = new ArrayList();
            foreach (DictionaryEntry d in times)
            {
                Int64 value = (Int64)d.Value;
                double t1 = ((double)value) / (double)GetFrequency() * 1000 * 1000;
                string msg = d.Key.ToString();
                StringBuilder s1 = new StringBuilder("\r\n");
                if (t1 < 1000)
                    s1.AppendFormat("{0} Time = {1} us", msg, t1.ToString("F2"));
                else if (t1 < 1000000)
                    s1.AppendFormat("{0} Time = {1} ms", msg, (t1 / 1000).ToString("F2"));
                else
                    s1.AppendFormat("{0} Time = {1} s", msg, (t1 / 1000000).ToString("F2"));
                al.Add(s1.ToString());
            }
            al.Sort();
			foreach (string s in al)
				sb.Append(s);
			times.Clear();
			return sb.ToString();
		}

	}
}
