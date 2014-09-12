using LINQPadHost;
using SkyLinq.Web.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkyLinq.Web.Controllers
{
    public class SkyLINQPadController : Controller
    {
        // GET: SkyLINGPad
        public ActionResult Index()
        {
            LINQPadViewModel model = new LINQPadViewModel()
            {
                Kind = KindEnum.Expression,
                Code = "",
                Results = "Please click run to see results."
            };
            return View(model);
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult Index(LINQPadViewModel model, HttpPostedFileBase fileUpload)
        {
            Host host = new Host();
            if (model.References == null)
            {
                model.References = string.Empty;
            }
            if (model.Namespaces == null)
            {
                model.Namespaces = string.Empty;
            }

            Query query;
            if (fileUpload != null)
            {
                query = host.ParseLinqFile(new StreamReader(fileUpload.InputStream));
                ModelState.Clear();
                if (query.Reference != null)
                    model.References = string.Join(Environment.NewLine, query.Reference);
                if (query.Namespace != null)
                    model.Namespaces = string.Join(Environment.NewLine, query.Namespace);
                model.Kind = (KindEnum)Enum.Parse(typeof(KindEnum), query.Kind);
                model.Code = query.Code;
            }
            else
            {
                query = new Query()
                {
                    Kind = model.Kind.ToString(),
                    Reference = model.References.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
                    Namespace = model.Namespaces.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
                    Code = model.Code
                };
            }

            CompilerResults cr = host.CompileLinqQuery(query);
            if (cr.Errors != null && cr.Errors.Count > 0)
            {
                foreach(CompilerError ce in cr.Errors)
                {
                    model.Results += ce + "\r\n";    
                }
            }
            else
            { 
                StringWriter sw = new StringWriter();
                host.Run<IndentTextSerializer>(cr, sw);
                model.Results = sw.ToString();
            }
            return View(model);
        }
    }
}