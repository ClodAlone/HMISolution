#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Syncfusion.Windows.Data;
    using System.Xml.Serialization;
    using System.Windows;
    using Syncfusion.Windows.Styles;
    using System.Collections;

    /// <summary>
    /// Provides the data for relations in <see cref="GridDataControl"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataRelation
#if !SILVERLIGHT
        :Freezable, IRelationDefinition
#else
 : DependencyObject, IRelationDefinition
#endif

    {
        #region ..ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataRelation"/> class.
        /// </summary>
        public GridDataRelation()
        {
            ////this.TableProperties = new GridDataTableProperties();
            this.TableProperties = new GridDataTableProperties();
        }

        #endregion

        #region RelationalColumn
        /// <summary>
        /// DependencyProperty for <see cref = "GridDataRelation.RelationalColumn"/>.
        /// </summary>
        public readonly static DependencyProperty RelationalColumnProperty =
            DependencyProperty.Register("RelationalColumn", typeof(string), typeof(GridDataRelation), null);

        /// <summary>
        /// Gets or sets the parent table relational column.
        /// </summary>
        /// <value>The parent table relational column.</value>
        public string RelationalColumn
        {
            get
            {
                return (string)GetValue(GridDataRelation.RelationalColumnProperty);
            }
            set
            {
                SetValue(GridDataRelation.RelationalColumnProperty, value);
            }
        }

        #endregion

        #region Child Relational Column

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataRelation.ChildRelationalColumn"/>.
        /// </summary>
        public readonly static DependencyProperty ChildRelationalColumnProperty =
            DependencyProperty.Register("ChildRelationalColumn", typeof(string), typeof(GridDataRelation), null);


        /// <summary>
        ///  Gets or sets the child table relational column.
        /// </summary>
        /// <value>The child table relational column.</value>
        public string ChildRelationalColumn
        {
            get
            {
                return (string)GetValue(GridDataRelation.ChildRelationalColumnProperty);
            }
            set
            {
                SetValue(GridDataRelation.ChildRelationalColumnProperty, value);
            }
        }

        #endregion

        #region Child ItemsSource

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataRelation.ChildItemsSource"/>.
        /// </summary>
        public readonly static DependencyProperty ChildItemsSourceProperty =
            DependencyProperty.Register("ChildItemsSource", typeof(object), typeof(GridDataRelation), null);

        /// <summary>
        /// Gets or sets the child items source.
        /// </summary>
        /// <value>The child items source.</value>
        public object ChildItemsSource
        {
            get
            {
                return this.GetValue(GridDataRelation.ChildItemsSourceProperty);
            }

            set
            {
                this.SetValue(GridDataRelation.ChildItemsSourceProperty, value);
            }
        }

        #endregion

        #region Relation Type

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataRelation.RelationType"/>.
        /// </summary>
        private static readonly DependencyProperty RelationTypeProperty =
            DependencyProperty.Register("RelationType", typeof(RelationType), typeof(GridDataRelation), null);

        /// <summary>
        /// Gets or sets the type of the relation.
        /// </summary>
        /// <value>The type of the relation.</value>
        public RelationType RelationType
        {
            get
            {
                return (RelationType)GetValue(GridDataRelation.RelationTypeProperty);
            }
            set
            {
                SetValue(GridDataRelation.RelationTypeProperty, value);
            }
        }

        #endregion

        #region TableProperties

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataRelation.TableProperties"/>.
        /// </summary>
        private static DependencyProperty TablePropertiesProperty =
            DependencyProperty.Register("TableProperties", typeof(GridDataTableProperties), typeof(GridDataRelation), null);

        /// <summary>
        /// Gets or sets the table properties.
        /// </summary>
        /// <value>The table properties.</value>
        public GridDataTableProperties TableProperties
        {
            get
            {
                return (GridDataTableProperties)GetValue(GridDataRelation.TablePropertiesProperty);
            }

            set
            {
                SetValue(GridDataRelation.TablePropertiesProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Initializes from another instance of a <see cref="GridDataRelation"/>.
        /// </summary>
        /// <param name="r">The r.</param>
        public void InitializeFrom(GridDataRelation r)
        {
            this.RelationalColumn = r.RelationalColumn;
            this.RelationType = r.RelationType;
            this.TableProperties = r.TableProperties;
        }

#if !SILVERLIGHT
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
