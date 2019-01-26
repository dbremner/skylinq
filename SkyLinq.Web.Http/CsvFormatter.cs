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
    public sealed class CsvFormatter : BufferedMediaTypeFormatter 
    {
        public CsvFormatter()
        {
            // Add the supported media type.
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }

        /// <summary>
        /// Only support IEnumerable<IDictionary<string, TValue>> at this time. TValue can be anything.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
                    int rowNo = 0;
                    //row must be IDictionary which support IEnumerable
                    foreach (IEnumerable row in rows)
                    {
                        rowNo++;
                        if (rowNo == 1)
                        {
                            //First row, write the column header
                            WriteHeaders(writer, row);
                            writer.WriteLine();
                        }
                        else
                        {
                            writer.WriteLine();
                        }
                        WriteValues(writer, row);
                    }
                }
            }
            writeStream.Close();
        }

        private static void WriteValues(StreamWriter writer, IEnumerable row)
        {
            int colNo = 0;
            foreach (dynamic kvp in row)
            {
                colNo++;
                if (colNo > 1)
                {
                    writer.Write(',');
                }

                object val = kvp.Value;
                if (val != null)
                {
                    Type valType = val.GetType();
                    if (valType.IsPrimitive || valType == typeof(DateTime))
                    {
                        writer.Write("{0}", val);
                    }
                    else
                    {
                        writer.Write("\"{0}\"", val.ToString().Replace("\"", "\"\""));
                    }
                }
            }
        }

        private static void WriteHeaders(StreamWriter writer, IEnumerable row)
        {
            int colNo = 0;
            foreach (dynamic kvp in row)
            {
                colNo++;
                if (colNo > 1)
                {
                    writer.Write(',');
                }

                writer.Write("\"{0}\"", kvp.Key);
            }
        }
    }
}
