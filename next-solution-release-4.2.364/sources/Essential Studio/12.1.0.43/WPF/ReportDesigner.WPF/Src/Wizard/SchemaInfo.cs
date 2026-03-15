//-------------------------------------------------------------------------------------------------
// <copyright file="SchemaInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    internal class SchemaInfo : ICloneable, INotifyPropertyChanged
    {
        private bool _isSelected;
        public SchemaInfo()
        {
            this.SchemaInfos = new List<SchemaInfo>();
            this.TreeNodeType = NodeType.Folder;
            this.IsSelected = false;
            this.DataType = string.Empty;
            //this.IsFolder = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Key { get; set; }

        public List<SchemaInfo> SchemaInfos { get; set; }

        public NodeType TreeNodeType { get; set; }

        public string DataType { get; set; }

        public object ImageSource { get; set; }

        public override string ToString()
        {
            if (this.Key.Length > 0)
            {
                return this.Key;
            }
            return string.Empty;
        }

        public bool IsSelected 
        {
            get
            { 
                return _isSelected; 
            }
            set
            {
                _isSelected = value;
                if ((this.TreeNodeType == NodeType.Table || this.TreeNodeType == NodeType.View )&& this.Key != "Table")
                {
                    //if (!IsChildNodeSelected())
                    //{
                        foreach (var item in this.SchemaInfos)
                        {
                            item._isSelected = value;
                            item.OnPropertyChanged("IsSelected");
                        }
                    //}
                }
                else if (this.TreeNodeType == NodeType.TableColumn)
                {
                    if (this.Parent != null)
                    {
                        if (!value)
                        {
                            if (!IsChildNodeSelected(this.Parent))
                            {
                                this.Parent._isSelected = value;
                                this.Parent.OnPropertyChanged("IsSelected");
                            }
                        }
                        else
                        {
                            if (IsChildNodeSelected(this.Parent))
                            {
                                this.Parent._isSelected = value;
                                this.Parent.OnPropertyChanged("IsSelected");
                            }
                        }
                    }
                }
                this.OnPropertyChanged("IsSelected");
            }
        }

        private bool IsChildNodeSelected(SchemaInfo schemaInfo)
        {
            foreach (var item in schemaInfo.SchemaInfos)
            {
                if (item._isSelected)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method returns only the selected table
        /// </summary>
        /// <param name="schemaInfo"></param>
        /// <returns></returns>
        public SchemaInfo GetSelectedNode(SchemaInfo schemaInfo)
        {
            foreach (var item in schemaInfo.SchemaInfos)
            {
                if (item.TreeNodeType == NodeType.TableColumn && item.IsSelected)
                {
                    return item.Parent;
                }
                else if ((item.TreeNodeType == NodeType.Table || item.TreeNodeType == NodeType.View) && item.IsSelected && item.Key != "Table" && item.Key != "Views")
                {
                    return item;
                }
                SchemaInfo schema = GetSelectedNode(item);
                if (schema != null)
                {
                    return schema;
                }
                else
                {
                    continue;
                }
            }
            return null;
        }

        public SchemaInfo GetParentSchema(SchemaInfo schemaInfo)
        {
            if (schemaInfo.Parent == null)
            {
                return schemaInfo;
            }
            else
            {
                return GetParentSchema(schemaInfo.Parent);
            }
        }

        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }   
        }

        public object Clone()
        {
            SchemaInfo schemaInfo = new SchemaInfo();
            schemaInfo.IsSelected = this.IsSelected;
            schemaInfo.Key = this.Key;
            schemaInfo.Parent = this.Parent;
            schemaInfo.SchemaInfos = this.SchemaInfos;
            schemaInfo.TreeNodeType = this.TreeNodeType;
            return schemaInfo;
        }

        public SchemaInfo Parent { get; set; }
    }
}
