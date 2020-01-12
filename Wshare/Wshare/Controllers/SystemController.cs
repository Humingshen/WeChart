using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class SystemController : Controller
    {
        // GET: Page
        public ActionResult User()
        {
            return View();
        }

        // GET: Page
        public ActionResult Login()
        {
            return View();
        }

        // GET: Page
        public ActionResult Role()
        {
            return View();
        }
        // GET: Page
        public ActionResult Article()
        {
            return View();
        }
        public ActionResult ArtEdit(int id)
        {
            ViewBag.Art = (new wshareEntities()).T_Article.Find(id);
            return View();
        }
        // GET: Page
        public ActionResult Comment()
        {
            return View();
        }
        // GET: Page
        public ActionResult Visitors()
        {
            return View();
        }
        // GET: Page
        public ActionResult Authorize()
        {
            return View();
        }
        // GET: Page
        public ActionResult File()
        {
            return View();
        }
        // GET: Page
        public ActionResult DownLoad()
        {
            return View();
        }
        // GET: Page
        public ActionResult Password()
        {
            return View();
        }
    }
}