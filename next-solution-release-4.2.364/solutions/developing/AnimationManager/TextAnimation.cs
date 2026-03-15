using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using Utilities.Animations;
using Utilities.WPF;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#endif
using System.Runtime.Serialization;
using System.ComponentModel;
using UFInterfaces;
using DocumentManager.ComponentService;

namespace AnimationManager
{
    [DataContract(Name = "TextAnimation")]
    public class TextAnimation : AnimationManager
    {
#region Properties

        String format;
        [DataMember]
        public String Format
        {
            get { return format; }
            set
            {
                if (value == format)
                    return;
                format = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Format");
#endif
                Reexecute();
#endif
            }
        }

#endregion

#region Overrides
#if !NET_STANDARD
        Random R;
        Object content;
        Dictionary<ContentControl, String> mapOldContent;

        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            if (Control is ContentControl)
            {
                var contentControl = Control as ContentControl;
                if (contentControl.Content is String)
                    content = contentControl.Content;
                else
                {
                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
                                       where c.Content is String &&
#if !WINDOWS_UWP
                                            c.Visibility == System.Windows.Visibility.Visible
#else
                                            c.Visibility == Visibility.Visible
#endif
                                       select c).ToList();
                    if (mapOldContent == null)
                    {
                        mapOldContent = new Dictionary<ContentControl, String>();
                        currentList.ForEach(c =>
                        {
                            mapOldContent.Add(c, c.Content as String);
                        });
                    }
                }
            }
            else if (Control is TextBox)
                content = (Control as TextBox).Text;
            else if (Control is TextBlock)
                content = (Control as TextBlock).Text;

            //else
            //    throw new ArgumentException("The control is not a Content Control");
        }

#if !WINDOWS_UWP
        public override Type[] ExpectingControl()
        {
            return new Type[] { typeof(ContentControl), typeof(TextBox), typeof(TextBlock) };
        }
#endif
        public override void Terminate()
        {
            base.Terminate();

            if (Control is ContentControl)
            {
                var contentControl = Control as ContentControl;
                if (content != null)
                    contentControl.Content = content;
                else if (mapOldContent != null)
                    mapOldContent.Keys.ToList().ForEach(c => c.Content = mapOldContent[c]);
            }
            else if (Control is TextBox)
                (Control as TextBox).Text = content as String;
            else if (Control is TextBlock)
                (Control as TextBlock).Text = content as String;
        }

#if !WINDOWS_UWP
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationTime" || propertyName == "Repeatable" || 
                    propertyName == "Autoreverse" || propertyName == "AnimationBehavior" || 
                    propertyName == "AnimationEquation" || propertyName == "TagMinValue" ||
                    propertyName == "TagMaxValue")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            if (R == null)
                R = new Random();

            var txt = String.Empty;
            if (String.IsNullOrEmpty(Format))
                txt = R.Next().ToString();
            else
            {
                try
                {
                    txt = String.Format(Format, R.Next());
                }
                catch
                {
                    txt = R.Next().ToString();
                }
            }

            if (Control is ContentControl)
            {
                var contentControl = Control as ContentControl;
                if (contentControl.Content is String)
                    contentControl.Content = txt;
                else
                {
                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
                                       where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                       select c).ToList();
                    if (mapOldContent == null)
                    {
                        mapOldContent = new Dictionary<ContentControl, String>();
                        currentList.ForEach(c =>
                        {
                            mapOldContent.Add(c, c.Content as String);
                        });
                    }
                    currentList.ForEach(c =>
                    {
                        c.Content = txt;
                    });
                }
            }
            else if (Control is TextBox)
            {
                if (content == null)
                    content = (Control as TextBox).Text;
                (Control as TextBox).Text = txt;
            }
            else if (Control is TextBlock)
            {
                if (content == null)
                    content = (Control as TextBlock).Text;
                (Control as TextBlock).Text = txt;
            }
            base.Demo();
        }
#endif
        public override void Execute()
        {
            var txt = String.Empty;
            if (LastData != null && LastData.Value != null)
            {
                double num;
                bool isNumeric = double.TryParse(Convert.ToString(LastData.Value, System.Globalization.CultureInfo.InvariantCulture), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out num);
                if (isNumeric)
                {
                    if (String.IsNullOrEmpty(Format))
                        txt = dTargetValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else
                    {
                        try
                        {
                            txt = String.Format(Format, dTargetValue);
                        }
                        catch
                        {
                            txt = dTargetValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Format))
                        txt = LastData.Value.ToString();
                    else
                    {
                        DateTime dtresult;
                        if (DateTime.TryParse(LastData.Value.ToString(), out dtresult))
                        {
                            try
                            {
                                txt = String.Format(Format, dtresult);
                            }
                            catch
                            {
                                txt = LastData.Value.ToString();
                            }
                        }
                        else
                        {
                            try
                            {
                                txt = String.Format(Format, LastData.Value);
                            }
                            catch
                            {
                                txt = LastData.Value.ToString();
                            }
                        }
                    }
                }
            }

            if (Control is ContentControl)
            {
                var contentControl = Control as ContentControl;
                if (contentControl.Content is String)
                    contentControl.Content = txt;
                else
                {
                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
                                       where c.Content is String &&
#if !WINDOWS_UWP
                                            c.Visibility == System.Windows.Visibility.Visible
#else
                                            c.Visibility == Visibility.Visible
#endif
                                       select c).ToList();
                    if (mapOldContent == null)
                    {
                        mapOldContent = new Dictionary<ContentControl, String>();
                        currentList.ForEach(c =>
                        {
                            mapOldContent.Add(c, c.Content as String);
                        });
                    }
                    currentList.ForEach(c =>
                    {
                        c.Content = txt;
                    });
                }
            }
            else if (Control is TextBox)
            {
                var ctrl = (Control as TextBox);
                ctrl.Text = txt;
            }
            else if (Control is TextBlock)
            {
                var ctrl = (Control as TextBlock);
                ctrl.Text = txt;
            }
        }

        public override void Stop()
        {
            if (Control is ContentControl)
            {
                var contentControl = Control as ContentControl;
                if (content != null)
                    contentControl.Content = content;
                else if (mapOldContent != null)
                    mapOldContent.Keys.ToList().ForEach(c => c.Content = mapOldContent[c]);
            }
            else if (content != null)
            {
                if (Control is TextBox)
                    (Control as TextBox).Text = content as String;
                else if (Control is TextBlock)
                    (Control as TextBlock).Text = content as String;
            }
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.TextName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return true;
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
