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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {

        public EditorPropertiesBuilder NumericTextbox(string id)
        {
            var model = new EditorProperties();
            var editor = new Numeric(id, model);
            editor.EditorName = "ejNumericTextbox";
            return new EditorPropertiesBuilder(editor);
        }
        public Numeric NumericTextbox(String id, EditorProperties model)
        {
            var editor = new Numeric(id, model);
            return editor;
        }

        public EditorPropertiesBuilder PercentageTextbox(string id)
        {
            var model = new EditorProperties();
            var editor = new Percent(id, model);
            editor.EditorName = "ejPercentageTextbox";
            return new EditorPropertiesBuilder(editor);
        }
        public Percent PercentageTextbox(String id, EditorProperties model)
        {
            var editor = new Percent(id, model);
            return editor;
        }
        public EditorPropertiesBuilder CurrencyTextbox(string id)
        {
            var model = new EditorProperties();
            var editor = new Currency(id, model);
            editor.EditorName = "ejCurrencyTextbox";
            return new EditorPropertiesBuilder(editor);
        }
        public Currency CurrencyTextbox(String id, EditorProperties model)
        {
            var editor = new Currency(id, model);
            return editor;
        }
    }

}
