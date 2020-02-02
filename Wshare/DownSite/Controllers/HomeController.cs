using DownSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DownSite.Controllers
{
    public class HomeController : Controller
    {

        wshareEntities1 db = new wshareEntities1();

        public ActionResult Index(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                var slist = db.T_Files.Where(i => i.FileName.Contains(q)).ToList();
                ViewBag.sFiles = slist;
            }
            var list = db.T_Files.ToList();
            ViewBag.Files = list;
            return View();
        }

        public JsonResult LoadFile()
        {
            DirectoryInfo root = new DirectoryInfo(Server.MapPath("~/File/"));
            FileInfo[] files = root.GetFiles();
            int row = 0;
            foreach (var file in files)
            {
                if (!db.T_Files.Any(o => o.FileName == file.Name))
                {
                    db.T_Files.Add(new T_Files()
                    {
                        Created = DateTime.Now,
                        Download = 0,
                        Extension = file.Extension.Replace(".",""),
                        FileName = file.Name,
                        Title = file.Name.Replace(file.Extension, "")
                    });
                    row += 1;
                }
            }
            if (row > 0)
                db.SaveChanges();
            return new JsonResult() { Data = new { msg = "", code = 0, row = row }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult Down(string code)
        {
            var auth = db.T_Authorize.Where(o => o.Code == code && o.State == 1).FirstOrDefault();
            if (auth != null)
            {
                if (DateTime.Now.Month > 3)
                {
                    return new JsonResult() { Data = new { msg = "", code = 0 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    if (auth.Tag == 1)
                    {
                        return new JsonResult() { Data = new { msg = "", code = 0 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        return new JsonResult() { Data = new { msg = "打赏用户优先下载", code = 1 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
            }
            return new JsonResult() { Data = new { msg = "无效参数，下载码不可用！", code = 1 }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileStreamResult Download(string code,int id)
        {
            var auth = db.T_Authorize.Where(o => o.Code == code && o.State == 1).FirstOrDefault();
            if (auth != null)
            {
                auth.Updated = DateTime.Now;
                auth.FileId = id;
                auth.State = 2;
                db.SaveChanges();

                T_Files file = db.T_Files.Find(id);
                string fileName = file.FileName;//客户端保存的文件名
                string filePath = Server.MapPath("~/File/"+ file.FileName);//路径
                return File(new FileStream(filePath, FileMode.Open), FileType(file.Extension),
                fileName);
            }
            return null;
        }

        private string FileType(string fileType)
        {
            //获取文件格式
            switch (fileType)
            {
                case "doc":
                    return "application/msword";
                case "xls":
                    return "application/msexcel";
                case "jpg":
                    return "image/jpg";
                case "png":
                    return "image/png";
                case "txt":
                    return "text/plain";
                default:
                    return "application/" + fileType;
            }
        }
    }
}