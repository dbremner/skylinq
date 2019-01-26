using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace LINQPadHost
{
    public sealed class IndentTextSerializer : ITextSerializer
    {
        public void Serialize(TextWriter textWriter, object o)
        {
            Serialize(textWriter, o, 0);
        }

        private void Serialize(TextWriter textWriter, object o, int indent)
        {
            if (o == null)
            {
                WriteLine(textWriter, "(null)", indent);
            }
            else
            {
                string s = o as string;
                if (s != null)
                {
                    WriteLine(textWriter, o, indent);
                }
                else
                {
                    if (o is IEnumerable seq)
                    {
                        foreach (object child in seq)
                        {
                            Serialize(textWriter, child, indent + 1);
                        }
                    }
                    else
                    {
                        WriteLine(textWriter, o, indent);
                    }
                }
            }
        }

        private void WriteLine(TextWriter textWriter, object o, int indent)
        {
            textWriter.WriteLine("{0}{1}", new String('\t', indent), string.Format("{0}", o));
        }
    }
}
