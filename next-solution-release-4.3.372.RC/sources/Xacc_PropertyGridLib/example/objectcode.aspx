<%@ Page language="c#" Inherits="PropertyGridWeb.objectcode" CodeFile="objectcode.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>objectcode</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <PRE style="FONT-FAMILY: Bitstream Vera Sans Mono,Lucida Console,Courier New; BACKGROUND-COLOR: white; foreground-color: black">    <FONT color=blue>public class</FONT> Nested
    {
      <FONT color=blue>int</FONT> i = <FONT color=red>0</FONT>;
      <FONT color=blue>public int</FONT> Value
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> i;}
        <FONT color=blue>set</FONT> {i = <FONT color=blue>value</FONT>;}
      }

      <FONT color=blue>public override string</FONT> ToString<FONT color=darkblue>()</FONT>
      {
        <FONT color=blue>return</FONT> <FONT color=maroon>"Nested: "</FONT> + i;
      }

    }

    <FONT color=blue>public struct</FONT> Nested2
    {
      <FONT color=blue>int</FONT> i;
      <FONT color=blue>public int</FONT> Value
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> i;}
        <FONT color=blue>set</FONT> {i = <FONT color=blue>value</FONT>;}
      }

      <FONT color=blue>public override string</FONT> ToString<FONT color=darkblue>()</FONT>
      {
        <FONT color=blue>return</FONT> <FONT color=maroon>"Nested2: "</FONT> + i;
      }

    }

    <FONT color=blue>public class</FONT> TestObject
    {
      <FONT color=blue>string</FONT> name = <FONT color=maroon>"Hello World"</FONT>;
      <FONT color=blue>int</FONT> length = <FONT color=red>400</FONT>;
      <FONT color=blue>bool</FONT> enabled = <FONT color=blue>false</FONT>, visible = <FONT color=blue>true</FONT>;
      Color color = Color<FONT color=darkblue>.</FONT>Black;
      Rectangle bounds = <FONT color=blue>new</FONT> Rectangle<FONT color=darkblue>(</FONT><FONT color=red>0</FONT>,<FONT color=red>0</FONT>,<FONT color=red>200</FONT>,<FONT color=red>200</FONT><FONT color=darkblue>)</FONT>;
      DateTime created = DateTime<FONT color=darkblue>.</FONT>Now;

      DayOfWeek day;

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Details"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Standard greeting."</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public string</FONT> Name
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> name;}
        <FONT color=blue>set</FONT> {name = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Time since this object been created."</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> TimeSpan Age
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> DateTime<FONT color=darkblue>.</FONT>Now - created;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Time now, right now!"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> DateTime Time
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> DateTime<FONT color=darkblue>.</FONT>Now;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Memory now, right now!"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public long</FONT> WorkingSet
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> Environment<FONT color=darkblue>.</FONT>WorkingSet;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Process username"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public string</FONT> UserName
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> Environment<FONT color=darkblue>.</FONT>UserName;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> TimeSpan Uptime
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return new</FONT> TimeSpan<FONT color=darkblue>(</FONT>Environment<FONT color=darkblue>.</FONT>TickCount * <FONT color=red>10000L</FONT><FONT color=darkblue>)</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Computer name"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public string</FONT> ComputerName
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> Environment<FONT color=darkblue>.</FONT>MachineName;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Operating system"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> OperatingSystem OS
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> Environment<FONT color=darkblue>.</FONT>OSVersion;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Stats"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"CLR runtime version"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> Version RuntimeVersion
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> Environment<FONT color=darkblue>.</FONT>Version;}
      }



      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Behaviour"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Prefered work day"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> DayOfWeek Day
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> day;}
        <FONT color=blue>set</FONT> {day = <FONT color=blue>value</FONT>;}
      }


      Nested n = <FONT color=blue>new</FONT> Nested<FONT color=darkblue>()</FONT>;

      [TypeConverter<FONT color=darkblue>(</FONT><FONT color=blue>typeof</FONT><FONT color=darkblue>(</FONT>ExpandableObjectConverter<FONT color=darkblue>))</FONT>]
      <FONT color=blue>public</FONT> Nested NestedClass
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> n;}
      }

      Nested2 n2 = <FONT color=blue>new</FONT> Nested2<FONT color=darkblue>()</FONT>;

      [TypeConverter<FONT color=darkblue>(</FONT><FONT color=blue>typeof</FONT><FONT color=darkblue>(</FONT>ExpandableObjectConverter<FONT color=darkblue>))</FONT>]
      <FONT color=blue>public</FONT> Nested2 NestedStructNoSet
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> n2;}
      }

      [TypeConverter<FONT color=darkblue>(</FONT><FONT color=blue>typeof</FONT><FONT color=darkblue>(</FONT>ExpandableObjectConverter<FONT color=darkblue>))</FONT>]
      <FONT color=blue>public</FONT> Nested2 NestedStruct
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> n2;}
        <FONT color=blue>set</FONT> {n2 = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Appearance"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"This is the length in cm, please enter an integer value."</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public int</FONT> Length
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> length;}
        <FONT color=blue>set</FONT> {length = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Appearance"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Change this value, and see the ones below change too."</FONT> +
        <FONT color=maroon>"Change a value from below and see how this one changes."</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> Rectangle Bounds
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> bounds;}
        <FONT color=blue>set</FONT> {bounds = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Behaviour"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public bool</FONT> Righthanded
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return true</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Behaviour"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public bool</FONT> Enabled
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> enabled;}
        <FONT color=blue>set</FONT> {enabled = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Appearance"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public</FONT> Color Color
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> color;}
        <FONT color=blue>set</FONT> {color = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Appearance"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"Change this value, and see the one below change too."</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public bool</FONT> Visible
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> visible;}
        <FONT color=blue>set</FONT> {visible = <FONT color=blue>value</FONT>;}
      }

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Appearance"</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public bool</FONT> VisibleAmbient
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> Visible;}
      }

      <FONT color=blue>string</FONT><FONT color=darkblue>[]</FONT> buddies = {<FONT color=maroon>"Tom"</FONT>,<FONT color=maroon>"Dick"</FONT>,<FONT color=maroon>"Harry"</FONT>};

      [Category<FONT color=darkblue>(</FONT><FONT color=maroon>"Behaviour"</FONT><FONT color=darkblue>)</FONT>]
      [Description<FONT color=darkblue>(</FONT><FONT color=maroon>"This property cannot be edited, but it's child objects can."</FONT><FONT color=darkblue>)</FONT>]
      <FONT color=blue>public string</FONT><FONT color=darkblue>[]</FONT> Buddies
      {
        <FONT color=blue>get</FONT> {<FONT color=blue>return</FONT> buddies ; }
        <FONT color=blue>set</FONT> {buddies = <FONT color=blue>value</FONT>; }
      }
    }</PRE>
    </form>
  </body>
</HTML>
