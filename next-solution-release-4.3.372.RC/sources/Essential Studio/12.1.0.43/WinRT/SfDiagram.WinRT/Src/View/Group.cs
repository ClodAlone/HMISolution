#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media; 
#else
using System.Windows.Data; 
#endif
using Syncfusion.UI.Xaml.Diagram.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Diagram
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public partial class Group : 
        Node,
        IGroupView,
        IGroup
    {
#if WPF
        static Group()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Group), new FrameworkPropertyMetadata(typeof(Group)));
        } 
#endif

        public Group()
        {
#if !WPF
            DefaultStyleKey = typeof(Group); 
#endif
            //this.GroupTransform = new CompositeTransform();

            //Binding bind = new Binding();
            //bind.Path = new PropertyPath("GroupTransform");
            //bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
            //this.SetBinding(RenderTransformProperty, bind);

            //bind = new Binding();
            //bind.Path = new PropertyPath("Pivot");
            //bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
            //this.SetBinding(RenderTransformOriginProperty, bind);


            //bind = new Binding();
            //bind.Path = new PropertyPath("Nodes");
            //bind.RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
            //this.SetBinding(Group.NodesProperty, bind);
        }

        //public CompositeTransform GroupTransform
        //{
        //    get { return (CompositeTransform)GetValue(GroupTransformProperty); }
        //    set { SetValue(GroupTransformProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for NodeTransform.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty GroupTransformProperty =
        //    DependencyProperty.Register("GroupTransform", typeof(CompositeTransform), typeof(Group), new PropertyMetadata(null));

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = base.MeasureOverride(availableSize);
            desiredSize = (Wrapper as IProtectedGroup).UpdateBounds();
            Wrapper.SetDesiredSize(desiredSize);
            return desiredSize;
            //return new Size(0, 0);
        }

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    Size actualSize = base.ArrangeOverride(finalSize);
        //    Wrapper.SetActualSize(actualSize);
        //    Wrapper.UpdateCompositeTrans();
        //    return actualSize;
        //}

        private void BindToINode(IGroup source)
        {
            if (source != null)
            {
                Binding bind;
                foreach (var property in GroupConstants.GroupProperties)
                {
                    bind = new Binding();
                    bind.Path = new PropertyPath(property.Item2);
                    bind.Mode = BindingMode.TwoWay;
                    //bind.Source = source;
                    
                    if (ReadLocalValue(property.Item1) == DependencyProperty.UnsetValue)
                    {
                        this.SetBinding(property.Item1, bind);
                    }
                }
            }
        }

        private object BusinessObject { get; set; }

        void IView.SetBusinessObject(object item)
        {
            this.BusinessObject = item;
            if (AutoBind && BusinessObject is IGroup)
            {
                BindToINode(BusinessObject as IGroup);
            }
            if (this is Selector && AutoBind && BusinessObject is ISelector)
            {
                Binding bind = new Binding();
                bind.Path = new PropertyPath(SelectorConstants.QuickCommands);
                bind.Source = item;
                this.SetBinding(Selector.QuickCommandsProperty, bind);
            }
        }

        private void OnNodesChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        private void OnConnectorsChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        private void OnGroupsChanged(DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
