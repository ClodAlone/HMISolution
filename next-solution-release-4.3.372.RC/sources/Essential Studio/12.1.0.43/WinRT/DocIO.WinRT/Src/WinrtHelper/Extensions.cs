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
using System.Text;
using System.Threading.Tasks;
using Syncfusion.DocIO.DLS;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Syncfusion.DocIO.WinrtHelper
{
    public static class Extension
    {
        /// <summary>
        /// Returns a DateTime equivalent to the specified OLE Automation Date.
        /// </summary>
        /// <param name="doubleOLEValue">An OLE Automation Date value.</param>
        /// <returns>An object that represents the same date and time as d.</returns>
        public static DateTime FromOADate(double doubleOLEValue)
        {
            if (doubleOLEValue < -657435.0 || doubleOLEValue > 2958465.99999999)
                throw new ArgumentException("Not an valid OLE value.");

            double doubleSecondsPerDay = 24 * 60 * 60;
            string[] strSplitedValue = doubleOLEValue.ToString().Split('.');
            int integralPart = Convert.ToInt32(strSplitedValue[0]);
            double mantisaPart = Convert.ToDouble("." + strSplitedValue[1]);
            int totalSeconds = (int)(mantisaPart * doubleSecondsPerDay);
            DateTime MinOLEDate = DateTime.Parse("1899-12-30 12:0:0 AM");
            DateTime oleDateFromValue = MinOLEDate.AddDays(integralPart);
            oleDateFromValue = oleDateFromValue.AddSeconds(totalSeconds);
            return oleDateFromValue;
        }

        /// <summary>
        /// Converts the value of this instance to the equivalent OLE Automation date.
        /// </summary>
        /// <param name="inDateTime"></param>
        /// <returns>A double-precision floating-point number that contains an OLE Automation date equivalent to the value of this instance.</returns>
        public static double ToOADate(this DateTime inDateTime)
        {
            DateTime MinOLEDate = DateTime.Parse("1899-12-30 12:0:0 AM");
            DateTime MaxOLEDate = DateTime.Parse("9999-12-31 12:0:0 AM");

            if (inDateTime < MinOLEDate || inDateTime > MaxOLEDate)
                throw new ArgumentException("Not an Valid OLE Date.");

            double doubleSecondsPerDay = 24 * 60 * 60;


            TimeSpan dateDiff = inDateTime - MinOLEDate;
            double doubleOLEValue = (double)dateDiff.Days;
            doubleOLEValue += (double)dateDiff.TotalSeconds / doubleSecondsPerDay;

            return doubleOLEValue;

        }
        /// <summary>
        /// Determines whether the specified object is an instance of the current System.Type.
        /// </summary>
        /// <param name="ent">The entity to compare with the current Type.</param>
        /// <returns>true if the current Type is in the inheritance hierarchy of the entity represented by ent, or if the current Type is an interface that e supports otherwise false.</returns>
        public static bool IsInstanceOfType(this Type type, Entity ent)
        {
            if (type == ent.GetType())
                return true;
            else
            {
                switch(ent.EntityType)
                {
                    case EntityType.WordDocument:
                    case EntityType.Section:
                        if (type.Name == "WidgetContainer" || type.Name == "WidgetBase"
                            || type.Name == "Entity")
                            return true;
                        break;
                    // The text body
                    case EntityType.TextBody:
                    case EntityType.HeaderFooter:
                    case EntityType.TableCell:
                        if (type.Name == "WTextBody" || type.Name == "WidgetContainer"
                            || type.Name == "WidgetBase" || type.Name == "Entity")
                            return true;
                        break;
                    // The text body items
                    case EntityType.Paragraph:
                    case EntityType.AlternateChunk:
                    case EntityType.StructureDocumentTag:
                    //case EntityType.StructureDocumentTagRow:
                    //case EntityType.StructureDocumentTagCell:
                    case EntityType.Table:
                        if (type.Name == "TextBodyItem" || type.Name == "WidgetBase"
                            || type.Name == "Entity")
                            return true;
                        break;
                    case EntityType.SDTBlockContent:
                    case EntityType.SDTInlineContent:
                    //case EntityType.SDTRowContent:
                    //case EntityType.SDTCellContent:
                    case EntityType.TableRow:
                        if (type.Name == "WidgetBase" || type.Name == "Entity")
                            return true;
                        break;
                    // The paragraph items
                    case EntityType.BookmarkStart:
                    case EntityType.BookmarkEnd:
                    case EntityType.Break:
                    case EntityType.OleObject:
                    case EntityType.Shape:
                    case EntityType.AutoShape:
                    case EntityType.TOC:
                    case EntityType.Comment:
                    case EntityType.FieldMark:
                    case EntityType.Footnote:
                    case EntityType.StructureDocumentTagInline:
                    case EntityType.TextRange:
                    case EntityType.Picture:
                    case EntityType.Symbol:
                    case EntityType.TextBox:
                    case EntityType.XmlParaItem:
                    case EntityType.CommentMark:
                    case EntityType.AbsoluteTab:
                    //EntityType - Undefined handled specifically for Watermark.
                    case EntityType.Undefined:
                        if (type.Name == "ParagraphItem" || type.Name == "WidgetBase"
                            || type.Name == "Entity")
                            return true;
                        break;
                    case EntityType.Field:
                    case EntityType.EmbededField:
                    case EntityType.MergeField:
                    case EntityType.SeqField:
                    case EntityType.ControlField:
                        if (type.Name == "WField" || type.Name == "WTextRange"
                            || type.Name == "ParagraphItem" || type.Name == "WidgetBase"
                            || type.Name == "Entity")
                            return true;
                        break;
                    case EntityType.TextFormField:
                    case EntityType.DropDownFormField:
                    case EntityType.CheckBox:
                        if (type.Name == "WFormField" || type.Name == "WField"
                            || type.Name == "WTextRange" || type.Name == "ParagraphItem"
                            || type.Name == "WidgetBase" || type.Name == "Entity")
                            return true;
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
    internal class UIDispatcher
    {
        /// <summary>
        /// Executes the action using UIElement.
        /// </summary>
        /// <param name="action">An Action.</param>
        internal static void Execute(Action action)
        {
            ExecuteInternal(action).Wait();
        }
        
        /// <summary>
        /// Executes the action using UIElement.
        /// </summary>
        /// <param name="action">An Action.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        private static async Task ExecuteInternal(Action action)
        {
            if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
                action();
            else
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}