using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net;

namespace SkyLinq.Web.Http
{
    public class CsvFormatter : BufferedMediaTypeFormatter 
    {
        public CsvFormatter()
        {
            // Add the supported media type.
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }

        public override bool CanWriteType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                    Type[] argTypes = type.GetGenericArguments();
                    if (argTypes.Length == 1)
                    {
                        Type innerType = argTypes[0];
                        if (innerType.IsGenericType && (innerType.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                            || (innerType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))))
                        {
                            Type[] dictTypes = innerType.GetGenericArguments();
                            if (dictTypes[0] == typeof(string))
                            {
                                return true;
                            }
                        }
                    }
            }
            return false;
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                var rows = value as IEnumerable;
                if (rows != null)
                {
                    foreach (var row in rows)
                    {

                    }
                }
            }
            writeStream.Close();
        }

        static char[] _specialChars = new char[] { ',', '\n', '\r', '"' };

        private string Escape(object o)
        {
            if (o == null)
            {
                return "";
            }
            string field = o.ToString();
            if (field.IndexOfAny(_specialChars) != -1)
            {
                return String.Format("\"{0}\"", field.Replace("\"", "\"\""));
            }
            else return field;
        }
    }
}
