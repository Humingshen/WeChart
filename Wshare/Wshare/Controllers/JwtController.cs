using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class JwtController : Controller
    {


        // GET: Jwt
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 创建jwtToken
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public ActionResult CreateToken(string username, string pwd)
        {
            //假设用户名为"admin"，密码为"123"  
            if (username == "admin" && pwd == "123")
            {
                var payload = new Dictionary<string, object>
                {
                    { "username",username },
                    { "pwd", pwd }
                };
                return Json(new { Token = JwtUnit.SetJwtEncode(payload), Success = true, Message = "成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Token = "", Success = false, Message= "生成token失败" }, JsonRequestBehavior.AllowGet);
            }

            //get请求需要修改成这样
            //return Json(result);
            //return Json(result,JsonRequestBehavior.AllowGet);
        }
    }

}