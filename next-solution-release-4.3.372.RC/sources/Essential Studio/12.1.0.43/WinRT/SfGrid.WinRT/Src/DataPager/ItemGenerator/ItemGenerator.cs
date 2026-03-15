#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using System.Text;
using System.Diagnostics;

#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    public class ItemGenerator :NotifyPropertyChangeHelper, IItemGenerator
    {
        #region Private Members

        public List<ScrollableElement> Items;
        private SfDataPager dataPager;
        private Brush highlightThemeBrush;
        private Brush highlightForegroundBrush;

        #endregion

        #region Internal Members

        internal int ActualStartPageIndex = 0;
        internal int ActualEndPageIndex = 0;

        internal int VirtualStartIndex = 0;
        internal int VirtualEndIndex = 0;

        #endregion

        #region Public Members

        public Brush HighlightThemeBrush
        {
            get { return highlightThemeBrush; }
            set { highlightThemeBrush = value; OnPropertyChanged("HighlightThemeBrush"); }
        }

        public Brush HighlightForegroundBrush
        {
            get { return highlightForegroundBrush; }
            set { highlightForegroundBrush = value; OnPropertyChanged("HighlightForegroundBrush"); }
        }

        #endregion

        #region Ctor

        public ItemGenerator(SfDataPager pager)
        {
            Items = new List<ScrollableElement>();
            dataPager = pager;
        }

        #endregion

        #region IItemGenerator Members

        IList<IScrollableElement> IItemGenerator.Items
        {
            get { return Items.Cast<IScrollableElement>().ToList(); }
        }

        public void PreGenerateItems(int startIndex, int endIndex)
        {
            if (VirtualStartIndex <= 0)
                VirtualStartIndex = startIndex;
            if (VirtualEndIndex <= 0)
                VirtualEndIndex = endIndex;

            this.ActualStartPageIndex = this.GetActualStartIndex(startIndex);
            this.ActualEndPageIndex = this.GetActualEndIndex(endIndex);
            if (this.Items.Count == 0)
            {
                for (int index = ActualStartPageIndex; index <= ActualEndPageIndex; index++)
                {
                    var element = this.CreateItem(index);
                    this.Items.Add(element);
                }

                this.GenerateEllipsisElement(this.dataPager.AutoEllipsisMode);
            }
        }

        public void EnsureItems(int startIndex, int endIndex, bool isInternalOffset)
        {
            if (this.dataPager.InManipulation || this.ChangeVirtualIndexes() || !isInternalOffset)
            {
                this.VirtualStartIndex = startIndex;
                this.VirtualEndIndex = endIndex;
            }

            if (VirtualEndIndex > this.dataPager.PageCount - 1)
            {
                VirtualEndIndex = this.dataPager.PageCount - 1;
                VirtualStartIndex = VirtualEndIndex - (Math.Min(this.dataPager.NumericButtonCount,
                                                                this.dataPager.PageCount) - 1);
            }

            this.ActualStartPageIndex = this.GetActualStartIndex(this.VirtualStartIndex);
            this.ActualEndPageIndex = this.GetActualEndIndex(this.VirtualEndIndex);
            this.Items.ForEach(item => item.IsEnsured = false);
         
            var elipsisElements = this.Items.Where(item => item.IsElipsisElement);
            if (elipsisElements.Count() == 0 && this.dataPager.AutoEllipsisMode != AutoEllipsisMode.None)
            {
                this.GenerateEllipsisElement(this.dataPager.AutoEllipsisMode);
            }
            else
            {
                elipsisElements.ForEach(item => { item.IsEnsured = false; item.ElipsisPosition = ElipsisPosition.None; });

                switch (dataPager.AutoEllipsisMode)
                {
                    case AutoEllipsisMode.Before:
                        {
                            var element = elipsisElements.FirstOrDefault();
                            element.ElipsisPosition = ElipsisPosition.Left;
                        }
                        break;
                    case AutoEllipsisMode.After:
                        {
                            var element = elipsisElements.FirstOrDefault();
                            element.ElipsisPosition = ElipsisPosition.Right;
                        }
                        break;
                    case AutoEllipsisMode.Both:
                        {
                            var lastPosition= ElipsisPosition.None;
                            for (int index = 0; index < 2; index++)
                            {
                                if (elipsisElements.Any(item => item.ElipsisPosition == ElipsisPosition.None))
                                {
                                    var element = elipsisElements.FirstOrDefault(item => item.ElipsisPosition == ElipsisPosition.None);
                                    element.ElipsisPosition = lastPosition == ElipsisPosition.Right ? ElipsisPosition.Left : ElipsisPosition.Right;
                                    lastPosition = element.ElipsisPosition;
                                }
                                else
                                {
                                    this.GenerateEllipsisElement(AutoEllipsisMode.Before);
                                    elipsisElements = this.Items.Where(item => item.IsElipsisElement);                
                                }
                            }
                        }
                        break;
                }
            }


            foreach (var element in elipsisElements)
            {
                if (element.ElipsisPosition == ElipsisPosition.Right)
                {
                    if (this.VirtualEndIndex >= this.dataPager.PageCount - 1)
                    {
                        this.ActualEndPageIndex += 1;
                        element.Index = -1;
                        element.IsEnsured = false;
                    }
                    else
                    {
                        element.IsEnsured = true;
                        element.Index = this.VirtualEndIndex;
                        element.Element.Visibility = Visibility.Visible;
                    }
                }
                else if(element.ElipsisPosition== ElipsisPosition.Left)
                {
                    if (this.VirtualStartIndex < 2)
                    {
                        this.ActualStartPageIndex -= 1;
                        element.Index = -1;
                        element.IsEnsured = false;
                    }
                    else
                    {
                        element.IsEnsured = true;
                        element.Index = this.VirtualStartIndex;
                        element.Element.Visibility = Visibility.Visible;
                    }
                }
                else if (element.ElipsisPosition == ElipsisPosition.None)
                {
                    element.Index = -1;
                    element.IsEnsured = false;                    
                }
            }

            for (int index = this.ActualStartPageIndex; index <= this.ActualEndPageIndex; index++)
            {
                if (this.Items.All(item => item.Index != index))
                {
                    if (
                        this.Items.Any(
                            item =>
                            (item.Index < 0 || item.Index < startIndex || item.Index > endIndex) && !item.IsEnsured))
                    {
                        var elements =
                            this.Items.Where(
                                item =>
                                ((item.Index < 0 || item.Index < startIndex || item.Index > endIndex) && !item.IsEnsured &&
                                 !item.IsElipsisElement)).ToList();
                        if (elements != null && elements.Count > 0)
                        {
                            UpdateItems(elements, index);
                        }
                    }
                }

                var element = this.Items.FirstOrDefault(item => item.Index == index);
                if (element != null)
                {
                    if (index == this.dataPager.PageIndex)
                    {
                        (element.Element as NumericButton).IsCurrentPage = true;
                    }
                    if (element.Element.Visibility == Visibility.Collapsed)
                        element.Element.Visibility = Visibility.Visible;
                    element.IsEnsured = true;
                }
                else
                {
                    var newElement = CreateItem(index);
                    if (index == this.dataPager.PageIndex)
                    {
                        (newElement.Element as NumericButton).IsCurrentPage = true;
                    }
                    newElement.IsEnsured = true;
                    this.Items.Add(newElement);
                }
            }

            this.Items.ForEach(item =>
                {
                    if (!item.IsEnsured)
                    {
                        item.IsEnsured = true;
                        item.Element.Visibility = Visibility.Collapsed;
                    }
                });
        }

        #endregion

        #region Helper Methods

        private ScrollableElement CreateItem(int index)
        {
            ScrollableElement element = new ScrollableElement();
            element.Index = index;
            element.InitializeElement(this.dataPager.NumericButtonStyle, this.dataPager.AutoEllipsisText);
            this.SetBinding(element);
            return element;
        }

        private void GenerateEllipsisElement(AutoEllipsisMode mode)
        {
            if (mode == AutoEllipsisMode.After)
            {
                var element = this.CreateElipsisElement(ElipsisPosition.Right, this.ActualEndPageIndex+1);
                this.Items.Add(element);
            }
            else if (mode == AutoEllipsisMode.Before)
            {
                var element = this.CreateElipsisElement(ElipsisPosition.Left, -1);
                this.Items.Add(element);
            }
            else if (mode == AutoEllipsisMode.Both)
            {
                var  elementRight= this.CreateElipsisElement(ElipsisPosition.Right, this.ActualEndPageIndex+1);
                this.Items.Add(elementRight);

                var elementLeft= this.CreateElipsisElement(ElipsisPosition.Left, -1);
                this.Items.Add(elementLeft);
            }
        }

        private ScrollableElement CreateElipsisElement(ElipsisPosition position, int index)
        {
            ScrollableElement element = new ScrollableElement();
            element.Index = index;
            element.IsEnsured = true;
            element.IsElipsisElement = true;
            element.ElipsisPosition = position;
            element.InitializeElement(this.dataPager.NumericButtonStyle, this.dataPager.AutoEllipsisText);
            return element;
        }

        private void UpdateItems(List<ScrollableElement> elements, int index)
        {
            var element = elements.FirstOrDefault();
            if (element.Index < 0 || element.Index > dataPager.PageCount)
            {
                element.Element.Visibility = Visibility.Collapsed;
            }
            else
            {
                element.Index = index;
                element.UpdateElement();
            }
        }

        private int GetActualStartIndex(int startIndex)
        {
            if ((this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Before ||
                 this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Both))
                return startIndex + 1;
            return startIndex;
        }

        private int GetActualEndIndex(int endIndex)
        {
            if (this.dataPager.AutoEllipsisMode == AutoEllipsisMode.After ||
                this.dataPager.AutoEllipsisMode == AutoEllipsisMode.Both)
                return endIndex - 1;
            return endIndex;
        }

        private bool ChangeVirtualIndexes()
        {
            if ((VirtualEndIndex - VirtualStartIndex) < (Math.Min(this.dataPager.NumericButtonCount,
                                                                  this.dataPager.PageCount) - 1) 
               || (VirtualEndIndex - VirtualStartIndex) > (Math.Min(this.dataPager.NumericButtonCount,this.dataPager.PageCount) - 1)  )
            {
                return true;
            }
            return false;
        }

        private void SetBinding(ScrollableElement element)
        {
            var bind= new Binding();
            bind.Path = new PropertyPath("HighlightThemeBrush");
            bind.Source = this;
            bind.Mode= BindingMode.TwoWay;
            element.Element.SetBinding(NumericButton.HighlightingThemeBrushProperty,bind);

            bind=new Binding();
            bind.Path = new PropertyPath("HighlightForegroundBrush");
            bind.Source = this;
            bind.Mode = BindingMode.TwoWay;
            element.Element.SetBinding(NumericButton.HighlightForegroundBrushProperty, bind);
        }

        #endregion
    }
}