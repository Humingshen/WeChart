using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class ArticleController : Controller
    {

        wshareEntities db = new wshareEntities();

        // GET: Article
        public ActionResult Detail(int id)
        {
            return View();
        }

        // 
        [WeChat]
        public ActionResult Pay(int id,int amount)
        {
            T_Pay pay = new T_Pay()
            {
                Created = DateTime.Now,
                ArticleId = id,
                Price = amount,
                Remark = "",
                Status = false,
                UserId = Lib.UserId,
            };
            db.T_Pay.Add(pay);
            db.SaveChanges();
            ViewBag.ArticleId = id;
            ViewBag.ID = pay.Id;
            ViewBag.Amount = amount;
            return View();
        }

        [WeChat]
        public ActionResult Info(int id,int? uid)
        {
            var token = db.AccessToken.Find(1);
            var ticket = db.AccessToken.Find(2);

            var model = new WXShareModel();
            model.appId = WeiXinCommon._AppId;
            model.nonceStr = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            model.timestamp = Convert.ToInt64(WeiXinCommon.GenerateTimeStamp());

            if (ticket.Created.AddSeconds(6200) < DateTime.Now)
            {
                if (token.Created.AddSeconds(6200) < DateTime.Now)
                {
                    token.access_token = model.GetAccessToken();
                    token.Created = DateTime.Now;
                }
                ticket.access_token = model.GetJsApiTicket(token.access_token).ticket;
                ticket.Created = DateTime.Now;
                db.SaveChanges();
            }
            
            model.ticket = ticket.access_token;
            model.url = WeiXinCommon.Url + Request.Url.PathAndQuery;
            model.MakeSign(); 
            //WeiXinCommon.WriteErrorLog( model.url);

            ViewBag.Model = model;

            T_Article obj = db.T_Article.Find(id);
            var us = db.T_User.Find(Lib.UserId);
            if (uid != null)
            {
                if (!db.T_Visitors.Any(o => o.ArticleId == id && o.UserId == us.Id && o.ShareId == uid))
                {
                    T_Visitors vs = new T_Visitors()
                    {
                        ArticleId = id,
                        Created = DateTime.Now,
                        ShareId = uid,
                        UserId = us.Id
                    };
                    db.T_Visitors.Add(vs);
                    obj.Visitors += 1;
                    db.SaveChanges();
                }
            }

            ViewBag.Headimgurl = us.Headimgurl;
            ViewBag.Article = obj ?? new T_Article();
            ViewBag.Like = db.T_Pay.Where(o => o.Status && o.ArticleId == id).Count();
            List<string> zs = db.T_Pay.Where(o => o.Status && o.ArticleId == id).Select(o => o.T_User.Headimgurl).ToList();
            ViewBag.User = zs ?? new List<string>();

            return View();
        }

        // POST: Article/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        // GET: Article/Edit/5
        public JsonResult Get(int id)
        {
            T_Article art;
            using (wshareEntities db = new wshareEntities())
            {
                   art = db.T_Article.SingleOrDefault(o => o.Id == id);
            }
            return new JsonResult() { Data = art, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: Article/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Article/Delete/5
        public  JsonResult Delete(int id)
        {
            return new JsonResult() { };
        }

        // POST: Article/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
