#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;

using System.Diagnostics;
using System.Reflection;
using System.Xml;

using Syncfusion;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Factory for tag elements. Creates classes, responsible for HTML tags.
    /// </summary>
    public sealed class ElementsFactory
    {
        #region Class constants
        /// <summary>
        /// Max number of such classes, responsible for tags.
        /// </summary>
        private const int DEF_RESERVE_SIZE = 50;
        #endregion

        #region Class members
        /// <summary>
        /// Holds class instances, responsible for HTML tags.
        /// </summary>
        private static Hashtable m_dict = CollectionsUtil.CreateCaseInsensitiveHashtable(DEF_RESERVE_SIZE);
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the ElementsFactory class
        /// </summary>
        static ElementsFactory()
        {
#if DEBUG
            DateTime now = DateTime.Now;
            Debug.WriteLine("Init Factory");
#endif
            try
            {
                Assembly asm = typeof(ElementsFactory).Assembly;
                Type[] types = asm.GetTypes();

                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    object[] attribs = type.GetCustomAttributes(typeof(ElementTagAttribute), true);

                    if (attribs != null && attribs.Length > 0)
                    {
                        object[] args = new object[] { null };

                        object item = null;
                        ElementTagAttribute elm = null;

                        for (int index = 0, len = attribs.Length; index < len; index++)
                        {
                            item = attribs[index];
                            if (item is ElementTagAttribute == false) continue;

                            elm = item as ElementTagAttribute;
                            m_dict[elm.Name] = Activator.CreateInstance(type, args);

                            /*
                              Debug.WriteLine( string.Format(
                                "m_dict[\"{0}\"]=Activator.CreateInstance(typeof({1}),null);",
                                elm.Name, type.Name ) );
                              */
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                CreateElements();
            }

#if DEBUG
            TimeSpan diff = DateTime.Now.Subtract(now);
            Debug.WriteLine(diff, "Class Extract Performance");
#endif
        }

        #endregion

        #region Class static methods
        /// <summary>
        /// Adds the specified custom element to the factory of known elements.
        /// </summary>
        /// <param name="customElement">Instance of the element.</param>
        /// <returns>True if element was successfully added to factory;
        /// False if element with such name already exists.</returns>
        public static bool Add(IHTMLElement customElement)
        {
            if (customElement == null)
                throw new ArgumentNullException("customElement");

            object existing = m_dict[customElement.Name];
            bool success = existing == null;

            if (success)
            {
                m_dict[customElement.Name] = customElement;
            }

            return success;
        }

        /// <summary>
        /// Removes the specified element from the factory of known elements.
        /// </summary>
        /// <param name="customElement">Instance of such custom tag element.</param>
        public static void Remove(IHTMLElement customElement)
        {
            if (customElement == null)
                throw new ArgumentNullException("customElement");

            m_dict.Remove(customElement.Name);
        }
        #endregion

        #region Class Helper Methods
        /// <summary>
        /// Returns an instance of the element with the specified tag name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>Created element object.</returns>
        public static BaseElement GetElement(string tagName)
        {
            if (m_dict.ContainsKey(tagName))
            {
                return (BaseElement)((ICloneable)m_dict[tagName]).Clone();
            }

            return new UnknownElementImpl(null, tagName);
        }

        /// <summary>
        /// Converts XML element.
        /// </summary>
        /// <param name="control">Control object.</param>
        /// <param name="element">XML element which represents the element.</param>
        /// <returns>Converted element.</returns>
        public static BaseElement ConvertTo(HTMLUIControl control, XmlElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            BaseElement output = GetElement(element.Name);
            output.InfillFromXMLElement(control, element);

            return output;
        }

        /// <summary>
        /// Creates instances of the elements.
        /// </summary>
        private static void CreateElements()
        {
            object[] args = new object[] { null };

            m_dict["a"] = Activator.CreateInstance(typeof(AElementImpl), args);
            m_dict["b"] = Activator.CreateInstance(typeof(BElementImpl), args);
            m_dict["body"] = Activator.CreateInstance(typeof(BODYElementImpl), args);
            m_dict["br"] = Activator.CreateInstance(typeof(BRElementImpl), args);
            m_dict["code"] = Activator.CreateInstance(typeof(CODElementImpl), args);
            m_dict["div"] = Activator.CreateInstance(typeof(DIVElementImpl), args);
            m_dict["i"] = Activator.CreateInstance(typeof(IElementImpl), args);
            m_dict["em"] = Activator.CreateInstance(typeof(EMElementImpl), args);
            m_dict["font"] = Activator.CreateInstance(typeof(FONTElementImpl), args);
            m_dict["form"] = Activator.CreateInstance(typeof(FORMElementImpl), args);
            m_dict["h1"] = Activator.CreateInstance(typeof(H1ElementImpl), args);
            m_dict["h2"] = Activator.CreateInstance(typeof(H2ElementImpl), args);
            m_dict["h3"] = Activator.CreateInstance(typeof(H3ElementImpl), args);
            m_dict["h4"] = Activator.CreateInstance(typeof(H4ElementImpl), args);
            m_dict["h5"] = Activator.CreateInstance(typeof(H5ElementImpl), args);
            m_dict["h6"] = Activator.CreateInstance(typeof(H6ElementImpl), args);
            m_dict["head"] = Activator.CreateInstance(typeof(HEADElementImpl), args);
            m_dict["hr"] = Activator.CreateInstance(typeof(HRElementImpl), args);
            m_dict["html"] = Activator.CreateInstance(typeof(HTMLElementImpl), args);
            m_dict["img"] = Activator.CreateInstance(typeof(IMGElementImpl), args);
            m_dict["input"] = Activator.CreateInstance(typeof(INPUTElementImpl), args);
            m_dict["li"] = Activator.CreateInstance(typeof(LIElementImpl), args);
            m_dict["link"] = Activator.CreateInstance(typeof(LinkElementImpl), args);
            m_dict["ol"] = Activator.CreateInstance(typeof(OLElementImpl), args);
            m_dict["p"] = Activator.CreateInstance(typeof(PElementImpl), args);
            m_dict["pre"] = Activator.CreateInstance(typeof(PREElementImpl), args);
            m_dict["script"] = Activator.CreateInstance(typeof(SCRIPTElementImpl), args);
            m_dict["select"] = Activator.CreateInstance(typeof(SELECTElementImpl), args);
            m_dict["span"] = Activator.CreateInstance(typeof(SPANElementImpl), args);
            m_dict["strong"] = Activator.CreateInstance(typeof(STRONGElementImpl), args);
            m_dict["style"] = Activator.CreateInstance(typeof(StyleElementImpl), args);
            m_dict["table"] = Activator.CreateInstance(typeof(TABLEElementImpl), args);
            m_dict["td"] = Activator.CreateInstance(typeof(TDElementImpl), args);
            m_dict["textarea"] = Activator.CreateInstance(typeof(TEXTAREAElementImpl), args);
            m_dict["th"] = Activator.CreateInstance(typeof(THElementImpl), args);
            m_dict["tr"] = Activator.CreateInstance(typeof(TRElementImpl), args);
            m_dict["u"] = Activator.CreateInstance(typeof(UElementImpl), args);
            m_dict["ul"] = Activator.CreateInstance(typeof(ULElementImpl), args);
            m_dict["sup"] = Activator.CreateInstance(typeof(SUPElementImpl), args);
            m_dict["sub"] = Activator.CreateInstance(typeof(SUBElementImpl), args);
        }
        #endregion
    }
}