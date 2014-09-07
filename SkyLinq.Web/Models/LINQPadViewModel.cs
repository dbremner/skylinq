using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SkyLinq.Web.Models
{
    public enum KindEnum
    {
        Expression,
        Statements
    }


    public class LINQPadViewModel
    {
        public KindEnum Kind { get; set; }
        [DataType(DataType.MultilineText)]
        public string References { get; set; }
        [DataType(DataType.MultilineText)]
        public string Namespaces { get; set; }
        [DataType(DataType.MultilineText)]
        public string Code { get; set; }
        public string Results { get; set; }
    }
}