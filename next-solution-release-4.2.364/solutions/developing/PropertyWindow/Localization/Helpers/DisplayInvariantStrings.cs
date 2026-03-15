using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindscape.WpfElements.PropertyEditing;

namespace PropertyControl.Localization
{
    public class DisplayInvariantStrings
    {
        private Node _node;
        public DisplayInvariantStrings(Node node)
        {
            _node = node;
        }

        public string DisplayName => _node.Property.DisplayName;
        public  string Category => _node.Property.Category;
        public  string Description => _node.Property.Description;
        public int CategoryPriority => _node.CategoryPriority == int.MaxValue ? -1 : _node.CategoryPriority;
        public int Priority => _node.Priority == int.MaxValue || _node.Priority == int.MaxValue - 1 ? 0 : _node.Priority;
    }
}
