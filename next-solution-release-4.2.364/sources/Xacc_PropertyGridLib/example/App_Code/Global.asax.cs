using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Drawing;

namespace PropertyGridWeb 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}

    public class Nested
    {
      int i = 0;
      public int Value
      {
        get {return i;}
        set {i = value;}
      }

      public override string ToString()
      {
        return "Nested: " + i;
      }

    }

    public struct Nested2
    {
      int i;
      public int Value
      {
        get {return i;}
        set {i = value;}
      }

      public override string ToString()
      {
        return "Nested2: " + i;
      }

    }

    public class TestObject
    {
      string name = "Hello World";
      int length = 400;
      bool enabled = false, visible = true;
      Color color = Color.Black;
      Rectangle bounds = new Rectangle(0,0,200,200);
      DateTime created = DateTime.Now;

      DayOfWeek day;

      [Category("Details")]
      [Description("Standard greeting.")]
      public string Name
      {
        get {return name;}
        set {name = value;}
      }

      [Category("Stats")]
      [Description("Time since this object been created.")]
      public TimeSpan Age
      {
        get {return DateTime.Now - created;}
      }

      [Category("Stats")]
      [Description("Time now, right now!")]
      public DateTime Time
      {
        get {return DateTime.Now;}
      }

      [Category("Stats")]
      [Description("Memory now, right now!")]
      public long WorkingSet
      {
        get {return Environment.WorkingSet;}
      }

      [Category("Stats")]
      [Description("Process username")]
      public string UserName
      {
        get {return Environment.UserName;}
      }

      [Category("Stats")]
      public TimeSpan Uptime
      {
        get {return new TimeSpan(Environment.TickCount * 10000L);}
      }

      [Category("Stats")]
      [Description("Computer name")]
      public string ComputerName
      {
        get {return Environment.MachineName;}
      }

      [Category("Stats")]
      [Description("Operating system")]
      public OperatingSystem OS
      {
        get {return Environment.OSVersion;}
      }

      [Category("Stats")]
      [Description("CLR runtime version")]
      public Version RuntimeVersion
      {
        get {return Environment.Version;}
      }



      [Category("Behaviour")]
      [Description("Prefered work day")]
      public DayOfWeek Day
      {
        get {return day;}
        set {day = value;}
      }


      Nested n = new Nested();

      [TypeConverter(typeof(ExpandableObjectConverter))]
      public Nested NestedClass
      {
        get {return n;}
      }

      Nested2 n2 = new Nested2();

      [TypeConverter(typeof(ExpandableObjectConverter))]
      public Nested2 NestedStructNoSet
      {
        get {return n2;}
      }

      [TypeConverter(typeof(ExpandableObjectConverter))]
      public Nested2 NestedStruct
      {
        get {return n2;}
        set {n2 = value;}
      }

      [Category("Appearance")]
      [Description("This is the length in cm, please enter an integer value.")]
      public int Length
      {
        get {return length;}
        set {length = value;}
      }

      [Category("Appearance")]
      [Description("Change this value, and see the ones below change too." + 
        "Change a value from below and see how this one changes.")]
      public Rectangle Bounds
      {
        get {return bounds;}
        set {bounds = value;}
      }

      [Category("Behaviour")]
      public bool Righthanded
      {
        get {return true;}
      }

      [Category("Behaviour")]
      public bool Enabled
      {
        get {return enabled;}
        set {enabled = value;}
      }

      [Category("Appearance")]
      public Color Color
      {
        get {return color;}
        set {color = value;}
      }

      [Category("Appearance")]
      [Description("Change this value, and see the one below change too.")]
      public bool Visible
      {
        get {return visible;}
        set {visible = value;}
      }

      [Category("Appearance")]
      public bool VisibleAmbient
      {
        get {return Visible;}
      }

      string[] buddies = {"Tom","Dick","Harry"};

      [Category("Behaviour")]
      [Description("This property cannot be edited, but it's child objects can.")]
      [TypeConverter(typeof(StringArrayConverter))]
      public string[] Buddies
      {
        get {return buddies ; }
        set {buddies = value; }
      }

      public class StringArrayConverter : System.ComponentModel.ArrayConverter
      {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
          if (destinationType == typeof(string))
          {
            return true;
          }
          return base.CanConvertTo (context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
          if (sourceType == typeof(string))
          {
            return true;
          }
          return base.CanConvertFrom (context, sourceType);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
          if (value is string)
          {
            return (value as string).Split(',');
          }

          return base.ConvertFrom (context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
          if (destinationType == typeof(string))
          {
            return string.Join(",", value as string[]);
          }
          return base.ConvertTo (context, culture, value, destinationType);
        }


      }
    }



    public static object STATIC = new TestObject();
		
		protected void Application_Start(Object sender, EventArgs e)
		{

		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

