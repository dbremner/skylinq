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
    public sealed class AppDomainHelper : MarshalByRefObject
    {
        public ILinqpadQuery CreateQuery(byte[] assembly)
        {
            Assembly asm = Assembly.Load(assembly);
            return (ILinqpadQuery)asm.CreateInstance("LINQPadHost.Template");
        }
    }
}
