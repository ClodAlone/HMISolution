using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using UFRecipeLayout.LayoutItemControls;
using UFRecipeSettings.UFRecipeModel;
using UFRecipeSettings.Documents;
using UFUAModel.Extensions;
using Utilities.WPF;
using Utilities;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace UFRecipeLayout.Helpers
{
    public static class LayoutControlHelper
    {
        #region Declarations
        readonly static Object labelObjectTag = new Object();
        #endregion

        #region Layout Items
        public static FrameworkElements CompileLayoutItems(UFRecipeEntity root, LayoutRecipeEditor editor, bool bRunningOnServer = false)
        {
            var ret = new FrameworkElements();
            // recipe items
            ret.Add(LayoutControlHelper.CreateLayoutItemControl(root, editor, bRunningOnServer));
            foreach (var value in root.DataValues)
            {
                ret.Add(LayoutControlHelper.CreateLayoutItemControl(value));
            }

            // group items
            foreach (var group in root.Groups)
            {
                ret.Add(LayoutControlHelper.CreateLayoutItemControl(group));
            }

            return ret;
        }

        public static FrameworkElement CreateLayoutItemControl(UFRecipeEntity recipe, LayoutRecipeEditor editor, bool bRunningOnServer = false)
        {
            var reload = editor.GetBitmapImage("UFRLReload");
            var remove = editor.GetBitmapImage("UFRLRemove");
            var save = editor.GetBitmapImage("UFRLSave");
            var add = editor.GetBitmapImage("UFRLAdd");
            var import = editor.GetBitmapImage("UFRLImport");
            var export = editor.GetBitmapImage("UFRLExport");
            var read = editor.GetBitmapImage("UFRLRead");
            var write = editor.GetBitmapImage("UFRLWrite");

            var layoutgroup = new RecipeLayoutGroup()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.RecipeGroupCommands.ToString()),
                View = LayoutGroupView.GroupBox,
                Orientation = Orientation.Horizontal,
                Header = Properties.Resources.LayoutRecipeGroupCommands,
                Tag = recipe
            };

            // combo recipe index
            FrameworkElement uie = new ItemControlRecipeIndex() { RunningOnServer = bRunningOnServer };
            layoutgroup.Children.Add(new RecipeLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(recipe.NodeId.ToString()),
                Content = uie,
                Label = recipe.Name,
                ToolTip = !String.IsNullOrEmpty(recipe.Description) ? recipe.Description : null,
                HorizontalAlignment = HorizontalAlignment.Stretch
            });

            // reload command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(reload);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ReloadCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeReloadCommand,
                ToolTip = Properties.Resources.LayoutRecipeReloadCommandToolTip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // save command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(save);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.SaveCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeSaveCommand,
                ToolTip = Properties.Resources.LayoutRecipeSaveCommandToolTip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // new command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(add);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.NewCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeNewCommand,
                ToolTip = Properties.Resources.LayoutRecipeNewCommandToolTip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // remove command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(remove);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.RemoveCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeRemoveCommand,
                ToolTip = Properties.Resources.LayoutRecipeRemoveCommandToolTip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // Import command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(import);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ImportCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeImportCommand,
                ToolTip = Properties.Resources.LayoutRecipeImportCommandTooltip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // Export command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(export);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ExportCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeExportCommand,
                ToolTip = Properties.Resources.LayoutRecipeExportCommandTooltip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // read command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(read);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.ReadCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeReadCommand,
                ToolTip = Properties.Resources.LayoutRecipeReadCommandToolTip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            // write command
            uie = new ItemControlCommandButton();
            (uie as ICommandUI).SetIcon(write);
            layoutgroup.Children.Add(new RecipeCommandButtonLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(LayoutControlPersonalityGuids.WriteCommand.ToString()),
                Content = uie,
                Caption = Properties.Resources.LayoutRecipeWriteCommand, 
                ToolTip = Properties.Resources.LayoutRecipeWriteCommandToolTip,
                HorizontalAlignment = HorizontalAlignment.Right
            });

            return layoutgroup;
        }

        public static FrameworkElement CreateLayoutItemControl(UFGroupEntity group)
        {
            var layoutgroup = new RecipeLayoutGroup()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(group.NodeId.ToString()),
                View = LayoutGroupView.GroupBox,
                Orientation = Orientation.Vertical,
                Header = group.Name,
                ToolTip = !String.IsNullOrEmpty(group.Description) ? group.Description : null,
                Tag = group
            };

            foreach (var value in group.DataValues)
            {
                var itemcontrol = CreateLayoutItemControl(value);
                itemcontrol.VerticalAlignment = VerticalAlignment.Top;
                layoutgroup.Children.Add(itemcontrol);
            }

            return layoutgroup;
        }

        public static FrameworkElement CreateLayoutItemControl(UFDataValueEntity value)
        {
            EditValueControlTypeEnum controlType;
            if (value.DataType == UFUAModel.DataType.Boolean)
                controlType = EditValueControlTypeEnum.CheckBox;
            else
                controlType = EditValueControlTypeEnum.EditDisplay;

            return CreateLayoutItemControl(value, controlType);
        }

        public static FrameworkElement CreateLayoutItemControl(UFDataValueEntity value, EditValueControlTypeEnum controlType)
        {
            var layoutItem = new RecipeEditValueLayoutItem()
            {
                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(value.NodeId.ToString()),
                Content = CreateContentItemControl(value, controlType),
                Label = value.Name,
                ToolTip = !String.IsNullOrEmpty(value.Description) ? value.Description : null,
                DecimalDigits = value.DataType.IsDecimalType() ? 2 : 0,
                ControlType = controlType,
                Tag = value
            };

            layoutItem.SetContentValues();

            return layoutItem;
        }

        public static FrameworkElement CreateContentItemControl(UFDataValueEntity value)
        {
            if (value.DataType == UFUAModel.DataType.Boolean)
                return CreateContentItemControl(value, EditValueControlTypeEnum.CheckBox);
            else
                return CreateContentItemControl(value, EditValueControlTypeEnum.EditDisplay);
        }

        public static FrameworkElement CreateContentItemControl(UFDataValueEntity value, EditValueControlTypeEnum controlType)
        {
            if (controlType == EditValueControlTypeEnum.CheckBox)
                return new ItemControlBoolDataValue();
            else if (controlType == EditValueControlTypeEnum.ComboBox)
                return new ItemControlComboDataValue();
            else if (controlType == EditValueControlTypeEnum.EditDisplay)
            {
                Type type = value.DataType.ToNetType();
                if (type.IsValueType)
                    return new ItemControlNumericDataValue();
                else
                    return new ItemControlStringDataValue();
            }
            else if (controlType == EditValueControlTypeEnum.RadioButton)
                return new ItemControlRadioButtonDataValue();
            else if (controlType == EditValueControlTypeEnum.ListView)
                return new ItemControlListViewDataValue();
            else
                return new ItemControlStringDataValue();
        }

        #endregion

        #region Dependency Properties

        public static void AddNewLayoutItemLabelControlIfNecessary(this LayoutControl layoutControl)
        {
            var fe = (from c in layoutControl.AvailableItems
                      where c.Tag == labelObjectTag
                      select c).FirstOrDefault();

            if (fe == null)
                layoutControl.AddLayoutItemLabelControls(String.Empty);
        }

        public static void AddLayoutItemLabelControls(this LayoutControl layoutControl, string[] names)
        {
            foreach (var name in names)
                layoutControl.AddLayoutItemLabelControls(name);
        }

        public static void AddLayoutItemLabelControls(this LayoutControl layoutControl, string name)
        {
            FrameworkElement fe = null;
            if (String.IsNullOrEmpty(name))
                name = Utilities.WPF.DependencyObjectExtensions.AdaptName(Guid.NewGuid().ToString());
            else
            {
                fe = (from c in layoutControl.AvailableItems
                      where c.Tag == labelObjectTag && c.Name == name
                      select c).FirstOrDefault();
            }

            if (fe == null)
            {
                fe = new RecipeLayoutItem()
                {
                    Name = name,
                    Label = Properties.Resources.LabelObjectDescription,
                    Tag = labelObjectTag
                };

                layoutControl.AvailableItems.Add(fe);
                layoutControl.RegisterNameRecursive(fe);
            }
        }

        public static List<LayoutItem> GetLayoutItemLabelControls(this LayoutControl layoutControl)
        {
            return (from c in layoutControl.GetVisualChildrenOfType<LayoutItem>()
                    where c.Tag == labelObjectTag && !layoutControl.AvailableItems.Contains(c)
                    select c).ToList();
        }

        public static void RegisterNameRecursive(this LayoutControl layoutControl, FrameworkElement uie)
        {
            Utilities.WPF.DependencyObjectExtensions.RegisterName(layoutControl, uie, false, false);
            if (uie is IPanel)
            {
                var panel = uie as IPanel;
                foreach (var child in panel.GetChildren(true, true, false))
                {
                    layoutControl.RegisterNameRecursive(child);
                }
            }
        }

        public static void UnregisterNameRecursive(this LayoutControl layoutControl, FrameworkElement uie)
        {
            Utilities.WPF.DependencyObjectExtensions.UnregisterName(layoutControl, uie, false);
            if (uie is IPanel)
            {
                var panel = uie as IPanel;
                foreach (var child in panel.GetChildren(true, true, false))
                {
                    layoutControl.UnregisterNameRecursive(child);
                }
            }
        }

        #endregion

        #region Layout Persistance
        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            {

            }

            return null;
        }

        static String GetLayoutFileName(String title)
        {
            return String.Format("{0}{1}", title, Properties.Settings.Default.DefaultLayoutExt);
        }

        public static bool SaveLayout(UFRecipeDocument document, LayoutControl control, bool usestorage = false)
        {
            bool bRet = false;

            try
            {
                var isoStorage = GetStorage();
                if (usestorage && isoStorage != null)
                {
                    var filename = GetLayoutFileName(document.Title);
                    using (var stream = new IsolatedStorageFileStream(filename, FileMode.Create, isoStorage))
                    {
                        var settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (var writer = XmlWriter.Create(stream, settings))
                        {
                            control.WriteToXML(writer);
                            bRet = true;
                        }
                    }
                }
                else if (!usestorage)
                {
                    using (var stream = new MemoryStream(document.LayoutItems))
                    {
                        var settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (var writer = XmlWriter.Create(stream, settings))
                        {
                            control.WriteToXML(writer);
                            bRet = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return bRet;
        }

        public static bool LoadLayout(UFRecipeDocument document, LayoutControl control, bool usestorage = true)
        {
            bool bRet = false;

            try
            {
                var isoStorage = GetStorage();
                if (usestorage && isoStorage != null)
                {
                    try
                    {
                        var filename = GetLayoutFileName(document.Title);
                        using (var stream = new IsolatedStorageFileStream(filename, FileMode.Open, isoStorage))
                        {
                            XmlReaderSettings settings = new XmlReaderSettings
                            {
                                ConformanceLevel = ConformanceLevel.Document,
                                CloseInput = true
                            };

                            using (var reader = XmlReader.Create(stream, settings))
                            {
                                control.Children.Clear();
                                control.ReadFromXML(reader);
                                bRet = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (!bRet)
                {
                    try
                    {
                        using (var stream = new MemoryStream(document.LayoutItems))
                        {
                            XmlReaderSettings settings = new XmlReaderSettings
                            {
                                ConformanceLevel = ConformanceLevel.Document,
                                CloseInput = true
                            };

                            using (var reader = XmlReader.Create(stream, settings))
                            {
                                control.Children.Clear();
                                control.ReadFromXML(reader);
                                bRet = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return bRet;
        }

        #endregion
    }
}
