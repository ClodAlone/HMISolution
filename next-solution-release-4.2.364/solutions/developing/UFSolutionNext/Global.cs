using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;
using UFInterfaces;

namespace UFSolutionNext
{
    public class Global
    {
        public static PluginServices Plugins = new PluginServices();

        /*
            instead of on the frmMain.cs having to declare a PluginService object
            what i've done here is created one in the Global Class.. i've also made
            it static, so we don't have to worry about the object.. It's always gonna
            be there for us and the same object will always be accessed by everything
            else in the program...
			
            So now, everywhere else in this project i can type:
			
                Global.Plugins .... > 
				
            and it will bring up the Plugins object created above.. peachy, eh?
		
        */

#if CONNEXT
        internal static BitmapImage AppIcon = Global.GetImage("Connext.ico");
        static BitmapImage GetImage(String iconName)
        {
            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            Assembly assembly = Assembly.GetExecutingAssembly();

            String str = String.Format("pack://application:,,,/{0};component/{1}",
                System.IO.Path.GetFileNameWithoutExtension(assembly.Location), iconName);
            bm.UriSource = new Uri(str);
            bm.EndInit();
            return bm;
        }
#endif
    }
}
