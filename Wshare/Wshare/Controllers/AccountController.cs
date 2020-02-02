using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wshare.Controllers.DTOs;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class AccountController : Controller
    {
        wshareEntities db = new wshareEntities();

        // GET: Account
        [WeChat]
        public ActionResult Index()
        {
            var us = db.T_User.Find(Lib.UserId);
            var sa = from u in db.T_Visitors
                     where u.ShareId == us.Id
                     group u by  u.ArticleId into t
                     select new KV{ ID=  t.Key , Count = t.Count() };
            var result = from a in sa
                         join b in db.T_Article on a.ID equals b.Id
                         select new ResArt() { Id = a.ID, Title = b.Title, Cover = b.Cover, Count = a.Count };
            ViewBag.Articles = result.OrderBy(o=>o.Count).ToList();
            return View();
        }

        class KV
        {
            public int ID { get; set; }
            public int Count { get; set; }
        }
    }
}