using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
#endif
using Utilities;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media.Animation;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif
using System.ComponentModel;

namespace AnimationManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    public class StoryboardConverter : System.ComponentModel.TypeConverter
    {
        public StoryboardConverter()
        {
        }

        // Indicates this converter provides a list of standard values.
        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        // Returns a StandardValuesCollection of standard value objects.
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            // Passes the local integer array.
            if (StoryboardAnimation.currentControl != null)
            {
                StandardValuesCollection svc =
                new StandardValuesCollection(StoryboardAnimation.currentControl.GetAllResourceTypesInChildren(typeof(Storyboard)).Keys);
                return svc;
            }
            else
            {
                StandardValuesCollection svc =
                new StandardValuesCollection(Application.Current.MainWindow.GetAllResourceTypesInChildren(typeof(Storyboard)).Keys);
                return svc;
            }
        }

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        // If the type of the value to convert is string, parses the string 
        // and returns the integer to set the value of the property to. 
        // This example first extends the integer array that supplies the 
        // standard values collection if the user-entered value is not 
        // already in the array.
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                // Parses the string to get the integer to set to the property.
                int newVal = int.Parse((string)value);

                // Returns the integer value to assign to the property.
                return newVal;
            }
            else
                return base.ConvertFrom(context, culture, value);
        }
    }
#endif


    [DataContract(Name = "StoryboardAnimation")]
    public class StoryboardAnimation : AnimationManager
    {
#region Declaration
#if !NET_STANDARD
        public static FrameworkElement currentControl;
        Storyboard sb;
#endif
#endregion

#region Properties

        String storyBoard;
        [DataMember]
#if !WINDOWS_UWP && !NET_STANDARD
        [TypeConverter(typeof(StoryboardConverter))]
#endif
        public String StoryBoard
        {
            get { return storyBoard; }
            set
            {
                if (value == storyBoard)
                    return;
                storyBoard = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("StoryBoard");
#endif
                Reexecute();
#endif
            }
        }

#endregion

#region Overrides
#if !NET_STANDARD
#if !WINDOWS_UWP
        [Browsable(false)]
        public override UserControl Editor
        {
            get
            {
                currentControl = Control as FrameworkElement;
                return base.Editor;
            }
        }

        public override void Demo()
        {
            sb = Control.ApplyStoryBoard(StoryBoard, Repeatable, Autoreverse);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control != null)
                sb = Control.ApplyStoryBoard(StoryBoard, Repeatable, Autoreverse);
        }

        public override void Stop()
        {
            if (sb != null)
                sb.Stop();
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.StoryboardName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        public override String AnimationSummary
        {
            get
            {
                return base.AnimationSummary;
            }
        }
#endif
        
#endregion
    }
}
