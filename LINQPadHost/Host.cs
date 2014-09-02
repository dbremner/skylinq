using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LINQPadHost
{
    public class Host
    {
        public void Run<T>(string file) where T : ITextSerializer, new()
        {
            CompilerResults cr = CompileLinqFile(file);
            Run<T>(cr);
            File.Delete(cr.PathToAssembly);
        }

        public void Run<T>(CompilerResults cr) where T : ITextSerializer, new()
        {
            AppDomain domain = AppDomain.CreateDomain("New domain");
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap("LINQPadHost", "LINQPadHost.AppDomainHelper");
            ILinqpadQuery compiledQuery = helper.CreateQuery(cr.PathToAssembly);
            compiledQuery.InitSerializer<JsonTextSerializer>();
            compiledQuery.Out = Console.Out;
            compiledQuery.Run();
            AppDomain.Unload(domain);
        }

        public CompilerResults CompileLinqFile(string file)
        {
            StreamReader sr = File.OpenText(file);
            StringBuilder sb = new StringBuilder();
            string line;
            while (!string.IsNullOrEmpty(line = sr.ReadLine()))
            {
                sb.AppendLine(line);
            };

            XmlSerializer serializer = new XmlSerializer(typeof(Query));
            Query query = (Query)serializer.Deserialize(new StringReader(sb.ToString()));
            Console.WriteLine("Kind: " + query.Kind);
            sb = new StringBuilder();
            if (query.Namespace != null)
            {
                foreach (var ns in query.Namespace)
                {
                    //Console.WriteLine("Namespace: " + ns);
                    sb.AppendLine("using " + ns + ";");
                }
            }

            //string source = File.ReadAllText("Template.cs");
            string source = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LINQPadHost.Template.cs")).ReadToEnd();
            source = source.Replace("//{Namespaces}//", sb.ToString());

            //foreach (var r in query.Reference)
            //{
            //    Console.WriteLine("Reference: " + r);
            //}

            sb = new StringBuilder();
            while (!sr.EndOfStream)
            {
                sb.AppendLine(sr.ReadLine());
            }

            string replacePart;
            if ("Expression".Equals(query.Kind))
            {
                replacePart = "//{Expression}//";
            }
            else
            {
                replacePart = "//{Statements}//";
            }
            source = source.Replace(replacePart, sb.ToString());

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            //Add Linq pad default assemblies
            string[] linqPadAssemblies = new string[] {
                    "System.dll",
	                "Microsoft.CSharp.dll",  
	                "System.Core.dll",
	                "System.Data.dll",
	                "System.Transactions.dll",
	                "System.Xml.dll",
	                "System.Xml.Linq.dll",
	                "System.Data.Linq.dll",
	                "System.Drawing.dll",
	                "System.Data.DataSetExtensions.dll",
                    "LINQPadHost.dll"
                };
            foreach (var linqPadAssembly in linqPadAssemblies)
            {
                cp.ReferencedAssemblies.Add(linqPadAssembly);
            }

            if (query.Reference != null)
            {
                foreach (var r in query.Reference)
                {
                    var rf = Regex.Replace(r, @"<\w+>", m =>
                    {
                        switch (m.Value)
                        {
                            case "<RuntimeDirectory>":
                                return RuntimeEnvironment.GetRuntimeDirectory();
                            case "<ProgramFilesX86>":
                                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                            default:
                                return string.Empty;
                        }
                    });
                    cp.ReferencedAssemblies.Add(rf);
                }
            }
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, source);
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building");
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("successfully build.");
            }
            return cr;
        }
    }
}
