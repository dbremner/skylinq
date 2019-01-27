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
            switch (o)
            {
                case null:
                    WriteLine(textWriter, "(null)", indent);
                    break;
                case string s:
                    WriteLine(textWriter, o, indent);
                    break;
                case IEnumerable seq:
                {
                    foreach (object child in seq)
                    {
                        Serialize(textWriter, child, indent + 1);
                    }

                    break;
                }
                default:
                    WriteLine(textWriter, o, indent);
                    break;
            }
        }

        private void WriteLine(TextWriter textWriter, object o, int indent)
        {
            textWriter.WriteLine("{0}{1}", new String('\t', indent), string.Format("{0}", o));
        }
    }
}
