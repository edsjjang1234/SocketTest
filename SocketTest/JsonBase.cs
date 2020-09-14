using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SocketTest
{
    [DataContract]
    public class JsonBase
    {
        protected string Serialize<T>(T obj)
        {
            using (MemoryStream stream1 = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream1, obj);

                stream1.Position = 0;
                using (StreamReader sr = new StreamReader(stream1))
                {
                    var jsonStr = sr.ReadToEnd();
                    return jsonStr;
                }
            }

        }

        protected static T Deseialize<T>(string text)
        {
            try
            {
                if (text.Last() != '}' && text.LastIndexOf('}') + 1 < text.Length)
                {
                    text = text.Remove(text.LastIndexOf('}') + 1);
                }
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.Unicode.GetBytes(text)))
            {
                return (T)js.ReadObject(ms);
            }
        }
    }
}
