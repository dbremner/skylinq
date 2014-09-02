using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace LINQPadHost
{
    public class XmlTextSerializer : ITextSerializer
    {
        public void Serialize(TextWriter textWriter, object o)
        {
            if (o == null)
                textWriter.WriteLine("(null)");

            string s = o as string;
            if (s == null)
            {
                IEnumerable seq = o as IEnumerable;
                if (seq != null)
                {
                    o = seq.Cast<object>().ToList();
                }
            }
            XmlSerializer xs = new XmlSerializer(o.GetType());
            xs.Serialize(textWriter, o);
        }
    }
}
