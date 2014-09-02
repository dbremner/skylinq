using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
//using LINQPad;
using LINQPadHost;
//{Namespaces}//

namespace LINQPadHost
{
    public class Template : MarshalByRefObject, ILinqpadQuery
    {
        public void Run()
        {
            //{Expression}//.Dump();
            //{Statements}//
        }

        public void InitSerializer<T>() where T : ITextSerializer, new()
        {
            DumpHelper._serializer = new T();
        }

        public TextWriter Out 
        { 
            set
            {
                DumpHelper._output = value;
            }
        }
    }

    public static class DumpHelper
    {
        public static TextWriter _output;
        public static ITextSerializer _serializer;

        public static void Dump(this object o)
        {
            _serializer.Serialize(_output, o);
        }
    }
}
