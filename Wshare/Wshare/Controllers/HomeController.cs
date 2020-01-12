using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class HomeController : Controller
    {
        wshareEntities db = new wshareEntities();
        public ActionResult Index()
        {
            List<string> tags = (from a in db.T_Article
                                 where a.State == 1
                                 group a by a.Tags
                                 into t
                                 select t.Key
                                 ).ToList();

            List<T_Article> list = (from a in db.T_Article
                                    where a.State == 1
                                    select a).ToList();

            ViewBag.Articles = list ?? new List<T_Article>();
            ViewBag.Tags = tags ?? new List<string>();

            return View();
        }
    }
}