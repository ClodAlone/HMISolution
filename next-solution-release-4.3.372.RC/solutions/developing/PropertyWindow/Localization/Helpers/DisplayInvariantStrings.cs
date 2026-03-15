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
        public DisplayInvariantStrings(Node node, string projectTypeToHide)
        {
            _node = node;
            ProjectTypeToHide = projectTypeToHide;
        }

        public string DisplayName => _node.Property.DisplayName;
        public  string Category => _node.Property.Category;
        public  string Description => _node.Property.Description;
        public int CategoryPriority => (_node.CategoryPriority == PropertyControlUI.maxCategoryPriority || _node.CategoryPriority == PropertyControlUI.maxAdvancedCategoryPriority) ? -1 : _node.CategoryPriority;
        public int Priority => _node.Priority == int.MaxValue || _node.Priority == int.MaxValue - 1 ? 0 : _node.Priority;
        public int AdvPropertyPriority => _node.AdvPropertyPriority;
        public string ProjectTypeToHide;
    }
}
