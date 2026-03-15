using System;
using System.Net;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using AmazedSaint.Elastic.Lib;

namespace AmazedSaint.Elastic
{
    /// <summary>
    /// Extension methods for our ElasticObject. 
    /// See http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
    /// </summary>
    public static class DynamicExtensions
    {

        /// <summary>
        /// Converts an XElement to the expando
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static dynamic ToElastic(this XElement e)
        {
            return (dynamic) ElasticFromXElement(e);
        }


        /// <summary>
        /// Converts an expando to XElement
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static XElement ToXElement(this ElasticObject e)
        {
            return (dynamic)XElementFromElastic(e);
        }

        /// <summary>
        /// Build an expando from an XElement
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static ElasticObject ElasticFromXElement(XElement el) 
        {
            var exp = new ElasticObject();

            if (!string.IsNullOrEmpty(el.Value))
                exp.InternalValue = el.Value;

            exp.InternalName = el.Name.LocalName;

            foreach (var a in el.Attributes())
                exp.CreateOrGetAttribute(a.Name.LocalName, a.Value);


            var textNode= el.Nodes().FirstOrDefault();
             if (textNode is XText) 
                {
                    exp.InternalContent = textNode.ToString();
                }

            foreach (var c in el.Elements())
            {

                var child = ElasticFromXElement(c);
                child.InternalParent = exp;
                exp.AddElement(child);
            }
            return exp;
        }


        /// <summary>
        /// Returns an XElement from an ElasticObject
        /// </summary>
        /// <param name="elastic"></param>
        /// <returns></returns>
        public static XElement XElementFromElastic(ElasticObject elastic)
        {

            var exp = new XElement(elastic.InternalName);


            foreach (var a in elastic.Attributes)
            {
                    if (a.Value.InternalValue != null)
                        exp.Add(new XAttribute(a.Key, a.Value.InternalValue));
            }

            if (null != elastic.InternalContent && elastic.InternalContent is string)
            {
                exp.Add(new XText(elastic.InternalContent as string));
            }

            foreach (var c in elastic.Elements)
            {
                    var child = XElementFromElastic(c);
                    exp.Add(child);
            }
            return exp;
        }


    }



}