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

        [WeChat]
        public ActionResult Index(string k)
        {
            List<T_Article> list = (from a in db.T_Article
                                    where a.State == 1 && (a.Tags == k || string.IsNullOrEmpty(k))
                                    orderby a.Id descending
                                    select a).ToList();

            ViewBag.Articles = list ?? new List<T_Article>();
            ViewBag.Tags =  new List<string>();

            return View();
        }
    }
}