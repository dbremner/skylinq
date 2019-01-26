using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LINQPadHost
{
    public sealed class Host
    {
        public void Run<T>(string file, TextWriter tw) where T : ITextSerializer, new()
        {
            CompilerResults cr = CompileLinqFile(file);
            Run<T>(cr, tw);
        }

        public void Run<T>(CompilerResults cr, TextWriter tw) where T : ITextSerializer, new()
        {
            byte[] assembly = File.ReadAllBytes(cr.PathToAssembly);
            Run<T>(assembly, tw);
            File.Delete(cr.PathToAssembly);
        }

        public void Run<T>(StreamReader sr, TextWriter tw) where T : ITextSerializer, new()
        {
            CompilerResults cr = CompileLinqFile(sr);
            Run<T>(cr, tw);
        }

        public void Run<T>(byte[] assembly, TextWriter tw) where T : ITextSerializer, new()
        {
            Type helperType = typeof(AppDomainHelper);
            string appBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            var setup = new AppDomainSetup { ApplicationBase = appBasePath };
            //var permissions = new PermissionSet(PermissionState.None);
            //permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            //permissions.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
            //permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));
            AppDomain domain = AppDomain.CreateDomain("New domain", AppDomain.CurrentDomain.Evidence,
                setup/*, permissions*/);
            AppDomainHelper helper = (AppDomainHelper)domain.CreateInstanceAndUnwrap(helperType.Assembly.FullName, helperType.FullName);
            ILinqpadQuery compiledQuery = helper.CreateQuery(assembly);
            compiledQuery.InitSerializer<JsonTextSerializer>();
            compiledQuery.Out = tw;
            compiledQuery.Run();
            AppDomain.Unload(domain);
        }

        public CompilerResults CompileLinqFile(string file)
        {
            Query query = ParseLinqFile(file);

            return CompileLinqQuery(query);
        }

        public CompilerResults CompileLinqFile(StreamReader sr)
        {
            Query query = ParseLinqFile(sr);

            return CompileLinqQuery(query);
        }

        public Query ParseLinqFile(string file)
        {
            StreamReader sr = File.OpenText(file);
            return ParseLinqFile(sr);
        }

        public Query ParseLinqFile(StreamReader sr)
        {
            StringBuilder sb = new StringBuilder();
            string line;
            while (!string.IsNullOrEmpty(line = sr.ReadLine()))
            {
                sb.AppendLine(line);
            };

            XmlSerializer serializer = new XmlSerializer(typeof(Query));
            Query query = (Query)serializer.Deserialize(new StringReader(sb.ToString()));
            //Console.WriteLine("Kind: " + query.Kind);

            sb = new StringBuilder();
            while (!sr.EndOfStream)
            {
                sb.AppendLine(sr.ReadLine());
            }
            query.Code = sb.ToString();
            return query;
        }

        public  CompilerResults CompileLinqQuery(Query query)
        {
            StringBuilder sbNamespaces = new StringBuilder();
            if (query.Namespace != null)
            {
                foreach (var ns in query.Namespace)
                {
                    //Console.WriteLine("Namespace: " + ns);
                    sbNamespaces.AppendLine("using " + ns + ";");
                }
            }

            //string source = File.ReadAllText("Template.cs");
            string template = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LINQPadHost.Template.cs")).ReadToEnd();
            template = template.Replace("//{Namespaces}//", sbNamespaces.ToString());

            //foreach (var r in query.Reference)
            //{
            //    Console.WriteLine("Reference: " + r);
            //}

            if ("Expression".Equals(query.Kind))
            {
                template = template.Replace("//{Expression}//", "(" + query.Code + ")");
            }
            else
            {
                template = template.Replace("//{Statements}//", query.Code);
            }

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
                    Assembly.GetExecutingAssembly().Location
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
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, template);
            //if (cr.Errors.Count > 0)
            //{
            //    // Display compilation errors.
            //    Console.WriteLine("Errors building");
            //    foreach (CompilerError ce in cr.Errors)
            //    {
            //        Console.WriteLine("  {0}", ce.ToString());
            //        Console.WriteLine();
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("successfully build.");
            //}
            return cr;
        }
    }
}
