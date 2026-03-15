using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ASP = System.Web.UI.WebControls;

namespace Skinny
{
  // Applying this attribute to your public methods makes it possible
  // to invoke them from JavaScript in your client pages.
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  sealed class MethodAttribute : Attribute
  {
  }

  sealed class Manager
  {
    #region Control registration

    public static void Register(Control control)
    {
      Register(control.Page, control);
    }

    static void Register(Page page, Control control)
    {
      AddManager(page);
      Manager manager = GetManager();
      if (!object.ReferenceEquals(page, control))
      {
        manager.AddTarget(control);
      }
    }

    static void AddManager(Page page)
    {
      if (!HttpContext.Current.Items.Contains("Skinny.Manager"))
      {
        Manager manager = new Manager();
        page.PreRender += new EventHandler(manager.OnPreRender);
        page.Error += new EventHandler(manager.OnError);
        HttpContext.Current.Items["Skinny.Manager"] = manager;
        manager.RegisterPageScript(page);
      }
    }

    public static Manager GetManager()
    {
      Manager manager = HttpContext.Current.Items["Skinny.Manager"] as Manager;
      if (manager == null)
      {
        throw new ApplicationException("This page was never registered with Skinny.Manager!");
      }
      return manager;
    }

    Manager()
    {
      
    }

    readonly Hashtable _targets = new Hashtable();
    static readonly Hashtable targetmethods = new Hashtable();
    static readonly Hashtable statictargetmethods = new Hashtable();

    void AddTarget(Control control)
    {
      _targets[control.ClientID] = control;

      Type t = control.GetType();

      if (!targetmethods.ContainsKey(t))
      {
        Hashtable meths = new Hashtable();

        foreach (MethodInfo mi in t.GetMethods())
        {
          if (Attribute.IsDefined(mi, typeof(MethodAttribute)))
          {
            meths[mi.Name] = mi;
          }
        }

        targetmethods.Add(t, meths);
      }

      _targets[t.Name] = t;

      if (!statictargetmethods.ContainsKey(t))
      {
        Hashtable meths = new Hashtable();

        foreach (MethodInfo mi in t.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
          if (Attribute.IsDefined(mi, typeof(MethodAttribute)))
          {
            meths[mi.Name] = mi;
          }
        }

        statictargetmethods.Add(t, meths);
      }
    }

