using System.Reflection;
using System.Collections.Generic;
using DevExpress.UI.Xaml.Layout;

namespace UFWinRTClient
{

    //The NavigationTypeProvider class returns an instance of a type by the type's name.
    //It is used when navigating pages within View Models and when a target page is set in XAML.
    public class NavigationTypeProvider : TypeProviderBase
    {
        public override IEnumerable<System.Reflection.Assembly> Assemblies
        {
            get
            {
                //Here you can enumerate assemblies that contain classes of pages used for navigating.
                yield return typeof(App).GetTypeInfo().Assembly;
            }
        }
    }
}