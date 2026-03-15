using Mindscape.WpfElements;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.WpfPropertyGrid;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace WpfControlsUnitTests
{
    [TestFixture]
    public class PropertyNodeFixture
    {
        [Test, Apartment(ApartmentState.STA)]
        public void IsLocalizableProperty_TestStructProperties ()
        {
            //Arrange
            PropertyGrid grid = new PropertyGrid();
            Control ctrl = new Control();
            grid.SelectedObject = ctrl;

            //Act
            var values = new TestClass[]
            {
                CheckProperty(grid, nameof(Thickness)),
                CheckProperty(grid, nameof(Size)),
                CheckProperty(grid, nameof(Point)),
                CheckProperty(grid, nameof(Rect))
            };

            //Assert
            for (int i = 0; i < values.Length; i++)
            {
                Assert.IsTrue(values[i].LocalizablePropertyNode);
                Assert.IsFalse(values[i].InnerLocalizableProperty);
                Assert.IsNotNull(values[i].InnerPropertyNode);
            }
        }

        [Test, Apartment(ApartmentState.STA)]
        public void IsLocalizableProperty_TestNestedProperties()
        {
            //Arrange
            PropertyGrid grid = new PropertyGrid();
            Control ctrl = new Control();
            grid.SelectedObject = ctrl;

            //Act
            var values = new TestClass[]
            {
                CheckProperty(grid, nameof(FontFamily)),
                CheckProperty(grid, nameof(XmlLanguage)),
                CheckProperty(grid, nameof(Transform)),
                CheckProperty(grid, nameof(Dispatcher)),
                CheckProperty(grid, nameof(DependencyObjectType))
            };

            //Assert
            for (int i = 0; i < values.Length; i++)
            {
                Assert.IsTrue(values[i].LocalizablePropertyNode);
                Assert.IsTrue(values[i].InnerLocalizableProperty);
                Assert.IsNotNull(values[i].InnerPropertyNode);
            }
        }

        [Test, Apartment(ApartmentState.STA)]
        public void IsLocalizableProperty_TestSimpleProperties()
        {
            //Arrange
            PropertyGrid grid = new PropertyGrid();
            Control ctrl = new Control();
            grid.SelectedObject = ctrl;

            //Act
            var values = new TestClass[]
            {
                CheckProperty(grid, nameof(Brush)),
                CheckProperty(grid, nameof(FontStretch)),
                CheckProperty(grid, nameof(FontStyle)),
                CheckProperty(grid, nameof(FontWeight)),
                CheckProperty(grid, nameof(HorizontalAlignment)),
                CheckProperty(grid, nameof(VerticalAlignment)),
                CheckProperty(grid, nameof(Int32)),
                CheckProperty(grid, nameof(Boolean)),
                CheckProperty(grid, nameof(ControlTemplate)),
                CheckProperty(grid, nameof(Style)),
                CheckProperty(grid, nameof(TriggerCollection)),
                CheckProperty(grid, nameof(DependencyObject)),
                CheckProperty(grid, nameof(ResourceDictionary)),
                CheckProperty(grid, nameof(Object)),
                CheckProperty(grid, nameof(BindingGroup)),
                CheckProperty(grid, nameof(String)),
                CheckProperty(grid, nameof(InputScope)),
                CheckProperty(grid, nameof(Double)),
                CheckProperty(grid, nameof(FlowDirection)),
                CheckProperty(grid, nameof(Cursor)),
                CheckProperty(grid, nameof(ContextMenu)),
                CheckProperty(grid, nameof(InputBindingCollection)),
                CheckProperty(grid, nameof(CommandBindingCollection)),
                CheckProperty(grid, nameof(BitmapEffect)),
                CheckProperty(grid, nameof(Effect)),
                CheckProperty(grid, nameof(BitmapEffectInput)),
                CheckProperty(grid, nameof(CacheMode)),
                CheckProperty(grid, nameof(Visibility)),
                CheckProperty(grid, nameof(Geometry)),
                CheckProperty(grid, "IEnumerable`1"),
                CheckProperty(grid, nameof(InputMethodState)),
                CheckProperty(grid, nameof(ImeConversionModeValues)),
                CheckProperty(grid, nameof(ImeSentenceModeValues)),
                CheckProperty(grid, nameof(UIElement)),
                CheckProperty(grid, nameof(PlacementMode)),
                CheckProperty(grid, nameof(EdgeMode)),
                CheckProperty(grid, nameof(BitmapScalingMode)),
                CheckProperty(grid, nameof(ClearTypeHint)),
                CheckProperty(grid, nameof(NumberCultureSource)),
                CheckProperty(grid, nameof(CultureInfo)),
                CheckProperty(grid, nameof(NumberSubstitutionMethod)),
                CheckProperty(grid, nameof(FontFraction)),
                CheckProperty(grid, nameof(FontVariants)),
                CheckProperty(grid, nameof(FontCapitals)),
                CheckProperty(grid, nameof(FontNumeralStyle)),
                CheckProperty(grid, nameof(FontNumeralAlignment)),
                CheckProperty(grid, nameof(FontEastAsianWidths)),
                CheckProperty(grid, nameof(FontEastAsianLanguage)),
                CheckProperty(grid, nameof(KeyboardNavigationMode))
            };

            //Assert
            for (int i = 0; i < values.Length; i++)
            {
                Assert.IsTrue(values[i].LocalizablePropertyNode);
                Assert.IsNull(values[i].InnerPropertyNode);
            }
        }

        #region Private Methods
        private TestClass CheckProperty(PropertyGrid grid, string propName)
        {
            Node node = grid.Nodes.FirstOrDefault(s => s.PropertyType.Name == propName);
            var innerProperty = (node as PropertyNode).Children.FirstOrDefault();

            var testClass = new TestClass()
            {
                LocalizablePropertyNode = (node as PropertyNode).IsLocalizableProperty
            };

            if (innerProperty is PropertyNode propertyNode)
            {
                testClass.InnerLocalizableProperty = propertyNode.IsLocalizableProperty;
                testClass.InnerPropertyNode = propertyNode;
            }

            return testClass;
        }
        #endregion
    }

    internal class TestClass
    {
        public bool LocalizablePropertyNode { get; set; }

        public bool InnerLocalizableProperty { get; set; }

        public Node InnerPropertyNode { get; set; }
    }
}
