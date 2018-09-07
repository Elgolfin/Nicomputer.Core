using System;
using Microsoft.XmlDiffPatch;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace Nicomputer.Core
{
    public class XmlDiff
    {
        
        public List<string> XpathList {get; set;}
        public XmlNode ComparedXmlNode1 {get; private set;}
        public XmlNode ComparedXmlNode2 {get; private set;}
        public string XmlDiffResult {get; private set;}

        public XmlDiff () {
            XpathList = new List<string>{};
            XmlDiffResult = String.Empty;
        }

        /// <summary>
        /// Compare two XML Nodes
        /// </summary>
        /// <param name="xmlString1"></param>
        /// <param name="xmlString2"></param>
        public bool Compare(string xmlString1, string xmlString2, bool ignoreCase)
        {
            if (ignoreCase) {
                IgnoreXmlCase(ref xmlString1);
                IgnoreXmlCase(ref xmlString2);
            }

            var xmlNode1 = StringToXmlNode(xmlString1);
            var xmlNode2 = StringToXmlNode(xmlString2);

            IgnoreXmlNodes(xmlNode1);
            IgnoreXmlNodes(xmlNode2);
            
            ComparedXmlNode1 = xmlNode1;
            ComparedXmlNode2 = xmlNode2;

            return CompareXmlNodes(xmlNode1, xmlNode2);
        }

        public bool CompareXmlNodes(XmlNode xmlNode1, XmlNode xmlNode2)
        {
            bool result;
            var sw = new StringWriter();
            using (var xw = new XmlTextWriter(sw) { Formatting = System.Xml.Formatting.Indented })
            {
                Microsoft.XmlDiffPatch.XmlDiff diff = new Microsoft.XmlDiffPatch.XmlDiff
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

                result = diff.Compare(xmlNode1, xmlNode2, xw);

                if (result == false)
                {
                    XmlDiffResult = sw.ToString();
                }
            }
            
            return result;
        }
        
        private void IgnoreXmlCase(ref string origString)
        {
           origString = origString.ToLower();
        }

        /// <summary>
        /// In order to ignore XML nodes,
        /// we need to remove them from the XmlNode objects listed in the XPathList property
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
