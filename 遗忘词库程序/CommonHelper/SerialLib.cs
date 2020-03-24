using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace CommonHelper
{
    public class SerialLib
    {

        public static object ThreadLockObject = new object();
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objModel"></param>
        /// <param name="sFilePath"></param>
        public static void SerializeBinary<T>(T objModel, string sFilePath) where T : class
        {
            lock (ThreadLockObject)
            {

                using (FileStream fs = new FileStream(sFilePath, FileMode.Create))
                {
                    IFormatter format = new BinaryFormatter();

                    format.Serialize(fs, objModel);
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        public static T DeserializeBinary<T>(string sFilePath) where T : class
        {
            lock (ThreadLockObject)
            {
                if (File.Exists(sFilePath))
                {
                    using (FileStream fs = new FileStream(sFilePath, FileMode.Open))
                    {
                        IFormatter format = new BinaryFormatter();
                        return (format.Deserialize(fs)) as T;
                    }
                }
                return default(T);
            }
        }

        /// <summary>
        /// 序列化对象为XML文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objModel"></param>
        /// <param name="sFilePath"></param>
        public static void SerializeXml<T>(T objModel, string sFilePath) where T : class
        {
            lock (ThreadLockObject)
            {

                using (StreamWriter sw = new StreamWriter(sFilePath))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(sw, objModel);
                    sw.Close();


                }
            }
        }
        /// <summary>
        /// 反序列化XML文档为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sFilePath"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(string sFilePath) where T : class
        {
            lock (ThreadLockObject)
            {
                if (File.Exists(sFilePath))
                {
                    using (FileStream fs = new FileStream(sFilePath, FileMode.Open))
                    {
                        using (XmlTextReader xtr = new XmlTextReader(fs)) //解决文本框中丢失换行符的问题
                        {
                            xtr.Normalization = false;
                            XmlSerializer xs = new XmlSerializer(typeof(T));
                            return (xs.Deserialize(xtr)) as T;
                        }
                    }
                }

                return default(T);
            }


        }

        /// <summary>
        /// 序列化对象为XML字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objModel"></param>
        /// <param name="sFilePath"></param>
        public static string SerializeStrXml<T>(T objModel) where T : class
        {
            lock (ThreadLockObject)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(sw, objModel);
                    sw.Close();
                }
                return sb.ToString();
            }
        }


        /// <summary>
        /// 反序化Xml字符串为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sStrXml"></param>
        /// <returns></returns>
        public static T DeserializeStrXml<T>(string sStrXml) where T : class
        {
            lock (ThreadLockObject)
            {
                using (StringReader sr = new StringReader(sStrXml))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return (xs.Deserialize(sr)) as T;
                }
            }
        }
    }
}
