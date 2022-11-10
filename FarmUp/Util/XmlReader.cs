using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Util
{
    public class XmlReader
    {
        private static XmlDocument doc = new XmlDocument();
        private static String _filePath;
        private static String _rootNode;

        public static Dictionary<string, object> Read(string filepath, string rootNode = "/setting/*")
        {
            _filePath = filepath;
            _rootNode = rootNode;

            doc.Load(filepath);
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (XmlNode n in doc.SelectNodes(rootNode))
            {
                d[n.Name] = n.InnerText.Trim();
            }
            return d;
        }

        public static T Read<T>(string filepath, string rootNode = "/setting/*")
        {
            doc.Load(filepath);
            T data = (T)Activator.CreateInstance(typeof(T), null);
            foreach (XmlNode n in doc.SelectNodes(rootNode))
            {
                Type type = data.GetType();
                System.Reflection.PropertyInfo prop = type.GetProperty(n.Name);
                if (prop != null)
                {
                    if (prop.PropertyType.Equals(Type.GetType("System.String")))
                    {
                        prop.SetValue(data, n.InnerText.Trim());
                    }
                    else
                    {
                        //generic convert
                        var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                        if (converter != null && converter.IsValid(n.InnerText.Trim()))
                        {
                            prop.SetValue(data, converter.ConvertFromString(n.InnerText.Trim()));
                        }
                    }
                }
            }
            return data;
        }

        public static void Save<T>(T setting, string filepath, string rootNode = "/setting/*")
        {
            foreach (XmlNode n in doc.SelectNodes(rootNode))
            {
                Type type = setting.GetType();
                System.Reflection.PropertyInfo prop = type.GetProperty(n.Name);

                n.InnerText = prop.GetValue(setting).ToString();
            }

            doc.Save(filepath);
        }
    }
}
