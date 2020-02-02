using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Wshare.Models;

namespace Wshare
{
    public class JsonFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string authHeader= HttpContext.Current.Request.Headers["Authorization"];
            if (authHeader == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }
            var userinfo = JwtUnit.GetJwtDecode(authHeader);

           // 举个例子 生成jwtToken 存入redis中
           //这个地方用jwtToken当作key 获取实体val 然后看看jwtToken根据redis是否一样
            if (userinfo == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }
        }
    }
}