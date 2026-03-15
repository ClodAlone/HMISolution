using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TraceTextBox
{
	interface ITraceTextSink
	{
		void Fail(string msg);
		void Event(string msg, TraceEventType eventType);
	}
}
