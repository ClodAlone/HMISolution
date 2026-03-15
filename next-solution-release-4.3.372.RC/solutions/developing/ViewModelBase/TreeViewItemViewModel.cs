using System;
using System.Collections.Generic;
using Utilities;

namespace ViewModelLib
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : ViewModelBase
    {
        #region Data
        protected static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        SafeObservableCollection<TreeViewItemViewModel> _children;
        readonly TreeViewItemViewModel _parent;

        bool _isExpanded;
        bool _isSelected;
        bool _lazyLoadChildren;
        #endregion // Data

        #region Constructors

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _lazyLoadChildren = lazyLoadChildren;
            if (lazyLoadChildren)
            {
                Children.Add(DummyChild);
            }
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }

        #endregion // Constructors

        #region Children
        public bool HasChildren
        {
            get
            {
                lock (lockObject)
                {
                    return _children != null && _children.Count > 0;
                }
            }
        }
        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public SafeObservableCollection<TreeViewItemViewModel> Children
        {
            get 
            {
                lock (lockObject)
                {
                    if (_children == null)
                        _children = new SafeObservableCollection<TreeViewItemViewModel>();
                }

                return _children; 
            }
        }
        #endregion // Children

        #region HasLoadedChildren
        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get 
            {
                lock (lockObject)
                {
                    return this.Children.Count == 1 && this.Children[0] == DummyChild;
                }
            }
        }
        #endregion // HasLoadedChildren

        #region IsExpanded
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (_isExpanded && this.HasDummyChild)
                {
                    lock (lockObject)
                    {
                        this.Children.Remove(DummyChild);
                    }
                    this.LoadChildren();
                }
                else if (!_isExpanded && !this.HasDummyChild && _lazyLoadChildren)
                {
                    lock (lockObject)
                    {
                        this.Children.Add(DummyChild);
                    }
                }
            }
        }
        #endregion // IsExpanded

        #region IsSelected
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }
        #endregion // IsSelected

        #region LoadChildren
        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        protected override void OnDispose()
        {
            List<TreeViewItemViewModel> children = null;
            lock (lockObject)
            {
                if (_children != null)
                    children = new List<TreeViewItemViewModel>(Children);
            }
            if (children != null)
            {
                foreach (TreeViewItemViewModel var in children)
                    var.OnDispose();
            }
            base.OnDispose();
        }
        #endregion // LoadChildren

        #region Parent
        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }
        #endregion // Parent

#if !WINDOWS_UWP && !NET_STANDARD

#region Validations
        public override string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
#endregion
#endif
    }
}