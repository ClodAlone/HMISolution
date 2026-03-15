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
using System.Reflection;
using System.Diagnostics;

namespace Syncfusion.Scripting.Design
{
  /// <summary>
  /// Summary description for ScriptWrapper.
  /// </summary>
  public class ScriptWrapper
  {
    #region Class constants
    /// <summary>
    /// This construction must be replaced for real class name
    /// when class wraps user script.
    /// </summary>
    private const string DEF_CLASS_NAME = "<%?ClassName%>";
    /// <summary>
    /// This construction must be replaced for real object name
    /// when class wraps user script.
    /// </summary>
    private const string DEF_OBJECT_NAME = "<%?ObjectName%>";
    /// <summary>
    /// This construction must be replaced for real event name
    /// when class wraps user script.
    /// </summary>
    private const string DEF_EVENT_NAME = "<%?EventName%>";

    private const string DEF_NAMESPACE = "namespace ";

	private const char DEF_BRACE_OPEN = '{';

	private const char DEF_BRACE_CLOSE = '}';

    /// <summary>
    /// Class header for CSharp.
    /// </summary>
	private const string DEF_CS_CLASS_HEADER = "public class " + DEF_CLASS_NAME;

    /// <summary>
    /// Class footer for CSharp.
    /// </summary>
    private const string DEF_CS_CLASS_FOOTER = "}";

    /// <summary>
    /// Class header for Visual Basic Net.
    /// </summary>
    private const string DEF_VB_CLASS_HEADER = "module " + DEF_CLASS_NAME;
    /// <summary>
    /// Class footer for Visual Basic Net.
    /// </summary>
    private const string DEF_VB_CLASS_FOOTER = "end module";

    /// <summary>
    /// Class header for Java Script Net.
    /// </summary>
	private const string DEF_JS_CLASS_HEADER = "class " + DEF_CLASS_NAME;
    /// <summary>
    /// Class footer for Java Script Net.
    /// </summary>
    private const string DEF_JS_CLASS_FOOTER = DEF_CS_CLASS_FOOTER;

    /// <summary>
    /// Using directive for CSharp.
    /// </summary>
    private const string DEF_CS_USING = "using ";
    /// <summary>
    /// Using directive for Visual Basic Net.
    /// </summary>
    private const string DEF_VB_USING = "Imports ";
    /// <summary>
    /// Using directive for Java Script Net.
    /// </summary>
    private const string DEF_JS_USING = "import ";

    /// <summary>
    /// Delimeter between using directives in CSharp.
    /// </summary>
    private const string DEF_CS_USINGDELIM = ";\n";
    /// <summary>
    /// Delimeter between using directives in Visual Basic Net.
    /// </summary>
    private const string DEF_VB_USINGDELIM = "\n";
    /// <summary>
    /// Delimeter between using directives in Java Script Net.
    /// </summary>
    private const string DEF_JS_USINGDELIM = ";\n";

    /// <summary>
    /// Delimiter before inherited class.
    /// </summary>
    private const string DEF_CS_INHERITSDELIM = " : ";
    /// <summary>
    /// Delimiter before inherited class in Visual Basic Net.
    /// </summary>
    private const string DEF_VB_INHERITSDELIM = "\r\nInherits ";
    /// <summary>
    /// Delimiter before inherited class in Java Script Net.
    /// </summary>
    private const string DEF_JS_INHERITSDELIM = " extends ";
	/// <summary>
	/// Name of the global class for accessing to global objects.
	/// </summary>
	private const string DEF_GLOBAL_NAME = "Global";
    #endregion

