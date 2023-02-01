using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers.Dashboards
{
    public class ClientFeedbackController : Controller
    {
        public ActionResult FeedbackTrends()
        {
            return View();
        }

        public ActionResult LastFeedback()
        {
            return View();
        }
    }
}