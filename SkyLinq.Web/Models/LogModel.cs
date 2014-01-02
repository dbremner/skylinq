using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SkyLinq.Linq;

namespace SkyLinq.Web.Models
{
    public class LogModel
    {
        public IEnumerable<string[]> GetReports()
        {
            return typeof(BuildInW3SVCLogReports).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.StartsWith("Get"))
                .Select(m => new string[] {
                    m.Name.Substring(3),
                    ((DisplayAttribute)m.GetCustomAttribute(typeof(DisplayAttribute))).Description
                });
        }
    }
}