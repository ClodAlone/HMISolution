using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using LinqToTree;
using System.Windows.Controls;

namespace LinqToTreeWPF
{
            //// get all the TextBox's which have a Grid as direct parent
            //var itemsFluent = new VisualTreeAdapter(this).Descendants()
            //                                       .Where(i => i.Ancestors().First().Item is Grid)
            //                                       .Where(i => i.Item is TextBox)
            //                                       .Select(i => i.Item);

            //var itemsQuery = from v in new VisualTreeAdapter(this).Descendants()
            //                 where v.Ancestors().First().Item is Grid && v.Item is TextBox
            //                 select v.Item;

            //// get all the StackPanels that are within another StackPanel visual tree
            //var items2Fluent = new VisualTreeAdapter(this).Descendants()
            //                                       .Where(i => i.Item is StackPanel)
            //                                       .Descendants()
            //                                       .Where(i => i.Item is StackPanel)
            //                                       .Select(i => i.Item);

            //var items2Query = from i in
            //                      (from v in new VisualTreeAdapter(this).Descendants()
            //                       where v.Item is StackPanel
            //                       select v).Descendants()
            //                  where i.Item is StackPanel
            //                  select i.Item;

            //// find all descendant text boxes
            //var textBoxes = new VisualTreeAdapter(this).Descendants<DependencyObject, TextBox>();

            //string tree = this.DescendantsAndSelf().Aggregate("",
            //    (bc, n) => bc + n.Ancestors().Aggregate("", 
            //      (ac, m) => (m.ElementsAfterSelf().Any() ? "| " : "  ") + ac,
            //    ac => ac + (n.ElementsAfterSelf().Any() ? "+-" : "\\-")) + 
            //      n.GetType().Name + "\n");
    /// <summary>
    /// An adapter for DependencyObject which implements ILinqToTree in
    /// order to allow Linq queries on the visual tree
    /// </summary>
    public class VisualTreeAdapter : ILinqToTree<DependencyObject>
    {
        private DependencyObject _item;

        public VisualTreeAdapter(DependencyObject item)
        {
            _item = item;
        }

        public IEnumerable<ILinqToTree<DependencyObject>> Children()
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(_item);
            for (int i = 0; i < childrenCount; i++)
            {
                yield return new VisualTreeAdapter(VisualTreeHelper.GetChild(_item, i));
            }
        }

        public ILinqToTree<DependencyObject> Parent
        {
            get
            {
                return new VisualTreeAdapter(VisualTreeHelper.GetParent(_item));
            }
        }

        public DependencyObject Item
        {
            get
            {
                return _item;
            }
        }

    }

}
