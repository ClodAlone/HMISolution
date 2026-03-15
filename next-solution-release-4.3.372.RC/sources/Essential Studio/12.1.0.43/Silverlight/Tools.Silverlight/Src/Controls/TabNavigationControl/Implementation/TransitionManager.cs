#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public static class TransitionManager
    {
        private static System.Windows.ResourceDictionary mgeneric;

        private static Storyboard oldanimation;

        private static Storyboard newanimation;

        private static TabNavigationControl tabcontrol;

        static TransitionManager()
        {
            mgeneric = new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Tools.Silverlight;component/Controls/TabNavigationControl/Themes/TransitionEffects.xaml", UriKind.RelativeOrAbsolute) };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        /// <param name="effect"></param>
        public static void PlayTransitionEffect(TabNavigationControl control, TabNavigationItem  oldContent, TabNavigationItem newContent, TransitionEffects effect)
        {
            if (oldanimation != null)
            {
                oldanimation.Stop();
            }

            if (newanimation != null)
            {
                newanimation.Stop();
            }

            tabcontrol = control;
            if (tabcontrol != null && tabcontrol.frontOldContent != null && tabcontrol.oldContent != null)
            {
                tabcontrol.frontOldContent.Opacity = 1;
                tabcontrol.oldContent.Opacity = 1;
            }
            control.selectedContent.IsEnabled = newContent.IsEnabled;
            switch (effect)
            {
                case TransitionEffects.Slide:
                    {
                        if (control.Items.IndexOf(oldContent) < control.Items.IndexOf(newContent))
                        {
                            if (oldContent != null)
                            {
                                oldanimation = mgeneric["NextOldSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = oldanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[1] as EasingDoubleKeyFrame;
                                keyframe.Value = -control.ActualWidth;
                                Image oldcontent = Clone(oldContent.Content as UIElement);
                                if (oldcontent != null && control.oldContent != null)
                                {
                                    oldcontent.Effect = null;
                                    control.frontOldContent.Content = null;
                                    control.oldContent.Content = oldcontent;
                                    oldcontent.RenderTransform = new CompositeTransform();
                                    Storyboard.SetTarget(oldanimation, oldcontent);
                                    oldanimation.Begin();
                                }
                            }

                            if (newContent != null)
                            {
                                newanimation = mgeneric["NextNewSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = newanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                keyframe.Value = control.ActualWidth;
                                newContent.RenderTransform = new CompositeTransform();
                                UIElement newcontent = newContent.Content as UIElement;
                                if (newcontent != null)
                                {
                                    newcontent.RenderTransform = new CompositeTransform();
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
                                keyframe.Value = control.ActualWidth;
                                Image oldcontent = Clone(oldContent.Content as UIElement);
                                if (oldcontent != null)
                                {
                                    oldcontent.Effect = null;
                                    control.frontOldContent.Content = null;
                                    control.oldContent.Content = oldcontent;
                                    oldcontent.RenderTransform = new CompositeTransform();
                                    Storyboard.SetTarget(oldanimation, oldcontent);
                                    oldanimation.Begin();
                                }
                            }

                            if (newContent != null)
                            {
                                newanimation = mgeneric["NextNewSlide"] as Storyboard;
                                DoubleAnimationUsingKeyFrames animation = newanimation.Children[0] as DoubleAnimationUsingKeyFrames;
                                EasingDoubleKeyFrame keyframe = animation.KeyFrames[0] as EasingDoubleKeyFrame;
                                keyframe.Value = -control.ActualWidth;
                                newContent.RenderTransform = new CompositeTransform();
                                UIElement newcontent = newContent.Content as UIElement;
                                if (newcontent != null)
                                {
                                    newcontent.RenderTransform = new CompositeTransform();
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
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            oldcontent.Effect = null;
                            control.oldContent.Content = oldcontent;
                            control.frontOldContent.Content = null;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["Fade"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }
                case TransitionEffects.NewsFlash:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            oldcontent.Effect = null;
                            control.oldContent.Content = oldcontent;
                            control.frontOldContent.Content = null;
                           // control.oldContent.Visibility = Visibility.Collapsed;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["NewsFlash"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            newcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            newcontent.RenderTransform = new CompositeTransform();
                            newcontent.Projection = new PlaneProjection();
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }
                case TransitionEffects.Zoom:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.oldContent.Content = oldcontent;
                            control.frontOldContent.Content = null;
                            oldcontent.Effect = null;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["Zoom"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            newcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            newcontent.RenderTransform = new CompositeTransform();
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }
                case TransitionEffects.Flip:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.frontOldContent.Content = oldcontent;
                            control.oldContent.Content = null;                                
                            oldcontent.Effect = null;
                            oldanimation = mgeneric["Flip"] as Storyboard;
                            oldcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            oldcontent.RenderTransform = new CompositeTransform();
                            oldcontent.Projection = new PlaneProjection();
                            Storyboard.SetTarget(oldanimation, oldcontent);
                            oldanimation.Begin();
                        }
                        if (newContent != null)
                        {
                            
                        }
                        break;
                    }
                case TransitionEffects.Blur:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.frontOldContent.Content = oldcontent;
                            control.oldContent.Content = null;
                            oldanimation = mgeneric["Blur"] as Storyboard;
                            oldcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            oldcontent.RenderTransform = new CompositeTransform();
                            oldcontent.Projection = new PlaneProjection();
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
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.oldContent.Content = oldcontent;
                            control.frontOldContent.Content = null;
                            oldcontent.Effect = null;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["Push"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            newcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            newcontent.RenderTransform = new CompositeTransform();
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }

                case TransitionEffects.PushIn:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.oldContent.Content = oldcontent;
                            control.frontOldContent.Content = null;
                            oldcontent.Effect = null;
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["PushIn"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;
                            newcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            newcontent.RenderTransform = new CompositeTransform();
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }
                case TransitionEffects.Uncover:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.frontOldContent.Content = oldcontent;
                            control.oldContent.Content = null;
                            oldcontent.Effect = null;
                            oldanimation = mgeneric["UnCover"] as Storyboard;                            
                            oldcontent.RenderTransform = new CompositeTransform();
                            oldcontent.Projection = new PlaneProjection();
                            Storyboard.SetTarget(oldanimation, oldcontent);
                            oldanimation.Begin();
                        }
                        if (newContent != null)
                        {
                            newanimation = mgeneric["Cover"] as Storyboard;
                            UIElement newcontent = newContent.Content as UIElement;                           
                            newcontent.RenderTransform = new CompositeTransform();
                            newcontent.Projection = new PlaneProjection();
                            Storyboard.SetTarget(newanimation, newcontent);
                            newanimation.Begin();
                        }
                        break;
                    }
                case TransitionEffects.Wipe:
                    {
                        if (oldContent != null)
                        {
                            Image oldcontent = Clone(oldContent.Content as UIElement);
                            control.frontOldContent.Content = oldcontent;
                            control.oldContent.Content = null;
                            oldanimation = mgeneric["Wipe"] as Storyboard;
                            oldcontent.RenderTransformOrigin = new Point(0.5, 0.5);
                            oldcontent.RenderTransform = new CompositeTransform();
                            oldcontent.Projection = new PlaneProjection();                            
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

        static void oldanimation_Completed(object sender, EventArgs e)
        {
            HideFakeElements();
            oldanimation.Completed -= new EventHandler(oldanimation_Completed);
        }

        static void newanimation_Completed(object sender, EventArgs e)
        {
            HideFakeElements();
            newanimation.Completed -= new EventHandler(newanimation_Completed);
        }

        private static void HideFakeElements()
        {
            if (tabcontrol != null)
            {
                tabcontrol.oldContent.Opacity = 0;
                tabcontrol.frontOldContent.Opacity = 0;

               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Image Clone(UIElement element)
        {
            Image returnImage = new Image();
            if (element != null)
            {
                ImageSource source = new WriteableBitmap(element, new TranslateTransform());
                returnImage.Source = source;
            }
            return returnImage;
        }
    }
}
