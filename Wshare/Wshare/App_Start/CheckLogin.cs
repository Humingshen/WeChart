using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare
{
    public class CheckLogin : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中
            var authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader == null)
            {
                httpContext.Response.StatusCode = 403;
                return false;
            }
            var userinfo = JwtUnit.GetJwtDecode(authHeader);

            //举个例子  生成jwtToken 存入redis中    
            //这个地方用jwtToken当作key 获取实体val   然后看看jwtToken根据redis是否一样
            if (userinfo.UserName == "admin" && userinfo.Pwd == "123")
                return true;

            httpContext.Response.StatusCode = 403;
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}