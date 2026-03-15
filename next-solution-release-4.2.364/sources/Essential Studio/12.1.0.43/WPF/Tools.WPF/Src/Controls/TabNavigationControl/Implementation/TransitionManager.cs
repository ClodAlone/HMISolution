#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TransitionManager
    {
        private System.Windows.ResourceDictionary mgeneric;

        private Storyboard oldanimation;

        private Storyboard newanimation;

        private TabNavigationControl m_tabcontrol;

        public TransitionManager(TabNavigationControl tab)
        {
            m_tabcontrol = tab;
            mgeneric = new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/TransitionEffects.xaml", UriKind.RelativeOrAbsolute) };
        }

        public void PlayTransitionEffect(TabNavigationItem oldContent, TabNavigationItem newContent, TransitionEffects effect)
        {
            if (oldanimation != null)
            {
                oldanimation.Stop();
            }

            if (newanimation != null)
            {
                newanimation.Stop();
            }

            if (m_tabcontrol != null && m_tabcontrol.frontOldContent != null && m_tabcontrol.oldContent != null)
            {
                if (oldContent == null && m_tabcontrol.IsTouchTranform)
                {
                    m_tabcontrol.frontOldContent.Opacity = 0;
                    m_tabcontrol.oldContent.Opacity = 0;
                }
                else
                {
                    m_tabcontrol.frontOldContent.Opacity = 1;
                    m_tabcontrol.oldContent.Opacity = 1;
                }
            }

            switch (effect)
            {
                case TransitionEffects.None:
                    {
                        HideFakeElements();
                        break;
                    }

                case TransitionEffects.Slide:
                    {
                        int oldindex = -1, newindex = -1;
                        if (m_tabcontrol.ItemsSource != null)
                        {
                            if (oldContent != null)
                            {
                                if (oldContent.DataContext != null && m_tabcontrol.Items.Contains(oldContent.DataContext))
                                {
                                    oldindex = m_tabcontrol.Items.IndexOf(oldContent.DataContext);
                                }
                                else if (m_tabcontrol.Items.Contains(oldContent))
                                {
                                    oldindex = m_tabcontrol.Items.IndexOf(oldContent);
                                }
                            }

                            if (newContent != null)
                            {
                                if (newContent.DataContext != null && m_tabcontrol.Items.Contains(newContent.DataContext))
                                {
                                    newindex = m_tabcontrol.Items.IndexOf(newContent.DataContext);
                                }
                                else if (m_tabcontrol.Items.Contains(newContent))
                                {
                                    newindex = m_tabcontrol.Items.IndexOf(newContent);
                                }
                            }
                        }
                        else
                        {
                            oldindex = m_tabcontrol.Items.IndexOf(oldContent);
                            newindex = m_tabcontrol.Items.IndexOf(newContent);
                        }
                        if (oldindex < newindex)
                        {
                            if (oldContent != null)
                            {
                                oldanimation = mgeneric["NextOldSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = oldanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe0 = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                                if (m_tabcontrol != null && m_tabcontrol.IsTouchTranform)
                                {
                                    if (m_tabcontrol.IsTouchTransformReset)
                                    {
                                        keyframe0.Value = -(m_tabcontrol.ActualWidth - Math.Abs(m_tabcontrol.TotalTransformValue.X));
                                    }
                                    else
                                    {
                                        keyframe0.Value = -Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                    }
                                    keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                                }
                                else
                                {
                                    keyframe0.Value = 1;
                                    keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                                }

                                keyframe.Value = -m_tabcontrol.ActualWidth;
                                FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                                if (oldcontent != null && m_tabcontrol.oldContent != null)
                                {
                                    oldcontent.Effect = null;
                                    m_tabcontrol.frontOldContent.Content = null;

                                    m_tabcontrol.oldContent.Content = oldcontent;
                                    oldcontent.RenderTransform = new TranslateTransform();
                                    Storyboard.SetTarget(oldanimation, oldcontent);

                                    oldanimation.Begin();
                                }
                            }

                            if (newContent != null)
                            {
                                newanimation = mgeneric["NextNewSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = newanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame keyframe1 = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                                if (m_tabcontrol != null && m_tabcontrol.IsTouchTranform)
                                {
                                    if (m_tabcontrol.IsTouchTransformReset)
                                    {
                                        if (m_tabcontrol.IsLast)
                                        {
                                            keyframe.Value = -Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                        }
                                        else
                                        {
                                            keyframe.Value = Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                        }
                                    }
                                    else
                                    {
                                        keyframe.Value = m_tabcontrol.ActualWidth - Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                    }
                                    keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                                }
                                else
                                {
                                    keyframe.Value = m_tabcontrol.ActualWidth;
                                    keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                                }

                                newContent.RenderTransform = new TranslateTransform();

                                UIElement newcontent = newContent.Content as UIElement;
                                if (newcontent != null)
                                {
                                    newcontent.RenderTransform = new TranslateTransform();
                                    Storyboard.SetTarget(newanimation, newcontent);
                                    newanimation.Begin();
                                }
                            }
                        }
                        else
                        {
                            if (oldContent != null)
                            {
                                oldanimation = mgeneric["NextOldSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = oldanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame keyfrome0 = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                if (m_tabcontrol.IsTouchTranform)
                                {
                                    if (m_tabcontrol.IsTouchTransformReset)
                                    {
                                        keyfrome0.Value = m_tabcontrol.ActualWidth - Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                    }
                                    else
                                    {
                                        keyfrome0.Value = Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                    }
                                    keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                                }
                                else
                                {
                                    keyfrome0.Value = 1;
                                    keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                                }
                                keyframe.Value = m_tabcontrol.ActualWidth;
                                FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                                if (oldcontent != null)
                                {
                                    oldcontent.Effect = null;
                                    m_tabcontrol.frontOldContent.Content = oldcontent;
                                    m_tabcontrol.oldContent.Content = null;
                                    oldcontent.RenderTransform = new TranslateTransform();
                                    Storyboard.SetTarget(oldanimation, oldcontent);
                                    oldanimation.Begin();
                                }
                            }

                            if (newContent != null)
                            {
                                newanimation = mgeneric["NextNewSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = newanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                EasingDoubleKeyFrame keyframe1 = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                                if (m_tabcontrol.IsTouchTranform)
                                {
                                    if (m_tabcontrol.IsTouchTransformReset)
                                    {
                                        keyframe.Value = -Math.Abs(m_tabcontrol.TotalTransformValue.X);
                                    }
                                    else
                                    {
                                        keyframe.Value = -(m_tabcontrol.ActualWidth - Math.Abs(m_tabcontrol.TotalTransformValue.X));
                                    }
                                    keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                                }
                                else
                                {
                                    keyframe.Value = -m_tabcontrol.ActualWidth;
                                    keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                                }
                                newContent.RenderTransform = new TranslateTransform();
                                UIElement newcontent = newContent.Content as UIElement;
                                if (newcontent != null)
                                {
                                    newcontent.RenderTransform = new TranslateTransform();
                                    Storyboard.SetTarget(newanimation, newcontent);
                                    newanimation.Begin();
                                }
                            }
                        }
                        break;
                    }
                case TransitionEffects.Fade:
                    {
                        if (oldContent != null)
                        {
                            FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                            oldcontent.Effect = null;
                            m_tabcontrol.oldContent.Content = oldcontent;
                            m_tabcontrol.frontOldContent.Content = null;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["Fade"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            newanimation.Completed += new EventHandler(newanimation_Completed);
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }

                case TransitionEffects.Zoom:
                    {
                        if (oldContent != null)
                        {
                            FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                            m_tabcontrol.oldContent.Content = oldcontent;
                            m_tabcontrol.frontOldContent.Content = null;
                            oldcontent.Effect = null;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["Zoom"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            newcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            newcontent.RenderTransform = new ScaleTransform();
                            newanimation.Completed += new EventHandler(newanimation_Completed);
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }

                case TransitionEffects.Blur:
                    {
                        if (oldContent != null)
                        {
                            UIElement oldcontent = Clone(oldContent.Content as UIElement);
                            m_tabcontrol.frontOldContent.Content = oldcontent;
                            m_tabcontrol.oldContent.Content = null;
                            if (m_tabcontrol.ItemsSource != null)
                            {
                                m_tabcontrol.oldContent.ContentTemplate = null;
                            }
                            oldanimation = mgeneric["Blur"] as Storyboard;
                            oldcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            oldcontent.RenderTransform = new ScaleTransform();
                            if (newanimation != null)
                                newanimation.Completed += new EventHandler(newanimation_Completed);
                            oldcontent.Effect = new BlurEffect();
                            Storyboard.SetTarget(oldanimation, oldcontent);
                            oldanimation.Begin();
                        }
                        if (newContent != null)
                        {
                        }
                        break;
                    }
                case TransitionEffects.Push:
                    {
                        double actualHeight = m_tabcontrol.ActualHeight - (m_tabcontrol.contentRoot as Grid).RowDefinitions[0].ActualHeight;
                        if (oldContent != null)
                        {
                            oldanimation = mgeneric["PushSlide"] as Storyboard;
                            DoubleAnimationUsingKeyFrames animation = oldanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                            EasingDoubleKeyFrame keyframe = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                            EasingDoubleKeyFrame keyframe0 = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                            if (m_tabcontrol != null && m_tabcontrol.IsTouchTranform)
                            {
                                if (m_tabcontrol.IsTouchTransformReset)
                                {
                                    keyframe0.Value = -(actualHeight - Math.Abs(m_tabcontrol.TotalTransformValue.Y));
                                }
                                else
                                {
                                    keyframe0.Value = -Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                }
                                keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                            }
                            else
                            {
                                keyframe0.Value = 1;
                                keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                            }
                            keyframe.Value = -actualHeight;
                            FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                            if (oldcontent != null && m_tabcontrol.oldContent != null)
                            {
                                oldcontent.Effect = null;
                                m_tabcontrol.frontOldContent.Content = null;

                                m_tabcontrol.oldContent.Content = oldcontent;
                                oldcontent.RenderTransform = new TranslateTransform();
                                Storyboard.SetTarget(oldanimation, oldcontent);

                                oldanimation.Begin();
                            }
                        }

                        if (newContent != null)
                        {
                            newanimation = mgeneric["PullSlide"] as Storyboard;
                            DoubleAnimationUsingKeyFrames animation = newanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                            EasingDoubleKeyFrame keyframe = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                            EasingDoubleKeyFrame keyframe1 = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                            if (m_tabcontrol != null && m_tabcontrol.IsTouchTranform)
                            {
                                if (m_tabcontrol.IsTouchTransformReset)
                                {
                                    if (m_tabcontrol.IsLast)
                                    {
                                        keyframe.Value = -Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                    }
                                    else
                                    {
                                        keyframe.Value = Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                    }
                                }
                                else
                                {
                                    keyframe.Value = actualHeight - Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                }
                                keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                            }
                            else
                            {
                                keyframe.Value = actualHeight;
                                keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                            }
                            newContent.RenderTransform = new TranslateTransform();

                            UIElement newcontent = newContent.Content as UIElement;
                            if (newcontent != null)
                            {
                                newcontent.RenderTransform = new TranslateTransform();
                                Storyboard.SetTarget(newanimation, newcontent);
                                newanimation.Begin();
                            }
                        }

                        break;
                    }

                case TransitionEffects.PushIn:
                    {
                        double actualHeight = m_tabcontrol.ActualHeight - (m_tabcontrol.contentRoot as Grid).RowDefinitions[0].ActualHeight;
                        if (oldContent != null)
                        {
                            oldanimation = mgeneric["PushSlide"] as Storyboard;
                            DoubleAnimationUsingKeyFrames animation = oldanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                            EasingDoubleKeyFrame keyframe = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                            EasingDoubleKeyFrame keyfrome0 = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                            if (m_tabcontrol.IsTouchTranform)
                            {
                                if (m_tabcontrol.IsTouchTransformReset)
                                {
                                    keyfrome0.Value = actualHeight - Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                }
                                else
                                {
                                    keyfrome0.Value = Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                }
                                keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                            }
                            else
                            {
                                keyfrome0.Value = 1;
                                keyframe.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                            }

                            keyframe.Value = actualHeight;
                            FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                            if (oldcontent != null && m_tabcontrol.oldContent != null)
                            {
                                oldcontent.Effect = null;
                                m_tabcontrol.frontOldContent.Content = oldcontent;

                                m_tabcontrol.oldContent.Content = null;
                                oldcontent.RenderTransform = new TranslateTransform();
                                Storyboard.SetTarget(oldanimation, oldcontent);

                                oldanimation.Begin();
                            }
                        }

                        if (newContent != null)
                        {
                            newanimation = mgeneric["PullSlide"] as Storyboard;
                            DoubleAnimationUsingKeyFrames animation = newanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                            EasingDoubleKeyFrame keyframe = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                            EasingDoubleKeyFrame keyframe1 = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                            if (m_tabcontrol.IsTouchTranform)
                            {
                                if (m_tabcontrol.IsTouchTransformReset)
                                {
                                    keyframe.Value = -Math.Abs(m_tabcontrol.TotalTransformValue.Y);
                                }
                                else
                                {
                                    keyframe.Value = -(actualHeight - Math.Abs(m_tabcontrol.TotalTransformValue.Y));
                                }
                                keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(0.2).Ticks));
                            }
                            else
                            {
                                keyframe.Value = -actualHeight;
                                keyframe1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(TimeSpan.FromSeconds(1).Ticks));
                            }

                            newContent.RenderTransform = new TranslateTransform();

                            UIElement newcontent = newContent.Content as UIElement;
                            if (newcontent != null)
                            {
                                newcontent.RenderTransform = new TranslateTransform();
                                Storyboard.SetTarget(newanimation, newcontent);
                                newanimation.Begin();
                            }
                        }

                        break;
                    }

                case TransitionEffects.Wipe:
                    {
                        if (oldContent != null)
                        {
                            FrameworkElement oldcontent = oldContent.Content as FrameworkElement;
                            m_tabcontrol.frontOldContent.Content = oldcontent;
                            m_tabcontrol.oldContent.Content = null;
                            oldanimation = mgeneric["Wipe"] as Storyboard;
                            oldcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            oldcontent.RenderTransform = new ScaleTransform();
                            Storyboard.SetTarget(oldanimation, oldcontent);
                            oldanimation.Begin();
                        }
                        if (newContent != null)
                        {
                        }
                        break;
                    }
            }
            if (newanimation != null)
            {
                newanimation.Completed += new EventHandler(newanimation_Completed);
            }

            if (oldanimation != null)
            {
                oldanimation.Completed += new EventHandler(oldanimation_Completed);
            }
        }

        private void oldanimation_Completed(object sender, EventArgs e)
        {
            HideFakeElements();
            oldanimation.Completed -= new EventHandler(oldanimation_Completed);
        }

        private void newanimation_Completed(object sender, EventArgs e)
        {
            HideFakeElements();
            newanimation.Completed -= new EventHandler(newanimation_Completed);
        }

        private void HideFakeElements()
        {
            if (m_tabcontrol != null)
            {
                if (m_tabcontrol.oldContent != null)
                    m_tabcontrol.oldContent.Opacity = 0;
                if (m_tabcontrol.frontOldContent != null)
                    m_tabcontrol.frontOldContent.Opacity = 0;
            }
        }

        public UIElement Clone(UIElement element)
        {
            UIElement returnImage = null;
            if (element != null)
            {
                VisualBrush v = new VisualBrush(element as Visual);
                Rectangle a = new Rectangle();
                a.Fill = v;
                returnImage = a;
            }
            return returnImage;
        }
    }
}