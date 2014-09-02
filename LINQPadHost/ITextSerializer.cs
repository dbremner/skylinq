using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LINQPadHost
{
    public interface ITextSerializer
    {
        void Serialize(TextWriter textWriter, Object o);
    }
}
