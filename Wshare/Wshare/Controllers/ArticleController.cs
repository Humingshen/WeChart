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

        public ActionResult Info(int id)
        {
            T_Article obj = db.T_Article.Find(id);
            ViewBag.Article = obj ?? new T_Article();

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
        [CheckLogin]
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