    public static string GetWrappedScript( ScriptLanguages language, string script, string namespacedirective, 
		string directives, string globalcode, string classname, string baseclassname)
    {
      string result = string.Empty;
      string usingWord;
      string usingDelim;
      string classHeader;
      string classFooter;
      string inheritDelim;

      GetLanguageKeywords( language, out usingWord, out usingDelim, out classHeader,
                           out classFooter, out inheritDelim );
	  bool bhasbraces = false;
	  if((language == ScriptLanguages.CSharp) || (language == ScriptLanguages.JScript))
		bhasbraces = true;
	 	  
	  result += classHeader.Replace(DEF_CLASS_NAME, classname);
	  if(bhasbraces == true)
		  result += "\n{";
	  script = "\r\n" + script.Trim('\r','\n',' ');	 
      result += "  " + script.Replace("\n", "\n  ") + "\n" + classFooter;

      if(globalcode != String.Empty )
      {
        globalcode = globalcode.TrimStart('\n', '\r', ' ');
        if( globalcode != String.Empty )
          result = result + "\n\n" + globalcode;
      }

	  if(namespacedirective != String.Empty)
	  {
		  result = String.Concat(DEF_NAMESPACE, namespacedirective, "\n{\n") + result;
		  result += "\n}\n";
	  }

	  if(directives != String.Empty)
	  {
		  directives = directives.TrimEnd(' ', '\n', '\r');
		  result = directives + "\n\n" + result;
	  }

      return result;
    }

    
    public static string GetUnwrappedScript(ScriptLanguages language, string script, ref string namespacedirective, 
		ref string directives, ref string globalcode, string classname, string baseclassname)
    {
      string usingword;
      string usingdelim;
      string classheader;
      string classfooter;
      string inheritdelim;

      GetLanguageKeywords(language, out usingword, out usingdelim, out classheader,
                           out classfooter, out inheritdelim);
	  bool bhasbraces = false;
	  if((language == ScriptLanguages.CSharp) || (language == ScriptLanguages.JScript))
		bhasbraces = true;
      classheader = classheader.Replace(DEF_CLASS_NAME, classname);	 

      string unwrappedscript = string.Empty;
      globalcode = string.Empty;
      if(script != string.Empty)
      {
        // If a namespace exists in the script then remove the namespace define
		int namespaceindex = script.IndexOf(DEF_NAMESPACE);
		if(namespaceindex >= 0)
		{
			int beginbraces = script.IndexOf(DEF_BRACE_OPEN, namespaceindex);
			if(beginbraces > namespaceindex)
			{
				int namestart = namespaceindex+DEF_NAMESPACE.Length;
				namespacedirective = script.Substring(namestart, beginbraces-namestart);

				script = script.Remove(namespaceindex, beginbraces-(namespaceindex-1));
				script = script.Remove(script.LastIndexOf(DEF_BRACE_CLOSE), 1);				

				namespacedirective = namespacedirective.TrimEnd(' ', '\n', '\r');
			}
			else
				Debug.Assert(false, "Error: Please make sure that the code in the editor is correctly formatted.");
		}

		int usingwordlength = usingword.Length;
        int directivestart = script.IndexOf(usingword);
        int classstart = script.IndexOf(classheader, 0);
        if(directivestart >= 0)
        {
          if(classstart >= 0)
          {
            directives = script.Substring(directivestart, classstart-directivestart);
          }
          else
          {
            directives = script.Substring(directivestart);
          }
		  script = script.Remove(directivestart, directives.Length);
          directives = directives.TrimEnd(' ', '\n', '\r');
		  classstart = script.IndexOf(classheader, 0);	// The classstart index will have changed
        }

        if(classstart >= 0)
        {
          int footerstart = script.LastIndexOf(classfooter);
          if(footerstart >= 0)
          {
            unwrappedscript = script.Substring(classstart+classheader.Length, footerstart-(classstart+classheader.Length));		    
			if(bhasbraces == true)
				unwrappedscript = unwrappedscript.TrimStart(' ','\n','\r','{');
          }
          unwrappedscript = unwrappedscript.Replace("\n  ", "\n");
          globalcode = script.Remove(classstart, (footerstart+classfooter.Length) - classstart);		  
 		  globalcode = globalcode.Trim(' ', '\n', '\r');
        }
      }
      return unwrappedscript.TrimEnd(' ', '\n', '\r');
    }

	public static string GetEventHandlerScriptByType(ScriptLanguages language, string classname, EventInfo evInfo)
	{
        
		  MethodInfo metInfo = evInfo.EventHandlerType.GetMethod("Invoke");
		  ParameterInfo[] parInfos = metInfo.GetParameters();
          string argsTypeName = string.Empty;
          foreach (ParameterInfo parInfo in parInfos)
          {
              if (parInfo.Name == "evtArgs")
              {
                  argsTypeName = parInfo.ParameterType.Name;
                  argsTypeName = argsTypeName.Replace('+', '.');
                  break;
              }               
          }		  

		  return ScriptWrapper.GetEventHandlerScript(language, classname, evInfo.Name, argsTypeName); 
	}

	public static string GetEventHandlerScript(ScriptLanguages language, string classname, string eventname, string argsType)
	{
		  string scriptHandler = "";
		  string handlername = classname + "_" + eventname;
		  switch(language)
		  {
			  case ScriptLanguages.CSharp:
				  scriptHandler =
					  "public static void " + handlername + "(object sender, " + argsType + " args" + ")\n{\n}";
				  break;

			  case ScriptLanguages.VisualBasic:
				  scriptHandler =
					  "Public Sub " + handlername + "(sender As Object, e As " +
					  argsType + ")" + " Handles " + classname + "." + eventname + "\nEnd Sub";
				  break;

			  case ScriptLanguages.JScript:
				  scriptHandler =
					  "static function " + handlername + "(sender: Object, " +
					  " args:" + argsType + ")\n{\n}";
				  break;
		  }
		  return scriptHandler;
	}

