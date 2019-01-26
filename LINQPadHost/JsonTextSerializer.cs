using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQPadHost
{
    public sealed class JsonTextSerializer : ITextSerializer
    {
        public void Serialize(System.IO.TextWriter textWriter, object o)
        {
            if (o == null)
            {
                textWriter.WriteLine("(null)");
            }
            else
            {
                JsonSerializer js = new JsonSerializer();
                js.Formatting = Formatting.Indented;
                js.Serialize(textWriter, o);
            }
        }
    }
}
