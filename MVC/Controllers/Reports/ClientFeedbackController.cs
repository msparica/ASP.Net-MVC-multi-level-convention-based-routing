using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers.Reports
{
    public class ClientFeedbackController : Controller
    {
        public ActionResult ByClient()
        {
            return View();
        }

        public ActionResult ByProject()
        {
            return View();
        }
    }
}