using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Bangk.Common.Utils
{
    public class XmlToDynamic
    {
        public static void Parse(dynamic parent, IEnumerable<XElement> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.HasElements)
                {
                    if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                    {
                        //list
                        var item = new ExpandoObject();
                        var list = new List<dynamic>();
                        Parse(list, node.Elements());
                        AddProperty(item, node.Elements().First().Name.LocalName, list);
                        AddProperty(parent, node.Name.ToString(), item);
                    }
                    else
                    {
                        var item = new ExpandoObject();
                        foreach (var attribute in node.Attributes())
                        {
                            AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                        }

                        Parse(item, node.Elements());
                        AddProperty(parent, node.Name.ToString(), item);
                    }
                }
                else
                {
                    AddProperty(parent, node.Name.ToString(), node.Value.Trim());
                }
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<string, object>)[name] = value;
            }
        }
    }
}