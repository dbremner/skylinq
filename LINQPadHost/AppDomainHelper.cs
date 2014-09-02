using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace LINQPadHost
{
    public class AppDomainHelper : MarshalByRefObject
    {
        public ILinqpadQuery CreateQuery(string assembly)
        {
            Assembly asm = Assembly.LoadFrom(assembly);
            return (ILinqpadQuery)asm.CreateInstance("LINQPadHost.Template");
        }
    }
}