	public static string GetEventSubscriberScript(ScriptLanguages language, string nmespace, string scriptclass, string classname, EventInfo evinfo)
	{
		  string eventscript = string.Empty;
		  string eventname = evinfo.Name;	
		  switch(language)
		  {
			  case ScriptLanguages.VisualBasic:
				  break;
			  case ScriptLanguages.JScript:
				  eventscript = classname + "." + "add_" + eventname + "(" + scriptclass + "." + classname + "_" + eventname + ");";
				  break;
			  case ScriptLanguages.CSharp:
				  eventscript = nmespace + "." + DEF_GLOBAL_NAME + "." + classname + "." + eventname + " += new " + evinfo.EventHandlerType.Name + "(" + scriptclass + "." + classname + "_" + eventname + ");";
				  break;
		  }
		  return eventscript;
	}

	public static string GetEventUnsubscriberScript(ScriptLanguages language, string nmespace, string scriptclass, string classname, EventInfo evinfo)
	{
		  string eventscript = string.Empty;
		  string eventname = evinfo.Name;
		  switch(language)
		  {
			  case ScriptLanguages.VisualBasic:
				  break;
			  case ScriptLanguages.JScript:
				  eventscript = classname + "." + "remove_" + eventname + "(" + scriptclass + "." + classname + "_" + eventname + ");";
				  break;
			  case ScriptLanguages.CSharp:
				  eventscript = nmespace + "." + DEF_GLOBAL_NAME + "." + classname + "." + eventname + " -= new " + evinfo.EventHandlerType.Name + "(" + scriptclass + "." + classname + "_" + eventname + ");";
				  break;
		  }
		  return eventscript;
	}

	public static string AddEventSubscriptionToScript(ScriptLanguages language, string scriptcode, string methodname, string eventscript)
	{
		int methodindex = scriptcode.IndexOf(methodname);
		if(methodindex == -1)
		{
			// If the script does not include the particular method then add the code for it			
			scriptcode += "\r\n\n" + ScriptWrapper.GetNewMethodScript(language, methodname);
			methodindex = scriptcode.IndexOf(methodname);			
		}
		int methodstart = scriptcode.IndexOf(DEF_BRACE_OPEN, methodindex)+1;
		int methodend = scriptcode.IndexOf(DEF_BRACE_CLOSE, methodstart);
		if((methodstart >= 0) && (methodend >= 0))
		{
			string scriptmethodbody = scriptcode.Substring(methodstart, methodend-methodstart);
			scriptcode = scriptcode.Remove(methodstart, scriptmethodbody.Length);
			scriptmethodbody = scriptmethodbody.Trim(' ','\r','\n');
			scriptmethodbody += "\n" + eventscript;		  
			scriptcode = scriptcode.Insert(methodstart, String.Concat("\r\n", scriptmethodbody.Trim(' ','\r','\n'), "\r\n"));
		}
		return scriptcode;
	}

	public static string GetNewMethodScript(ScriptLanguages language, string methodname)
	{
		  string scriptNewFunction = string.Empty;
		  switch(language)
		  {
			  case ScriptLanguages.CSharp:
				  scriptNewFunction = "public static void " + methodname + "()\n{}";
				  break;

			  case ScriptLanguages.VisualBasic:
				  scriptNewFunction = "Public Sub " + methodname + "()/nEnd Sub";
				  break;

			  case ScriptLanguages.JScript:
				  scriptNewFunction = "public static function " + methodname + "()\n{}";
				  break;
		  }      
		  return scriptNewFunction;
	}


    public static void GetLanguageKeywords( ScriptLanguages language, out string usingWord, 
      out string usingDelimiter, out string classHeader, out string classFooter, 
      out string inheritDelimiter )
    {
      switch( language )
      {
        case ScriptLanguages.CSharp:
          usingWord = DEF_CS_USING;
          usingDelimiter = DEF_CS_USINGDELIM;
          classHeader = DEF_CS_CLASS_HEADER;
          classFooter = DEF_CS_CLASS_FOOTER;
          inheritDelimiter = DEF_CS_INHERITSDELIM;
          break;

        case ScriptLanguages.VisualBasic:
          usingWord = DEF_VB_USING;
          usingDelimiter = DEF_VB_USINGDELIM;
          classHeader = DEF_VB_CLASS_HEADER;
          classFooter = DEF_VB_CLASS_FOOTER;
          inheritDelimiter = DEF_VB_INHERITSDELIM;
          break;

        case ScriptLanguages.JScript:
          usingWord = DEF_JS_USING;
          usingDelimiter = DEF_JS_USINGDELIM;
          classHeader = DEF_JS_CLASS_HEADER;
          classFooter = DEF_JS_CLASS_FOOTER;
          inheritDelimiter = DEF_JS_INHERITSDELIM;
          break;

        default:
          throw new ArgumentOutOfRangeException( "language" );
      }
    }
  }
}