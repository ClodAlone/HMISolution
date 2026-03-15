using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace UFProjectManager.Document
{
    public class AutoloadScreen : Observable
    {
        String path;
        public 
        Uri uri;
        public Uri Uri
        {
            get
            {
                return uri;
            }
            set
            {
                Set<Uri>(ref uri, value, "Uri");
            }
        }
        ExecutionMode executionMode;
        public ExecutionMode ExecutionMode
        {
            get
            {
                return executionMode;
            }
            set
            {
                Set<ExecutionMode>(ref executionMode, value, "ExecutionMode");
            }
        }

        bool executeAsService;
        public bool ExecuteAsService
        {
            get
            {
                return executeAsService;
            }
            set
            {
                Set<bool>(ref executeAsService, value, "ExecuteAsService");
            }
        }
    }
}
