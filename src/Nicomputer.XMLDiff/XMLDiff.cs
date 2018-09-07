using System;
using Microsoft.XmlDiffPatch;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace Nicomputer
{
    public class XMLDiff
    {
        
        public List<string> XpathList {get; set;}

        public XMLDiff () {
            XpathList = new List<string>{};
        }

        /// <summary>
        /// In order to ignore XML nodes, we need to remove them from the XmlNode objects
        /// </summary>
        /// <param name="xmlString1"></param>
        /// <param name="xmlString2"></param>
        public string Compare(string xmlString1, string xmlString2)
        {
            IgnoreXmlCase(ref xmlString1);
            IgnoreXmlCase(ref xmlString2);

            var xmlNode1 = StringToXmlNode(xmlString1);
            var xmlNode2 = StringToXmlNode(xmlString2);

            IgnoreXmlNodes(xmlNode1);
            IgnoreXmlNodes(xmlNode2);

            var result = CompareXmlNodes(xmlNode1, xmlNode2);

            return result;
        }

        public string CompareXmlNodes(XmlNode legacyXmlNode, XmlNode newcallXmlNode)
        {
            string xmlDiff = null;
            var sw = new StringWriter();
            using (var xw = new XmlTextWriter(sw) { Formatting = System.Xml.Formatting.Indented })
            {
                XmlDiff diff = new XmlDiff
                {
                    Options = XmlDiffOptions.IgnorePI |
                                XmlDiffOptions.IgnoreChildOrder |
                                XmlDiffOptions.IgnoreComments |
                                XmlDiffOptions.IgnoreWhitespace |
                                XmlDiffOptions.IgnoreNamespaces |
                                XmlDiffOptions.IgnoreDtd |
                                XmlDiffOptions.IgnorePrefixes |
                                XmlDiffOptions.IgnoreXmlDecl
                };

                var result = diff.Compare(legacyXmlNode, newcallXmlNode, xw);

                if (result == false)
                {
                    xmlDiff = sw.ToString();
                }
            }
            
            return xmlDiff;
        }
        
        private void IgnoreXmlCase(ref string origString)
        {
           origString = origString.ToLower();
        }

        /// <summary>
        /// In order to ignore XML nodes, we need to remove them from the XmlNode objects
        /// </summary>
        /// <param name="xmlNode"></param>
        private void IgnoreXmlNodes(XmlNode xmlNode)
        {
            RemoveXmlNodes(xmlNode, XpathList);
        }

        private XmlNode StringToXmlNode(string input)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);
            XmlNode newNode = doc.DocumentElement;
            return newNode;
        }

        private XmlNode RemoveXmlNodes(XmlNode inputXml, List<string> xpathList)
        {
            foreach (var xpath in xpathList)
            {
                var node = inputXml.SelectSingleNode(xpath);

                if (node != null)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
                        
            return inputXml;
        }
    }
}
