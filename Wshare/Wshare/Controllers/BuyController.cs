using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wshare.Controllers
{
    public class BuyController : Controller
    {
        // GET: Buy
        [WeChat]
        public ActionResult Index()
        {
            return View();
        }
    }
}