    void RegisterPageScript(Page page)
    {
      HttpContext context = HttpContext.Current;
      string url = GetCallBackURL(context);
      string formID = GetFormID(page);

      string pageScript = string.Format(@"
<script type=""text/javascript"">
var Skinny_DefaultURL = ""{0}"";
var Skinny_FormID = ""{1}"";
</script>", url, formID);

      page.ClientScript.RegisterClientScriptBlock(
			typeof(Manager),
			typeof(Manager).FullName,
			pageScript,
			false);
    }

    #endregion
 
    #region Event handling
    
    void OnError(object source, EventArgs e)
    {
      if (IsCallBack)
      {
        Exception error = HttpContext.Current.Error;
        HttpContext.Current.ClearError();
        WriteResult(HttpContext.Current.Response, null, error.Message);
      }
    }

    void OnPreRender(object source, EventArgs e)
    {
      HttpContext context = HttpContext.Current;
      HttpRequest req = context.Request;
      HttpResponse resp = context.Response;

      if (!CheckIfRedirectedToLoginPage() && IsCallBack)
      {
        object targetObject = null;
        string methodName = null;
        bool invokeMethod = true;

        targetObject = _targets[req.Form["SID"]];
        methodName = req.Form["SM"];

        object val = null;
        string error = null;

        if (invokeMethod)
        {
          if (targetObject == null)
          {
            error = "CONTROLNOTFOUND";
          }
          else
          {
            if (methodName != null && methodName.Length > 0)
            {
              MethodInfo methodInfo = FindTargetMethod(targetObject, methodName);
              if (methodInfo == null)
              {
                error = "METHODNOTFOUND";
              }
              else
              {
                try
                {
                  object[] parameters = ConvertParameters(methodInfo, req);
                  val = InvokeMethod(targetObject, methodInfo, parameters);
                }
                catch (Exception ex)
                {
                  error = ex.Message;
                }
              }
            }
          }
        }

        context.Trace.IsEnabled = false;
        resp.Cache.SetCacheability(HttpCacheability.NoCache);
        resp.Filter = new CallBackFilter(this, resp.Filter);
        _value = val;
        _error = error;
      }
    }

    object _value;
    string _error;

    #endregion

    #region Utility methods

    bool CheckIfRedirectedToLoginPage()
    {
      HttpContext context = HttpContext.Current;
      HttpRequest req = context.Request;
      string returnURL = req.QueryString["ReturnURL"];
      if (returnURL != null && returnURL.Length > 0)
      {
        returnURL = context.Server.UrlDecode(returnURL);
		if (!string.IsNullOrEmpty(req.Params["SCB"]) && req.Params["SCB"] == "1")
        {
          HttpResponse resp = context.Response;
          WriteResult(resp, null, "LOGIN");
          resp.End();
          return true;
        }
      }
      return false;
    }

    static string GetUniqueIDWithDollars(Control control)
    {
      string uniqueIdWithDollars = control.UniqueID;
      if (uniqueIdWithDollars == null)
      {
        return null;
      }
      if (uniqueIdWithDollars.IndexOf(':') >= 0)
      {
        return uniqueIdWithDollars.Replace(':', '$');
      }
      return uniqueIdWithDollars;
    }

    static string GetPageURL(HttpContext context)
    {
      string url;
      string currentExecutionFilePath = context.Request.CurrentExecutionFilePath;
      string filePath = context.Request.FilePath;
      if (object.ReferenceEquals(currentExecutionFilePath, filePath))
      {
        url = filePath;
        int lastSlash = url.LastIndexOf('/');
        if (lastSlash != -1)
        {
          url = url.Substring(lastSlash + 1);
        }
      }
      else
      {
        Uri from = new Uri("file://foo" + filePath);
        Uri to = new Uri("file://foo" + currentExecutionFilePath);

        url = from.MakeRelative(to);

      }
      return url;
    }

    static string GetCallBackURL(HttpContext context)
    {
      string url = GetPageURL(context);
      string queryString = context.Request.Url.Query;
      if (queryString != null && queryString.Length != 0)
      {
        url = url + queryString + "&SCB=1";
      }
      else
      {
        url += "?SCB=1";
      }
      return url;
    }

    static string GetFormID(Control control)
    {
      HtmlForm form = FindForm(control.Page);
      if (form != null)
      {
        return form.ClientID;
      }
      return null;
    }

    static HtmlForm FindForm(Control parent)
    {
      foreach (Control child in parent.Controls)
      {
        HtmlForm form = child as HtmlForm;
        if (form != null && form.Visible)
        {
          return form;
        }
        if (child.HasControls())
        {
          HtmlForm htmlForm = FindForm(child);
          if (htmlForm != null)
          {
            return htmlForm;
          }
        }
      }
      return null;
    }

    static string GetStringEndingWithSemicolon(string value)
    {
      if (value != null)
      {
        int length = value.Length;
        if (length > 0 && value[length - 1] != ';')
        {
          return value + ";";
        }
      }
      return value;
    }

    public static bool IsCallBack
    {
      get
      {
        HttpContext context = HttpContext.Current;
        if (context != null)
        {
          HttpRequest req = context.Request;
          string s = req.Params["SCB"];
          return !string.IsNullOrEmpty(s) && s == "1";
        }
        return false;
      }
    }

    #endregion

    #region Method Invocation

    static MethodInfo FindTargetMethod(object target, string methodName)
    {
      if (target is Type)
      {
        Hashtable meths = statictargetmethods[target] as Hashtable;
        return meths[methodName] as MethodInfo;
      }
      else
      {
        Type type = target.GetType();
        Hashtable meths = targetmethods[type] as Hashtable;
        return meths[methodName] as MethodInfo;
      }
    }

    static object[] ConvertParameters(MethodInfo methodInfo, HttpRequest req)
    {
      ParameterInfo[] pis = methodInfo.GetParameters();
      object[] parameters = new object[pis.Length];
      int i = 0;
      foreach (ParameterInfo paramInfo in pis)
      {
        object param = null;
        string paramValue = req.Form["SA" + i];
        if (paramValue != null)
        {
          if (paramInfo.ParameterType.IsArray)
          {
            Type type = paramInfo.ParameterType.GetElementType();
            string[] values = req.Form.GetValues("SA" + i);
            Array array = Array.CreateInstance(type, values.Length);
            for (int index = 0; index < values.Length; index++)
            {
              array.SetValue(Convert.ChangeType(values[index], type), index);
            }
            param = array;
          }
          else
          {
            param = Convert.ChangeType(paramValue, paramInfo.ParameterType);
          }
        }
        parameters[i] = param;
        ++i;
      }
      return parameters;
    }

    static object InvokeMethod(object target, MethodInfo methodInfo, object[] parameters)
    {
      object val = null;
      if (target is Type)
      {
        target = null;
      }
      try
      {
        val = methodInfo.Invoke(target, parameters);
      }
      catch (TargetInvocationException ex)
      {
        // TargetInvocationExceptions should have the actual
        // exception the method threw in its InnerException
        // property.
        if (ex.InnerException != null)
        {
          throw ex.InnerException;
        }
        else
        {
          throw ex;
        }
      }
      return val;
    }

    #endregion

    #region JSON Emitter

    void WriteResult(Stream stream)
    {
      StringBuilder sb = new StringBuilder();
      try
      {
        WriteValueAndError(sb, _value, _error);
      }
      catch (Exception ex)
      {
        // If an exception was thrown while formatting the
        // result value, we need to discard whatever was
        // written and start over with nothing but the error
        // message.
        sb.Length = 0;
        WriteValueAndError(sb, null, ex.Message);
      }
      byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
      stream.Write(buffer, 0, buffer.Length);
    }

    static void WriteResult(HttpResponse resp, object val, string error)
    {
      StringBuilder sb = new StringBuilder();
      try
      {
        WriteValueAndError(sb, val, error);
      }
      catch (Exception ex)
      {
        // If an exception was thrown while formatting the
        // result value, we need to discard whatever was
        // written and start over with nothing but the error
        // message.
        sb.Length = 0;
        WriteValueAndError(sb, null, ex.Message);
      }
      resp.Write(sb.ToString());
    }

    static void WriteValueAndError(StringBuilder sb, object val, string error)
    {
      sb.Append("{\"value\":");
      WriteValue(sb, val);
      sb.Append(",\"error\":");
      WriteValue(sb, error);
      sb.Append("}");
    }

    static void WriteValue(StringBuilder sb, object val)
    {
      if (val == null || val == System.DBNull.Value)
      {
        sb.Append("null");
      }
      else if (val is string || val is Guid)
      {
        WriteString(sb, val.ToString());
      }
      else if (val is bool)
      {
        sb.Append(val.ToString().ToLower());
      }
      else if (val is double ||
        val is float ||
        val is long ||
        val is int ||
        val is short ||
        val is byte ||
        val is decimal)
      {
        sb.Append(val);
      }
      else if (val.GetType().IsEnum)
      {
        sb.Append((int)val);
      }
      else if (val is DateTime)
      {
        sb.Append("new Date(\"");
        sb.Append(((DateTime)val).ToString("MMMM, d yyyy HH:mm:ss", new CultureInfo("en-US", false).DateTimeFormat));
        sb.Append("\")");
      }
      else if (val is DataSet)
      {
        WriteDataSet(sb, val as DataSet);
      }
      else if (val is DataTable)
      {
        WriteDataTable(sb, val as DataTable);
      }
      else if (val is DataRow)
      {
        WriteDataRow(sb, val as DataRow);
      }
      else if (val is Hashtable)
      {
        WriteHashtable(sb, val as Hashtable);
      }
      else if (val is IEnumerable)
      {
        WriteEnumerable(sb, val as IEnumerable);
      }
      else
      {
        WriteObject(sb, val);
      }
    }

    static void WriteString(StringBuilder sb, string s)
    {
      sb.Append("\"");
      foreach (char c in s)
      {
        switch (c)
        {
          case '\"':
            sb.Append("\\\"");
            break;
          case '\\':
            sb.Append("\\\\");
            break;
          case '\b':
            sb.Append("\\b");
            break;
          case '\f':
            sb.Append("\\f");
            break;
          case '\n':
            sb.Append("\\n");
            break;
          case '\r':
            sb.Append("\\r");
            break;
          case '\t':
            sb.Append("\\t");
            break;
          default:
            int i = (int)c;
            if (i < 32 || i > 127)
            {
              sb.AppendFormat("\\u{0:X04}", i);
            }
            else
            {
              sb.Append(c);
            }
            break;
        }
      }
      sb.Append("\"");
    }

    static void WriteDataSet(StringBuilder sb, DataSet ds)
    {
      sb.Append("{\"Tables\":{");
      foreach (DataTable table in ds.Tables)
      {
        sb.AppendFormat("\"{0}\":", table.TableName);
        WriteDataTable(sb, table);
        sb.Append(",");
      }
      // Remove the trailing comma.
      if (ds.Tables.Count > 0)
      {
        --sb.Length;
      }
      sb.Append("}}");
    }

    static void WriteDataTable(StringBuilder sb, DataTable table)
    {
      sb.Append("{\"Rows\":[");
      foreach (DataRow row in table.Rows)
      {
        WriteDataRow(sb, row);
        sb.Append(",");
      }
      // Remove the trailing comma.
      if (table.Rows.Count > 0)
      {
        --sb.Length;
      }
      sb.Append("]}");
    }

    static void WriteDataRow(StringBuilder sb, DataRow row)
    {
      sb.Append("{");
      foreach (DataColumn column in row.Table.Columns)
      {
        sb.AppendFormat("\"{0}\":", column.ColumnName);
        WriteValue(sb, row[column]);
        sb.Append(",");
      }
      // Remove the trailing comma.
      if (row.Table.Columns.Count > 0)
      {
        --sb.Length;
      }
      sb.Append("}");
    }

    static void WriteHashtable(StringBuilder sb, Hashtable e)
    {
      bool hasItems = false;
      sb.Append("{");
      foreach (string key in e.Keys)
      {
        sb.AppendFormat("\"{0}\":", key.ToLower());
        WriteValue(sb, e[key]);
        sb.Append(",");
        hasItems = true;
      }
      // Remove the trailing comma.
      if (hasItems)
      {
        --sb.Length;
      }
      sb.Append("}");
    }

    static void WriteEnumerable(StringBuilder sb, IEnumerable e)
    {
      bool hasItems = false;
      sb.Append("[");
      foreach (object val in e)
      {
        WriteValue(sb, val);
        sb.Append(",");
        hasItems = true;
      }
      // Remove the trailing comma.
      if (hasItems)
      {
        --sb.Length;
      }
      sb.Append("]");
    }

    static void WriteObject(StringBuilder sb, object o)
    {
      MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
      sb.Append("{");
      bool hasMembers = false;
      foreach (MemberInfo member in members)
      {
        bool hasValue = false;
        object val = null;
        if ((member.MemberType & MemberTypes.Field) == MemberTypes.Field)
        {
          FieldInfo field = (FieldInfo)member;
          val = field.GetValue(o);
          hasValue = true;
        }
        else if ((member.MemberType & MemberTypes.Property) == MemberTypes.Property)
        {
          PropertyInfo property = (PropertyInfo)member;
          if (property.CanRead && property.GetIndexParameters().Length == 0)
          {
            val = property.GetValue(o, null);
            hasValue = true;
          }
        }
        if (hasValue)
        {
          sb.Append("\"");
          sb.Append(member.Name);
          sb.Append("\":");
          WriteValue(sb, val);
          sb.Append(",");
          hasMembers = true;
        }
      }
      if (hasMembers)
      {
        --sb.Length;
      }
      sb.Append("}");
    }

    #endregion

    #region HTML output filter

    sealed class CallBackFilter : Stream
    {
      private Manager _manager;
      private Stream _next;

      internal CallBackFilter(Manager manager, Stream next)
      {
        _manager = manager;
        _next = next;
      }

      public override bool CanRead
      {
        get { return false; }
      }

      public override bool CanSeek
      {
        get { return false; }
      }

      public override bool CanWrite
      {
        get { return true; }
      }

      public override long Length
      {
        get { return 0; }
      }

      public override long Position
      {
        get { return 0; }
        set { }
      }

      public override void Close()
      {
        base.Close();
        _manager.WriteResult(_next);
        _next.Close();
      }

      public override void Flush()
      {
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return 0;
      }

      public override void SetLength(long value)
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        return 0;
      }

      public override void Write(byte[] buffer, int offset, int count)
      {

      }
    }

    #endregion

  }
}
