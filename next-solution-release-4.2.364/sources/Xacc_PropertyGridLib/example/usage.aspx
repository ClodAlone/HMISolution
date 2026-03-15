<%@ Page language="c#" Inherits="PropertyGridWeb.usage" CodeFile="usage.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>usage</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <P>ASP.NET</P>
      <PRE style="FONT-FAMILY: Bitstream Vera Sans Mono,Lucida Console,Courier New; BACKGROUND-COLOR: white; foreground-color: black"><FONT color=blue>&lt;%</FONT>@ Page language="c#" 
Codebehind="default.aspx.cs" 
AutoEventWireup="false" 
Inherits="PropertyGridWeb.WebForm1" 
enableViewState="false" <FONT color=blue>%&gt;</FONT>
<FONT color=blue>&lt;%</FONT>@ Register TagPrefix="cc1" Namespace="PropertyGridLib" Assembly="PropertyGridLib" <FONT color=blue>%&gt;</FONT>
<FONT color=blue>&lt;!DOCTYPE</FONT><FONT color=red>html</FONT>
    <FONT color=red>PUBLIC</FONT>"-//<FONT color=red>W3C</FONT>//<FONT color=red>DTD XHTML</FONT>1<FONT color=red>.0 Strict</FONT>//<FONT color=red>EN</FONT>"
    "<FONT color=red>http:</FONT>// <FONT color=red>www.w3.org</FONT>/<FONT color=red>TR</FONT> /<FONT color=red>xhtml1</FONT> /<FONT color=red>DTD</FONT>/ <FONT color=red>xhtml1-strict.dtd</FONT>"<FONT color=blue>&gt;</FONT>
<FONT color=blue>&lt;html</FONT> <FONT color=red>lang</FONT><FONT color=darkblue>=</FONT> <FONT color=maroon>"en"</FONT><FONT color=blue>&gt;</FONT>
  <FONT color=blue>&lt;head&gt;</FONT>
    <FONT color=blue>&lt;title&gt;</FONT> ASP.NET PropertyGrid Demo<FONT color=blue>&lt;/title&gt;</FONT>
  <FONT color=blue>&lt;/head&gt;</FONT>
  <FONT color=blue>&lt;body&gt;</FONT>
    <FONT color=blue>&lt;form </FONT><FONT color=red>id</FONT><FONT color=darkblue>=</FONT> <FONT color=maroon>"Form1"</FONT><FONT color=red>method</FONT><FONT color=darkblue>=</FONT><FONT color=maroon>"post"</FONT>  <FONT color=red>runat</FONT><FONT color=darkblue>=</FONT> <FONT color=maroon>"server"</FONT><FONT color=blue>&gt;</FONT>
     <FONT color=blue>&lt;cc1:propertygrid</FONT> <FONT color=red>id</FONT> <FONT color=darkblue>=</FONT><FONT color=maroon>"pg1"</FONT> <FONT color=red>runat</FONT><FONT color=darkblue>=</FONT><FONT color=maroon>"server"</FONT><FONT color=red>ShowHelp</FONT><FONT color=darkblue>=</FONT><FONT color=maroon>"True"</FONT>  <FONT color=blue>&gt;&lt;/cc1:propertygrid&gt;</FONT>
     <FONT color=blue>&lt;cc1:propertygrid</FONT> <FONT color=red>id</FONT> <FONT color=darkblue>=</FONT><FONT color=maroon>"pg2"</FONT> <FONT color=red>runat</FONT><FONT color=darkblue>=</FONT><FONT color=maroon>"server"</FONT><FONT color=red>ReadOnly</FONT><FONT color=darkblue>=</FONT><FONT color=maroon>"True"</FONT>  <FONT color=blue>&gt;&lt;/cc1:propertygrid&gt;</FONT>
    <FONT color=blue>&lt;/form&gt;</FONT>
  <FONT color=blue>&lt;/body&gt;</FONT>
<FONT color=blue>&lt;/html&gt;</FONT></PRE>
      <P>C#</P>
      <PRE style="FONT-FAMILY: Bitstream Vera Sans Mono,Lucida Console,Courier New; BACKGROUND-COLOR: white; foreground-color: black">pg1<FONT color=darkblue>.</FONT>SelectedObject = Global<FONT color=darkblue>.</FONT>STATIC;
pg2<FONT color=darkblue>.</FONT>SelectedObject = Global<FONT color=darkblue>.</FONT>STATIC;</PRE>
    </form>
  </body>
</HTML>
