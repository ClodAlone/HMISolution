using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Utilities
{
    /*
        int count = 20;
        var circles = new LambdaCollection<Ellipse>(count)
          .WithXY(i => 100.0 + (4.0 * i * Math.Sin(i / 4.0 * (Math.PI))),
                  i => 100.0 + (4.0 * i * Math.Cos(i / 4.0 * (Math.PI))))
          .WithProperty(WidthProperty, i => 1.5 * i)
          .WithProperty(HeightProperty, i => 1.5 * i)
          .WithProperty(Shape.FillProperty, i => new SolidColorBrush(
            Color.FromArgb(255, 0, 0, (byte)(255 - (byte)(12.5 * i)))));
        foreach (var circle in circles)
          MyCanvas.Children.Add(circle);
     */

    public class LambdaCollection<T> : Collection<T> where T : DependencyObject, new()
    {
        public LambdaCollection(int count) { while (count-- > 0) Add(new T()); }

        public LambdaCollection<T> WithProperty<U>(DependencyProperty property, Func<int, U> generator)
        {
            for (int i = 0; i < Count; ++i)
                this[i].SetValue(property, generator(i));
            return this;
        }

        public LambdaCollection<T> WithXY<U>(Func<int, U> xGenerator, Func<int, U> yGenerator)
        {
            for (int i = 0; i < Count; ++i)
            {
                this[i].SetValue(Canvas.LeftProperty, xGenerator(i));
                this[i].SetValue(Canvas.TopProperty, yGenerator(i));
            }
            return this;
        }
    }

    public class LambdaDoubleAnimation : DoubleAnimation
    {
        public Func<double, double> ValueGenerator { get; set; }
        protected override double GetCurrentValueCore(double origin, double dst, AnimationClock clock)
        {
            return ValueGenerator(base.GetCurrentValueCore(origin, dst, clock));
        }
    }

    /*
        var c = new LambdaDoubleAnimationCollection(
          circles.Count, 
          i => 10.0 * i, 
          i => new Duration(TimeSpan.FromSeconds(2)),
          i => j => 100.0 / j);
        c.BeginApplyAnimation(circles.Cast<UIElement>().ToArray(), Canvas.LeftProperty);
     */
    public class LambdaDoubleAnimationCollection : Collection<LambdaDoubleAnimation>
    {
        public LambdaDoubleAnimationCollection(int count, Func<int, double> from, Func<int, double> to,
        Func<int, Duration> duration, Func<int, Func<double, double>> valueGenerator)
        {
            for (int i = 0; i < count; ++i)
            {
                var lda = new LambdaDoubleAnimation
                {
                    From = from(i), 
                    To = to(i), 
                    Duration = duration(i),
                    ValueGenerator = valueGenerator(i)
                };
                Add(lda);
            }
        }

        //public void BeginApplyAnimation(UIElement [] targets, DependencyProperty property)
        //{
        //    for (int i = 0; i < Count; ++i)
        //        targets[i].BeginAnimation(property, Items[i]);
        //}

        public void BeginApplyAnimation(UIElement[] targets, DependencyProperty property)
        {
            for (int i = 0; i < Count; ++i)
            {
                Items[i].BeginTime = new TimeSpan(0);
                targets[i].BeginAnimation(property, Items[i]);
            }
        }
        public void BeginSequentialAnimation(UIElement[] targets, DependencyProperty property)
        {
            TimeSpan acc = new TimeSpan(0);
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].BeginTime = acc;
                acc += Items[i].Duration.TimeSpan;
            }
            for (int i = 0; i < Count; ++i)
            {
                targets[i].BeginAnimation(property, Items[i]);
            }
        }
    }
}
