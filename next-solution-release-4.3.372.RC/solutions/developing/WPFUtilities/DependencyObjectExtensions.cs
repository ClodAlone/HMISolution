using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using log4net;
#else
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Utilities.WPF
{
    public static class DependencyObjectExtensions
    {
        static List<Type> notUsingCacheTypes = new List<Type> { typeof(System.Windows.Controls.TreeView) };
#if !WINDOWS_UWP
        public static String RegisterModel3D(FrameworkElement parent, FrameworkElement element, Model3D model, bool bGenerate = true)
        {
            var namescope = NameScope.GetNameScope(element) as IDictionary<string, object>;
            if (namescope != null && namescope.Keys != null && namescope.Keys.Count > 0)
            {
                var nameList = (from c in namescope.Keys where namescope[c] == model select c).ToList();
                if (nameList.Count > 0)
                    return nameList[0];
            }
            if (element is ContentControl)
            {
                var contentControl = element as ContentControl;
                if (contentControl.Content is FrameworkElement)
                {
                    namescope = NameScope.GetNameScope(contentControl.Content as FrameworkElement) as IDictionary<string, object>;
                    if (namescope != null && namescope.Keys != null && namescope.Keys.Count > 0)
                    {
                        var nameList = (from c in namescope.Keys where namescope[c] == model select c).ToList();
                        if (nameList.Count > 0)
                            return nameList[0];
                    }
                }
            }
            namescope = NameScope.GetNameScope(parent) as IDictionary<string, object>;
            if (namescope != null && namescope.Keys != null && namescope.Keys.Count > 0)
            {
                var nameList = (from c in namescope.Keys where namescope[c] == model select c).ToList();
                if (nameList.Count > 0)
                    return nameList[0];
            }
            if (!bGenerate)
                return null;
            var Name = Generate3DModelName(model);
            try
            {
                parent.UnregisterName(Name);
            }
            catch (Exception ex)
            {

            }
            try
            {
                element.UnregisterName(Name);
            }
            catch (Exception ex)
            {
                
            }
            parent.RegisterName(Name, model);
            element.RegisterName(Name, model);
            return Name;
        }

        public static String Generate3DModelName(Model3D model)
        {
            if (model is GeometryModel3D)
            {
                var model3D = model as GeometryModel3D;
                var geometrymesh3d = model3D.Geometry as MeshGeometry3D;
                var sb = new StringBuilder();
                int nCount = 5;
                foreach (var pt in geometrymesh3d.Positions)
                {
                    sb.Append(pt.ToString());
                    if (--nCount <= 0)
                        break;
                }
                return AdaptName(sb.ToString());
            }
            else if (model is Model3DGroup)
            {
                var model3D = model as Model3DGroup;
                var list = (from c in model3D.Children.OfType<GeometryModel3D>() select c).ToList();
                if (list.Count > 0)
                    return String.Format("Group_{0}", Generate3DModelName(list[0]));
                var listgroup = (from c in model3D.Children.OfType<Model3DGroup>() select c).ToList();
                if (listgroup.Count > 0)
                    return String.Format("Group_{0}", Generate3DModelName(listgroup[0]));
            }

            return AdaptName(model.ToString());
        }

        // readonly static Dictionary<String, Model3D> mapModel3Ds = new Dictionary<String, Model3D>();

        static Model3D GetMatchingModel3DInGroup(Model3DGroup group, String name)
        {
            foreach (var model in group.Children)
            {
                var found = Generate3DModelName(model);
                //if (!mapModel3Ds.ContainsKey(found))
                //    mapModel3Ds.Add(found, model);
                if (found == name)
                    return model;

                if (model is Model3DGroup)
                {
                    var ret = GetMatchingModel3DInGroup(model as Model3DGroup, name);
                    if (ret != null)
                        return ret;
                }
            }

            return null;
        }

        public static Model3D GetMatchingModel3D(UIElement element, String name)
        {
            var namescope = NameScope.GetNameScope(element) as IDictionary<string, object>;
            if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                namescope[name] is Model3D)
                return namescope[name] as Model3D;
            if (element is ContentControl)
            {
                var contentControl = element as ContentControl;
                if (contentControl.Content is DependencyObject)
                {
                    namescope = NameScope.GetNameScope(contentControl.Content as DependencyObject) as IDictionary<string, object>;
                    if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                        namescope[name] is Model3D)
                        return namescope[name] as Model3D;
                }
            }

            //if (mapModel3Ds.ContainsKey(name))
            //    return mapModel3Ds[name];

            var listViewport3Ds = element.GetChildrenOfType<Viewport3D>().ToList();
            if (listViewport3Ds.Count == 0 && element is Viewport3D)
                listViewport3Ds.Add(element as Viewport3D);
            foreach (var viewport3D in listViewport3Ds)
            {
                namescope = NameScope.GetNameScope(viewport3D) as IDictionary<string, object>;
                if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                    namescope[name] is Model3D)
                    return namescope[name] as Model3D;

                foreach (var child in viewport3D.Children)
                {
                    namescope = NameScope.GetNameScope(child) as IDictionary<string, object>;
                    if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                        namescope[name] is Model3D)
                        return namescope[name] as Model3D;

                    var modelVisual = child as ModelVisual3D;
                    if (modelVisual != null && modelVisual.Content != null)
                    {
                        var model = modelVisual.Content;

                        var found = Generate3DModelName(model);
                        //if (!mapModel3Ds.ContainsKey(found))
                        //    mapModel3Ds.Add(found, model);
                        if (found == name)
                            return model;

                        var modelGroup = model as Model3DGroup;
                        if (modelGroup != null)
                        {
                            var ret = GetMatchingModel3DInGroup(modelGroup, name);
                            if (ret != null)
                                return ret;
                        }
                    }
                }
            }

            return null;
        }

        public static ModelVisual3D GetMatchingModelVisual3D(UIElement element, String name)
        {
            var namescope = NameScope.GetNameScope(element) as IDictionary<string, object>;
            if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                namescope[name] is ModelVisual3D)
                return namescope[name] as ModelVisual3D;
            if (element is ContentControl)
            {
                var contentControl = element as ContentControl;
                if (contentControl.Content is DependencyObject)
                {
                    namescope = NameScope.GetNameScope(contentControl.Content as DependencyObject) as IDictionary<string, object>;
                    if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                        namescope[name] is ModelVisual3D)
                        return namescope[name] as ModelVisual3D;
                }
            }
            //if (mapModel3Ds.ContainsKey(name))
            //    return mapModel3Ds[name];

            var listViewport3Ds = element.GetChildrenOfType<Viewport3D>().ToList();
            if (listViewport3Ds.Count == 0 && element is Viewport3D)
                listViewport3Ds.Add(element as Viewport3D);
            foreach (var viewport3D in listViewport3Ds)
            {
                namescope = NameScope.GetNameScope(viewport3D) as IDictionary<string, object>;
                if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                    namescope[name] is ModelVisual3D)
                    return namescope[name] as ModelVisual3D;

                foreach (var child in viewport3D.Children)
                {
                    namescope = NameScope.GetNameScope(child) as IDictionary<string, object>;
                    if (namescope != null && namescope.Keys != null && namescope.Keys.Contains(name) &&
                        namescope[name] is ModelVisual3D)
                        return namescope[name] as ModelVisual3D;

                    var modelVisual = child as ModelVisual3D;
                    if (modelVisual != null && modelVisual.Content != null)
                    {
                        var model = modelVisual.Content;

                        var found = Generate3DModelName(model);
                        //if (!mapModel3Ds.ContainsKey(found))
                        //    mapModel3Ds.Add(found, model);
                        if (found == name)
                            return modelVisual;

                        var modelGroup = model as Model3DGroup;
                        if (modelGroup != null)
                        {
                            var ret = GetMatchingModel3DInGroup(modelGroup, name);
                            if (ret != null)
                                return modelVisual;
                        }
                    }
                }
            }

            return null;
        }

        private static Model3DGroup FindParentGroup(Model3DGroup group, Model3D element)
        {
            foreach(var model3d in group.Children)
            {
                if (model3d is Model3DGroup)
                {
                    var ret = FindParentGroup(model3d as Model3DGroup, element);
                    if (ret != null)
                        return ret;
                }
                else if (model3d == element)
                    return group;
            }

            return null;
        }

        public static Model3DGroup FindParentGroup(ModelVisual3D visual3D, Model3D element)
        {
            var group = visual3D.Content as Model3DGroup;
            if (group == null)
                return null;
            return FindParentGroup(group, element);
        }

        public static List<GeometryModel3D> GetAllGeometries(Model3DGroup group)
        {
            var ret = new List<GeometryModel3D>();
            foreach (var model3d in group.Children)
            {
                if (model3d is Model3DGroup)
                    ret.AddRange(GetAllGeometries(model3d as Model3DGroup));
                else if (model3d is GeometryModel3D)
                    ret.Add(model3d as GeometryModel3D);
            }
            return ret;
        }

        public static MeshGeometry3D GetMeshGeometry(Model3DGroup group)
        {
            var meshGeometry = new MeshGeometry3D();
            foreach (var childModel in group.Children)
            {
                if (childModel is GeometryModel3D)
                {
                    var childmeshGeometry = (childModel as GeometryModel3D).Geometry as MeshGeometry3D;
                    foreach (var normal in childmeshGeometry.Normals)
                        meshGeometry.Normals.Add(normal);
                    foreach (var position in childmeshGeometry.Positions)
                        meshGeometry.Positions.Add(position);
                    foreach (var texture in childmeshGeometry.TextureCoordinates)
                        meshGeometry.TextureCoordinates.Add(texture);
                    foreach (var triangle in childmeshGeometry.TriangleIndices)
                        meshGeometry.TriangleIndices.Add(triangle);
                }
                else if (childModel is Model3DGroup)
                {
                    var merge = GetMeshGeometry(childModel as Model3DGroup);
                    foreach (var normal in merge.Normals)
                        meshGeometry.Normals.Add(normal);
                    foreach (var position in merge.Positions)
                        meshGeometry.Positions.Add(position);
                    foreach (var texture in merge.TextureCoordinates)
                        meshGeometry.TextureCoordinates.Add(texture);
                    foreach (var triangle in merge.TriangleIndices)
                        meshGeometry.TriangleIndices.Add(triangle);
                }
            }

            return meshGeometry;
        }

        public static List<GeometryModel3D> GetAllGeometries(ModelVisual3D visual3D)
        {
            var group = visual3D.Content as Model3DGroup;
            if (group == null)
                return null;
            return GetAllGeometries(group);
        }

        public static String AdaptName(String name)
        {
            var newheader = name.Replace(' ', '_');
            newheader = newheader.Replace('\\', '_');
            newheader = newheader.Replace('/', '_');
            newheader = newheader.Replace('@', '_');
            newheader = newheader.Replace('$', '_');
            newheader = newheader.Replace('(', '_');
            newheader = newheader.Replace(')', '_');
            newheader = newheader.Replace(';', '_');
            newheader = newheader.Replace('=', '_');
            newheader = newheader.Replace('.', '_');
            newheader = newheader.Replace('?', '_');
            newheader = newheader.Replace(',', '_');
            newheader = newheader.Replace('-', '_');
            newheader = newheader.Replace('&', '_');
            newheader = newheader.Replace(':', '_');
            if (newheader.Length > 0 && Char.IsNumber(newheader[0]))
                newheader = String.Format("_{0}", newheader);
            return newheader;
        }

        static public void UnregisterName(FrameworkElement source, String name)
        {
            try
            {
                source.UnregisterName(name);
            }
            catch (Exception)
            {
            }
        }

        static public void UnregisterName(FrameworkElement source, FrameworkElement element, bool bIsNested = true)
        {
            try
            {
                if (!String.IsNullOrEmpty(element.Name))
                    source.UnregisterName(element.Name);

                if (bIsNested)
                {
                    var list = (from p in element.GetChildrenOfType<FrameworkElement>()
                                where !String.IsNullOrEmpty(p.Name)
                                select p).ToList();
                    list.ForEach(e => 
                        {
                            if (source.FindName(e.Name) != null)
                                source.UnregisterName(e.Name);
                        });
                }
                

                /*
                var namescope = NameScope.GetNameScope(element) as IDictionary<string, object>;
                if (namescope != null && namescope.Keys != null && namescope.Keys.Count > 0)
                {
                    foreach (var nameinscope in namescope.Keys)
                    {
                        try
                        {
                            source.UnregisterName(nameinscope);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }

                // if (bIsNested && !(element is UserControl))
                {
                    // NameScope.SetNameScope(element, new NameScope());

                    var list = (from p in element.GetChildrenOfType<FrameworkElement>()
                                where !String.IsNullOrEmpty(p.Name)
                                select p).ToList();
                    list.ForEach(e =>
                    {
                        if (!String.IsNullOrEmpty(e.Name))
                            source.UnregisterName(e.Name);
                    });
                }
                */

                //if (element is Panel)
                //{
                //    foreach (FrameworkElement item in (element as Panel).Children)
                //        UnregisterName(source, item);
                //}
                //else if (element is Viewbox)
                //    UnregisterName(source, (element as Viewbox).Child as FrameworkElement);
                //else if (element is ContentControl)
                //    UnregisterName(source, (element as ContentControl).Content as FrameworkElement);
            }
            catch (Exception)
            {
            }
        }

        const String NewNameRegExpr = @"\d+?$";
        internal static String NewName(String name, int counter, String format = "{0}{1}")
        {
            string fmtzero = "0";
            var baseName = Regex.Replace(name, NewNameRegExpr, "");
            var match = Regex.Match(name, NewNameRegExpr);
            if (match.Success && !String.IsNullOrEmpty(match.Value))
            {
                fmtzero = new String('0', match.Value.Length);
                try
                {
                    if (counter == 1)
                        counter = Convert.ToInt32(match.Value);
                }
                catch { }
            }
            //else
            //    counter = 1;

            name = String.Format(format, baseName, (counter++).ToString(fmtzero));

            return name;
        }

        static public IDictionary<String, String> RegisterName(FrameworkElement source, FrameworkElement element, bool bForceName = false, bool bIsNested = false)
        {
            var map = new Dictionary<String, String>();
            if (element == null)
                return map;

            var uid = element.Uid as String;
            if (!String.IsNullOrEmpty(uid))
                return map;

            element.Name = AdaptName(element.Name);
            if (String.IsNullOrEmpty(element.Name) && bForceName)
                element.Name = element.DependencyObjectType.Name;
            if (String.IsNullOrEmpty(element.Name) && !bIsNested)
                return map;

            if (ReferenceEquals(source.FindName(element.Name), element))
                return map;

            String name = element.Name;
            int i = 1;
            if (!String.IsNullOrEmpty(name))
            {
                var counter = 1;
                while (true)
                {
                    try
                    {
                        source.RegisterName(element.Name, element);
                        if (name != element.Name)
                            map.Add(name, element.Name);
                        //if (element is Panel)
                        //{
                        //    foreach (FrameworkElement item in (element as Panel).Children)
                        //        RegisterName(source, item);
                        //}
                        //else if (element is Viewbox)
                        //    RegisterName(source, (element as Viewbox).Child as FrameworkElement);
                        //else if (element is ContentControl)
                        //    RegisterName(source, (element as ContentControl).Content as FrameworkElement);
                        break;
                    }
                    catch (Exception)
                    {
                        //if (++counter > 10)
                        //{
                        //    var rnd = new Random();
                        //    i = rnd.Next(0, int.MaxValue - 2); // creates a number between 1 and 12
                        //}
                    }

                    // element.Name = String.Format("{0}{1}", name, i++);
                    try
                    {
                        element.Name = NewName(name, i++);
                    }
                    catch
                    {

                    }
                }
            }

            if (bIsNested && !(element is UserControl))
            {
                // NameScope.SetNameScope(element, new NameScope());
               
                var list = (from p in element.GetChildrenOfType<FrameworkElement>()
                            where !String.IsNullOrEmpty(p.Name) && 
                                  String.IsNullOrEmpty(p.Uid as String)
                            select p).ToList();
                list.ForEach(e =>
                {
                    if (String.IsNullOrEmpty(e.Name))
                        e.Name = e.DependencyObjectType.Name;

                    if (!ReferenceEquals(source.FindName(e.Name), e))
                    {
                        name = e.Name;
                        i = 1;
                        if (!String.IsNullOrEmpty(name))
                        {
                            var counter = 1;
                            while (true)
                            {
                                try
                                {
                                    source.RegisterName(e.Name, e);
                                    if (name != e.Name)
                                    {
                                        if (map.ContainsKey(name))
                                            map.Remove(name);
                                        map.Add(name, e.Name);
                                    }
                                    break;
                                }
                                catch (Exception)
                                {
                                    //if (++counter > 10)
                                    //{
                                    //    var rnd = new Random();
                                    //    i = rnd.Next(0, int.MaxValue - 2); // creates a number between 1 and 12
                                    //}
                                }
                                // e.Name = String.Format("{0}{1}", name, i++);
                                try
                                {
                                    e.Name = NewName(name, i++);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                });           
            }

            if (bIsNested && !(element is UserControl))
            {
                var namescope = NameScope.GetNameScope(element) as IDictionary<string, object>;
                if (namescope != null && namescope.Keys != null && namescope.Keys.Count > 0)
                {
                    foreach (var nameinscope in namescope.Keys)
                    {
                        if (map.ContainsKey(nameinscope))
                            continue;
                        name = nameinscope;
                        var sourceinscope = namescope[nameinscope];
                        while (true)
                        {
                            var counter = 1;
                            try
                            {
                                if (ReferenceEquals(source.FindName(name), sourceinscope))
                                    break;

                                source.RegisterName(name, sourceinscope);
                                if (name != nameinscope)
                                    map.Add(nameinscope, name);
                                //if (element is Panel)
                                //{
                                //    foreach (FrameworkElement item in (element as Panel).Children)
                                //        RegisterName(source, item);
                                //}
                                //else if (element is Viewbox)
                                //    RegisterName(source, (element as Viewbox).Child as FrameworkElement);
                                //else if (element is ContentControl)
                                //    RegisterName(source, (element as ContentControl).Content as FrameworkElement);
                                break;
                            }
                            catch (Exception)
                            {
                                //if (++counter > 10)
                                //{
                                //    var rnd = new Random();
                                //    i = rnd.Next(0, int.MaxValue - 2); // creates a number between 1 and 12
                                //}
                            }

                            // name = String.Format("{0}{1}", nameinscope, i++);
                            try
                            {
                                name = NewName(nameinscope, i++);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            if (map.Count > 0)
            {
                var mapfound = element.GetAllResourceTypes(typeof(Storyboard), false);
                foreach (var namechanged in map.Keys)
                {
                    var storyboards = (from c in mapfound.Values.OfType<Storyboard>()
                                       let sb = c
                                       from child in sb.Children
                                       where Storyboard.GetTargetName(child) == namechanged
                                       select child).ToList();

                    foreach (var child in storyboards)
                        Storyboard.SetTargetName(child, map[namechanged]);
                }
            }

            return map;
        }

        public static Window GetWindow(this DependencyObject dependencyObject)
        {
            return Window.GetWindow(dependencyObject);
            //var parent = LogicalTreeHelper.GetParent(dependencyObject);

            //while (parent != null && !(parent is Window))
            //{
            //    parent = LogicalTreeHelper.GetParent(parent);
            //}

            //return parent as Window;
        }

        public static Rect BoundsRelativeTo(this FrameworkElement element,
                                                 UIElement relativeTo)
        {
            var r = element.TransformToVisual(relativeTo)
                     .TransformBounds(System.Windows.Controls.Primitives.LayoutInformation.GetLayoutSlot(element));
            var topLeft = element.TranslatePoint(new Point(0, 0), relativeTo);
            return new Rect(topLeft, new Size(r.Width, r.Height));
        }

        public static Rect CalculateBoundRect(List<UIElement> elList, UIElement parent)
        {
            Rect boundce = Rect.Empty;
            elList.ForEach(el =>
            {
                Rect r;
                if (el is Shape && !(el is Path))
                {
                    Geometry g;
                    var shape = el as Shape;
                    g = shape.RenderedGeometry;
                    try
                    {
                        g.Transform = (Transform)el.TransformToVisual(parent);
                    }
                    catch (Exception ex)
                    {

                    }
                    r = g.GetRenderBounds(new Pen(shape.Stroke, shape.StrokeThickness));
                    if (r.IsEmpty && el is FrameworkElement)
                    {
                        var fe = el as FrameworkElement;
                        r = new Rect(Canvas.GetLeft(fe), Canvas.GetTop(fe), fe.ActualWidth, fe.ActualHeight);
                    }
                    // r.Offset(shape.StrokeThickness / 8, shape.StrokeThickness / 8);
                }
                else
                {
                    try
                    {
                        r = el.TransformToVisual(parent).TransformBounds(VisualTreeHelper.GetDescendantBounds(el));
                    }
                    catch (Exception ex)
                    {
                        r = Rect.Empty;
                    }
                    if (r.IsEmpty && el is FrameworkElement)
                    {
                        var fe = el as FrameworkElement;
                        r = new Rect(Canvas.GetLeft(fe), Canvas.GetTop(fe), fe.ActualWidth, fe.ActualHeight);
                    }
                }

                // var r = BoundsRelativeTo(el as FrameworkElement, parent);
                //if (el is Shape)
                //{
                //    var shape = el as Shape;
                //    r.Offset(-(shape.StrokeThickness / 8), -(shape.StrokeThickness / 8));
                //}

                if (boundce.IsEmpty)
                    boundce = r;
                else
                    boundce.Union(r);
            });

            return boundce;
        }
#endif

        public static void CleanChildrenOfTypeCache()
        {
            lock (lockCache)
            {
                mapCacheChildren.Clear();
                mapCacheVisualChildren.Clear();
                mapCacheChildrenFirst.Clear();
            }
        }

        public static void CleanChildrenOfTypeCache(DependencyObject dependencyObject)
        {
            lock (lockCache)
            {
                mapCacheChildren.Remove(dependencyObject);
                mapCacheVisualChildren.Remove(dependencyObject);
                mapCacheChildrenFirst.Remove(dependencyObject);
            }
        }

        static Object lockCache = new object();
        static Dictionary<DependencyObject, Dictionary<Type, System.Collections.IList>> mapCacheChildren = 
            new Dictionary<DependencyObject, Dictionary<Type, System.Collections.IList>>();

#if !WINDOWS_UWP
        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject dependencyObject) where T : class
        {
            var accumulator = new List<T>();
            var type = accumulator.GetType();
            lock (lockCache)
            {
                if (mapCacheChildren.ContainsKey(dependencyObject) &&
                    mapCacheChildren[dependencyObject].ContainsKey(type))
                    return mapCacheChildren[dependencyObject][type] as List<T>;
            

                AccumulateChildrenOfType(dependencyObject, accumulator, true);

                if (!mapCacheChildren.ContainsKey(dependencyObject))
                    mapCacheChildren.Add(dependencyObject, new Dictionary<Type, System.Collections.IList>());
                if (mapCacheChildren[dependencyObject].ContainsKey(type))
                    mapCacheChildren[dependencyObject].Remove(type);
                mapCacheChildren[dependencyObject].Add(type, accumulator);
            }

            return accumulator;
        }
#else
        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject dependencyObject) where T : class
        {
            var accumulator = new List<T>();
            AccumulateVisualChildrenOfType(dependencyObject, accumulator, true);
            return accumulator;
        }
#endif

        static Dictionary<DependencyObject, Dictionary<Type, System.Collections.IList>> mapCacheVisualChildren =
            new Dictionary<DependencyObject, Dictionary<Type, System.Collections.IList>>();

        public static IEnumerable<T> GetVisualChildrenOfType<T>(this DependencyObject dependencyObject) where T : class
        {
            var accumulator = new List<T>();
            var type = accumulator.GetType();
            if (notUsingCacheTypes.Contains(dependencyObject.GetType()))
                AccumulateVisualChildrenOfType(dependencyObject, accumulator, true);
            else
            {
                lock (lockCache)
                {
                    if (mapCacheVisualChildren.ContainsKey(dependencyObject) &&
                        mapCacheVisualChildren[dependencyObject].ContainsKey(type))
                        return mapCacheVisualChildren[dependencyObject][type] as List<T>;

                    AccumulateVisualChildrenOfType(dependencyObject, accumulator, true);

                    if (!mapCacheVisualChildren.ContainsKey(dependencyObject))
                        mapCacheVisualChildren.Add(dependencyObject, new Dictionary<Type, System.Collections.IList>());
                    if (mapCacheVisualChildren[dependencyObject].ContainsKey(type))
                        mapCacheVisualChildren[dependencyObject].Remove(type);
                    mapCacheVisualChildren[dependencyObject].Add(type, accumulator);
                }
            }

            return accumulator;
        }

#if !WINDOWS_UWP
        static Dictionary<DependencyObject, Dictionary<Type, System.Collections.IList>> mapCacheChildrenFirst =
            new Dictionary<DependencyObject, Dictionary<Type, System.Collections.IList>>();

        public static IEnumerable<T> GetChildrenOfTypeBreadthFirst<T>(this DependencyObject dependencyObject) where T : class
        {
            var accumulator = new List<T>();
            var type = accumulator.GetType();
            lock (lockCache)
            {
                if (mapCacheChildrenFirst.ContainsKey(dependencyObject) &&
                    mapCacheChildrenFirst[dependencyObject].ContainsKey(type))
                    return mapCacheChildrenFirst[dependencyObject][type] as List<T>;

                AccumulateChildrenOfType(dependencyObject, accumulator, false);

                if (!mapCacheChildrenFirst.ContainsKey(dependencyObject))
                    mapCacheChildrenFirst.Add(dependencyObject, new Dictionary<Type, System.Collections.IList>());
                if (mapCacheChildrenFirst[dependencyObject].ContainsKey(type))
                    mapCacheChildrenFirst[dependencyObject].Remove(type);
                mapCacheChildrenFirst[dependencyObject].Add(type, accumulator);
            }

            return accumulator;
        }

        public static TAncestor FindAncestor<TAncestor>(this DependencyObject dependencyObject) where TAncestor : class
        {
            if (dependencyObject == null)
                return null;

            var parent = LogicalTreeHelper.GetParent(dependencyObject);
            while (parent != null)
            {
                var casted = parent as TAncestor;
                if (casted != null)
                {
                    return casted;
                }
                parent = LogicalTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static TAncestor FindFirstAncestor<TAncestor>(this DependencyObject dependencyObject) where TAncestor : class
        {
            if (dependencyObject == null)
                return null;

            var parent = LogicalTreeHelper.GetParent(dependencyObject);
            while (parent != null)
            {
                var casted = parent as TAncestor;
                if (casted != null)
                {
                    return casted;
                }
            }
            return null;
        }
#endif
        public static TAncestor FindParent<TAncestor>(this DependencyObject dependencyObject) where TAncestor : class
        {
            if (dependencyObject == null)
                return null;

            var parent = VisualTreeHelper.GetParent(dependencyObject);
            while (parent != null)
            {
                var casted = parent as TAncestor;
                if (casted != null)
                {
                    return casted;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static TAncestor FindFirstParent<TAncestor>(this DependencyObject dependencyObject) where TAncestor : class
        {
            if (dependencyObject == null)
                return null;

            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent != null)
            {
                var casted = parent as TAncestor;
                if (casted != null)
                {
                    return casted;
                }
            }
            return null;
        }

#if !WINDOWS_UWP
        public static bool IsAncestorOf(this DependencyObject dependencyObject, DependencyObject child)
        {
            var accumulator = new List<DependencyObject>();
            /* We can improve efficiency here. There is no need to gather all. */
            AccumulateChildrenOfType(dependencyObject, accumulator, false);
            var result = accumulator.Contains(child);
            return result;
        }

        static void AccumulateChildrenOfType<T>(DependencyObject dependencyObject, ICollection<T> accumulator, bool depthFirst)
            where T : class
        {
            if (dependencyObject == null)
                return;

            if (dependencyObject is ContentControl)
            {
                var control = dependencyObject as ContentControl;
                var childOfType = control.Content as T;
                if (childOfType != null && !accumulator.Contains(childOfType))
                {
                    accumulator.Add(childOfType);
                }
            }

            var children = LogicalTreeHelper.GetChildren(dependencyObject);
            foreach (var child in children)
            {
                //				if (child is System.Windows.Controls.Expander)
                //				{
                //					Console.WriteLine("");
                //				}

                var childOfType = child as T;
                if (childOfType != null && !accumulator.Contains(childOfType))
                {
                    accumulator.Add(childOfType);
                }
                if (depthFirst)
                {
                    var childDependencyObject = child as DependencyObject;
                    if (childDependencyObject != null)
                    {
                        AccumulateChildrenOfType(childDependencyObject, accumulator, true);
                    }
                }
            }

            if (depthFirst)
            {
                return;
            }

            /* Breadth first. */
            foreach (var child in children)
            {
                var childDependencyObject = child as DependencyObject;
                if (childDependencyObject != null)
                {
                    AccumulateChildrenOfType(childDependencyObject, accumulator, false);
                }
            }
        }
#endif
        static void AccumulateVisualChildrenOfType<T>(DependencyObject dependencyObject, ICollection<T> accumulator, bool depthFirst)
            where T : class
        {
            if (dependencyObject == null)
                return;

            if (dependencyObject is ContentControl)
            {
                var control = dependencyObject as ContentControl;
                var childOfType = control.Content as T;
                if (childOfType != null && !accumulator.Contains(childOfType))
                {
                    accumulator.Add(childOfType);
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (int i = 0; i < count; ++i)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var childOfType = child as T;
                if (childOfType != null && !accumulator.Contains(childOfType))
                {
                    accumulator.Add(childOfType);
                }
                if (depthFirst)
                {
                    var childDependencyObject = child as DependencyObject;
                    if (childDependencyObject != null)
                    {
                        AccumulateVisualChildrenOfType(childDependencyObject, accumulator, true);
                    }
                }
            }

            if (depthFirst)
            {
                return;
            }

            /* Breadth first. */
            for (int i = 0; i < count; ++i)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var childDependencyObject = child as DependencyObject;
                if (childDependencyObject != null)
                {
                    AccumulateVisualChildrenOfType(childDependencyObject, accumulator, false);
                }
            }
        }

        public static KeyValuePair<UIElement, Tuple<DependencyProperty, string>> GetControlText(this FrameworkElement mainControl)
        {
            KeyValuePair<UIElement, Tuple<DependencyProperty, string>> ret = default(KeyValuePair<UIElement, Tuple<DependencyProperty, string>>);

            var txtbox = mainControl as TextBox;
            if (txtbox != null)
                ret = new KeyValuePair<UIElement, Tuple<DependencyProperty, string>>(mainControl, new Tuple<DependencyProperty, string>(TextBox.TextProperty, txtbox.Text));

            var chkbox = mainControl as CheckBox;
            if (chkbox != null && (chkbox.Content is string || chkbox.Content == null))
                ret = new KeyValuePair<UIElement, Tuple<DependencyProperty, string>>(mainControl, new Tuple<DependencyProperty, string>(CheckBox.ContentProperty, (string) chkbox.Content));

            var btn = mainControl as Button;
            if (btn != null && (btn.Content is string || btn.Content == null))
                ret = new KeyValuePair<UIElement, Tuple<DependencyProperty, string>>(mainControl, new Tuple<DependencyProperty, string>(Button.ContentProperty, (string) btn.Content));

            //            using (var cursor = new WaitCursor())
            //            {
            //#if !WINDOWS_UWP
            //                var listChildTooltip = (from c in mainControl.GetVisualChildrenOfType<FrameworkElement>() where c.ToolTip as String != null select c).ToList();
            //                listChildTooltip.ForEach(control =>
            //                {
            //                    ret.Add(control, new Tuple<string, string>(FrameworkElement.ToolTipProperty.Name, (string)control.ToolTip));
            //                });
            //#endif
            //                var listControls = mainControl.GetVisualChildrenOfType<ContentControl>().ToList();
            //                var listControlsText = (from c in listControls where c.Content as String != null select c).ToList();
            //                if ((mainControl as ContentControl)?.Content as string != null)
            //                    listControlsText.Add(mainControl as ContentControl);
            //                listControlsText.ForEach(control =>
            //                {
            //                    var content = control.Content as String;
            //                    if (!String.IsNullOrEmpty(content))
            //                        ret.Add(control, new Tuple<string, string>(ContentControl.ContentProperty.Name, (string)control.Content));
            //                });

            //                var listTextBoxes = (from c in mainControl.GetVisualChildrenOfType<TextBox>() select c).ToList();
            //                if (mainControl is TextBox)
            //                    listTextBoxes.Add(mainControl as TextBox);
            //                listTextBoxes.ForEach(control =>
            //                {
            //                    ret.Add(control, new Tuple<string, string>(TextBox.TextProperty.Name, control.Text));
            //                });

            //                var listTextBlockes = (from c in mainControl.GetVisualChildrenOfType<TextBlock>() select c).ToList();
            //                if (mainControl is TextBlock)
            //                    listTextBlockes.Add(mainControl as TextBlock);
            //                listTextBlockes.ForEach(control =>
            //                {
            //                    ret.Add(control, new Tuple<string, string>(TextBlock.TextProperty.Name, control.Text));
            //                });
            //#if !WINDOWS_UWP
            //                var listHeaderControls = (from c in mainControl.GetVisualChildrenOfType<HeaderedContentControl>() where c.Header as String != null select c).ToList();
            //                if ((mainControl as HeaderedContentControl)?.Header is string)
            //                    listHeaderControls.Add(mainControl as HeaderedContentControl);
            //                listHeaderControls.ForEach(control =>
            //                {
            //                    ret.Add(control, new Tuple<string, string>(HeaderedContentControl.HeaderProperty.Name, (string)control.Header));
            //                });
            //#endif
            //                //var listComboBoxBlockes = (from c in mainControl.GetVisualChildrenOfType<ComboBox>() select c).ToList();
            //                //if (mainControl is ComboBox)
            //                //    listComboBoxBlockes.Add(mainControl as ComboBox);
            //                //ret.AddRange(listComboBoxBlockes);

            //                //var listListBoxBlockes = (from c in mainControl.GetVisualChildrenOfType<ListBox>() select c).ToList();
            //                //if (mainControl is ListBox)
            //                //    listListBoxBlockes.Add(mainControl as ListBox);
            //                //ret.AddRange(listListBoxBlockes);

            //                if (mainControl.ToolTip is String && !ret.Keys.Contains(mainControl))
            //                    ret.Add(mainControl, new Tuple<string, string>(FrameworkElement.ToolTipProperty.Name, (string)mainControl.ToolTip));
            //            }
            return ret;
        }

#if !WINDOWS_UWP

        public static T GetUnsetPropertyValue<T>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, T value, T defValue, bool bUseNullValue = true)
        {
            if (dependencyObject.ReadLocalValue(dependencyProperty) != DependencyProperty.UnsetValue)
            {
                if (bUseNullValue)
                    return value;
                else if (value != null)
                    return value;
                else
                    return defValue;
            }
            else
                return defValue;
        }

        public static void UpdateAllBindingSources(this DependencyObject obj)
        {
            foreach (var binding in obj.GetAllBindings())
                binding.UpdateSource();
        }

        public static IEnumerable<BindingExpression> GetAllBindings(this DependencyObject obj, bool bDeep = false)
        {
            var stack = new Stack<DependencyObject>();

            stack.Push(obj);

            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                var lve = cur.GetLocalValueEnumerator();

                while (lve.MoveNext())
                    if (BindingOperations.IsDataBound(cur, lve.Current.Property))
                        yield return lve.Current.Value as BindingExpression;

                if (bDeep)
                {
                    int count = VisualTreeHelper.GetChildrenCount(cur);
                    for (int i = 0; i < count; ++i)
                    {
                        var child = VisualTreeHelper.GetChild(cur, i);
                        if (child is FrameworkElement)
                            stack.Push(child);
                    }
                }
            }
        }

        public static Dictionary<LocalValueEntry, FrameworkElement> GetAllValueEntries(this DependencyObject obj, bool bDeep = true)
        {
            var ret = new Dictionary<LocalValueEntry, FrameworkElement>();
            var stack = new Stack<DependencyObject>();

            stack.Push(obj);

            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                var lve = cur.GetLocalValueEnumerator();

                while (lve.MoveNext())
                    if (BindingOperations.IsDataBound(cur, lve.Current.Property))
                        ret.Add(lve.Current, cur as FrameworkElement);

                if (bDeep)
                {
                    bDeep = false;
                    var found = cur.GetChildrenOfType<FrameworkElement>();
                    foreach(var el in found)
                        stack.Push(el);

                    //int count = VisualTreeHelper.GetChildrenCount(cur);
                    //for (int i = 0; i < count; ++i)
                    //{
                    //    var child = VisualTreeHelper.GetChild(cur, i);
                    //    if (child is FrameworkElement)
                    //        stack.Push(child);
                    //}
                }
            }

            return ret;
        }

        public static BindingBase CloneBinding(BindingBase bindingBase)
        {
            var binding = bindingBase as Binding;
            if (binding != null)
            {
                var result = new Binding
                {
                    // Source = binding.Source,
                    // AsyncState = binding.AsyncState,
                    // BindingGroupName = binding.BindingGroupName,
                    // BindsDirectlyToSource = binding.BindsDirectlyToSource,
                    Converter = binding.Converter,
                    // ConverterCulture = binding.ConverterCulture,
                    ConverterParameter = binding.ConverterCulture,
                    // ElementName = binding.ElementName,
                    // FallbackValue = binding.FallbackValue,
                    // IsAsync = binding.IsAsync,
                    Mode = binding.Mode,
                    // NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                    // NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                    // NotifyOnValidationError = binding.NotifyOnValidationError,
                    Path = new PropertyPath(binding.Path.Path),
                    // RelativeSource = binding.RelativeSource,
                    StringFormat = binding.StringFormat,
                    // TargetNullValue = binding.TargetNullValue,
                    // UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = binding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = binding.ValidatesOnExceptions,
                    // XPath = binding.XPath,
                };

                foreach (var validationRule in binding.ValidationRules)
                {
                    result.ValidationRules.Add(validationRule);
                }

                return result;
            }

            var multiBinding = bindingBase as MultiBinding;
            if (multiBinding != null)
            {
                var result = new MultiBinding
                {
                    BindingGroupName = multiBinding.BindingGroupName,
                    Converter = multiBinding.Converter,
                    ConverterCulture = multiBinding.ConverterCulture,
                    ConverterParameter = multiBinding.ConverterParameter,
                    FallbackValue = multiBinding.FallbackValue,
                    Mode = multiBinding.Mode,
                    NotifyOnSourceUpdated = multiBinding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = multiBinding.NotifyOnTargetUpdated,
                    NotifyOnValidationError = multiBinding.NotifyOnValidationError,
                    StringFormat = multiBinding.StringFormat,
                    TargetNullValue = multiBinding.TargetNullValue,
                    UpdateSourceExceptionFilter = multiBinding.UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = multiBinding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = multiBinding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = multiBinding.ValidatesOnDataErrors,
                };

                foreach (var validationRule in multiBinding.ValidationRules)
                {
                    result.ValidationRules.Add(validationRule);
                }

                foreach (var childBinding in multiBinding.Bindings)
                {
                    result.Bindings.Add(CloneBinding(childBinding));
                }

                return result;
            }

            var priorityBinding = bindingBase as PriorityBinding;
            if (priorityBinding != null)
            {
                var result = new PriorityBinding
                {
                    BindingGroupName = priorityBinding.BindingGroupName,
                    FallbackValue = priorityBinding.FallbackValue,
                    StringFormat = priorityBinding.StringFormat,
                    TargetNullValue = priorityBinding.TargetNullValue,
                };

                foreach (var childBinding in priorityBinding.Bindings)
                {
                    result.Bindings.Add(CloneBinding(childBinding));
                }

                return result;
            }

            throw new NotSupportedException("Failed to clone binding");
        }

        // This is here 'til future versions of WPF provide this functionality
        public static bool ValidateBindings(this DependencyObject parent, bool validateOnlyVisibleElements = false)
        {
            ReadOnlyObservableCollection<ValidationError> ve = Validation.GetErrors(parent);
            if (ve != null && ve.Count > 0)
                return false;

            // Validate all the bindings on the parent
            bool valid = true;
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    if (binding != null)
                    {
                        foreach (ValidationRule rule in binding.ValidationRules)
                        {
                            // TODO: where to get correct culture info?
                            ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
                            if (!result.IsValid)
                            {
                                BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
                                Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
                                valid = false;
                            }
                        }
                    }
                }
            }

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (validateOnlyVisibleElements)
                {
                    if (child is FrameworkElement && (child as FrameworkElement).IsVisible == true)
                    {
                        if (!ValidateBindings(child, validateOnlyVisibleElements))
                        {
                            valid = false;
                        }
                    }
                }
                else
                {
                    if (!ValidateBindings(child, validateOnlyVisibleElements))
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        public static void UpdateAllBindings(DependencyObject o)
        {
            //Immediate Properties
            List<FieldInfo> propertiesAll = new List<FieldInfo>();
            Type currentLevel = o.GetType();
            // while (currentLevel != typeof(object))
            {
                propertiesAll.AddRange(currentLevel.GetFields());
                // currentLevel = currentLevel.BaseType;
            }
            var propertiesDp = propertiesAll.Where(x => x.FieldType == typeof(DependencyProperty));
            foreach (var property in propertiesDp)
            {
                BindingExpression ex = BindingOperations.GetBindingExpression(o, property.GetValue(o) as DependencyProperty);
                if (ex != null)
                {
                    var binding = DependencyObjectExtensions.CloneBinding(ex.ParentBinding) as Binding;
                    binding.RelativeSource = ex.ParentBinding.RelativeSource;
                    BindingOperations.SetBinding(o, property.GetValue(o) as DependencyProperty, binding);
                }
            }
        }

        private static readonly ILog log = LogManager.GetLogger("Utilities");
#endif
        public static void Dispose(this FrameworkElement fe)
        {
            var disposables = (from c in fe.GetChildrenOfType<FrameworkElement>()
                               where c is IDisposable && c.GetType().GetMethod("System.IDisposable.Dispose", BindingFlags.NonPublic | BindingFlags.Instance) == null
                               select c).ToList();

            if (fe is IDisposable && fe.GetType().GetMethod("System.IDisposable.Dispose", BindingFlags.NonPublic | BindingFlags.Instance) == null)
                disposables.Add(fe);

            disposables.ForEach(c =>
            {
                try
                {
                    (c as IDisposable).Dispose();
                }
                catch (Exception ex)
                {
#if !WINDOWS_UWP
                    log.Error(String.Format("An error occured Disposing the control type {0}", c.GetType().FullName), ex);
#endif
                }
            });

            CleanChildrenOfTypeCache(fe);
        }

        public static void Dispose(this Panel panel)
        {
            var disposables = (from c in panel.GetChildrenOfType<FrameworkElement>()
                               where c is IDisposable && c.GetType().GetMethod("System.IDisposable.Dispose", BindingFlags.NonPublic | BindingFlags.Instance) == null
                               select c).ToList();
#if !WINDOWS_UWP
            foreach (var uie in panel.Children)
            {
                try
                {
                    //if (uie is IDisposable)
                    //    (uie as IDisposable).Dispose();
                    //if (uie is ContentControl && (uie as ContentControl).Content is IDisposable)
                    //    ((uie as ContentControl).Content as IDisposable).Dispose();
                    BindingOperations.ClearAllBindings(uie as DependencyObject);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("An error occured cleaning all Binding for control type {0}", uie.GetType().FullName), ex);
                }
            }
#endif
            disposables.ForEach(c =>
            {
                try
                {
                    (c as IDisposable).Dispose();
                }
                catch (Exception ex)
                {
#if !WINDOWS_UWP
                    log.Error(String.Format("An error occured Disposing the control type {0}", c.GetType().FullName), ex);
#endif
                }
            });

            try
            {
                panel.Children.Clear();
            }
            catch (Exception ex)
            {
#if !WINDOWS_UWP
                log.Error(String.Format("An error occured Cleaning children of panel type {0}", panel.GetType().FullName), ex);
#endif
            }

            CleanChildrenOfTypeCache(panel);
        }

#if !WINDOWS_UWP
        public static void Dispose(this InkCanvas panel)
        {
            var disposables = (from c in panel.GetChildrenOfType<FrameworkElement>()
                               where c is IDisposable && c.GetType().GetMethod("System.IDisposable.Dispose", BindingFlags.NonPublic | BindingFlags.Instance) == null
                               select c).ToList();
            foreach (var uie in panel.Children)
            {
                try
                {
                    //if (uie is IDisposable)
                    //    (uie as IDisposable).Dispose();
                    //if (uie is ContentControl && (uie as ContentControl).Content is IDisposable)
                    //    ((uie as ContentControl).Content as IDisposable).Dispose();
                    BindingOperations.ClearAllBindings(uie as DependencyObject);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("An error occured cleaning all Binding for control type {0}", uie.GetType().FullName), ex);
                }
            }

            disposables.ForEach(c =>
            {
                try
                {
                    (c as IDisposable).Dispose();
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("An error occured Disposing the control type {0}", c.GetType().FullName), ex);
                }
            });

            try
            {
                panel.Children.Clear();
            }
            catch (Exception ex)
            {
                log.Error(String.Format("An error occured Cleaning children of panel type {0}", panel.GetType().FullName), ex);
            }

            CleanChildrenOfTypeCache(panel);
        }
#endif
    }
}
