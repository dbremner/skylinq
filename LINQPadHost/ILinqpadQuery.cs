using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LINQPadHost
{
    public interface ILinqpadQuery
    {
        void Run();

        void InitSerializer<T>() where T : ITextSerializer, new();

        TextWriter Out { set; }
    }
}
