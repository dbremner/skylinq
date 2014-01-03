using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SkyLinq.Web.Models;

namespace SkyLinq.Web.Controllers
{
    public class SkyLogController : Controller
    {
        //
        // GET: /LogRpt/
        public ActionResult Index()
        {
            LogModel model = new LogModel();
            return View(model);
        }
	}
}