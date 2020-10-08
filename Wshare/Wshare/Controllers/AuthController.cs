using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class AuthController : Controller
    {
        public string AppId = "wx3b571edd4c6f73fd";
        public string AppSecret = "95dcc9f577c1362c08e4bd9e0d9d3183";

        wshareEntities _context = new wshareEntities();
        // GET: Auth
        public ActionResult Index()
        {
            //var us = _context.T_User.FirstOrDefault();

            //HttpCookie cookie1 = new HttpCookie("userid");
            //cookie1.Value = us.Id + "";

            //Response.AppendCookie(cookie1);

            //return Redirect(Request["url"]);
            if (!string.IsNullOrEmpty(Request["code"]))
            {
                try
                {
                    //WeiXinCommon.WriteErrorLog(Request["code"]);
                    //根据appid，secret，code取到用户的全部信息  
                    Dictionary<string, object> dic = GetUserInfoByCode(AppId, AppSecret, Request["code"].ToString());
                    if (dic.ContainsKey("errcode"))
                    {
                        return Redirect("/Auth/Error?Msg=" + dic["errmsg"].ToString());
                    }
                    string openid = dic["openid"] + "";
                    //根据微信唯一标识openid  去数据库判断是否存在  

                    //1.不存在就新增  
                    if (!_context.T_User.Any(o => o.Openid == openid))
                    {
                        var user = new T_User()
                        {
                            Headimgurl = dic["headimgurl"].ToString(),
                            CreateTime = DateTime.Now,
                            City = dic["city"].ToString(),
                            Country = dic["country"].ToString(),
                            Nickname = dic["nickname"].ToString(),
                            Openid = dic["openid"].ToString(),
                            Province = dic["province"].ToString(),
                            Sex = dic["sex"].ToString(),
                            Unionid = ""/// dic["unionid"].ToString(),
                            ,
                            IsEnable = false,
                            PayRemark = "",
                            Score = 0
                        };
                        _context.T_User.Add(user);
                        _context.SaveChanges();
                    }
                    var u = _context.T_User.Where(i => i.Openid == openid).FirstOrDefault();
                    HttpCookie cookie = new HttpCookie("uid");
                    cookie.Value = u.Id + "";
                    cookie.Expires = DateTime.Now.AddHours(24);
                    Response.AppendCookie(cookie);
                    return Redirect(Request["url"]);
                }
                catch (Exception ex)
                {
                    //WeiXinCommon.WriteErrorLog(ex.Message);
                    return Redirect("/Auth/Error?Msg=" + ex.Message);
                }
            }
            else
            {
                string redirect_uri = HttpUtility.UrlEncode("http://" + Request.Url.Authority + Request.Url.PathAndQuery);
                return Redirect(string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect&connect_redirect=1", AppId, redirect_uri, "snsapi_userinfo"));
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        /// <summary>  
        ///用code换取获取用户信息（包括非关注用户的）(此access_token是网页授权的和普通无关)  
        /// </summary>  
        /// <param name="Appid"></param>  
        /// <param name="Appsecret"></param>  
        /// <param name="Code">回调页面带的code参数</param>  
        /// <returns>获取用户信息（json格式）</returns>  
        public Dictionary<string, object> GetUserInfoByCode(string Appid, string Appsecret, string Code)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", Appid, Appsecret, Code);

            //WeiXinCommon.WriteErrorLog(url);
            string ReText = WebRequestPostOrGet(url, "");//post/get方法获取信息  

            //WeiXinCommon.WriteErrorLog(ReText);
            Dictionary<string, object> DicText = (Dictionary<string, object>)Jss.DeserializeObject(ReText);
            if (!DicText.ContainsKey("openid"))
            {
                return DicText;
            }
            else
            {
                //WeiXinCommon.WriteErrorLog(DicText["access_token"]+"");
                Dictionary<string, object> respDic = (Dictionary<string, object>)Jss.DeserializeObject(WebRequestPostOrGet("https://api.weixin.qq.com/sns/userinfo?access_token=" + DicText["access_token"] + "&openid=" + DicText["openid"] + "&lang=zh_CN", ""));

                //WeiXinCommon.WriteErrorLog(WeiXinCommon.ToJson(respDic));
                return respDic;
            }
        }

        #region Post/Get提交调用抓取  
        /// <summary>  
        /// Post/get 提交调用抓取  
        /// </summary>  
        /// <param name="url">提交地址</param>  
        /// <param name="param">参数</param>  
        /// <returns>string</returns>  
        public static string WebRequestPostOrGet(string sUrl, string sParam)
        {
            byte[] bt = System.Text.Encoding.UTF8.GetBytes(sParam);

            Uri uriurl = new Uri(sUrl);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uriurl);//HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url + (url.IndexOf("?") > -1 ? "" : "?") + param);  
            req.Method = "Post";
            req.Timeout = 120 * 1000;
            req.ContentType = "application/x-www-form-urlencoded;";
            req.ContentLength = bt.Length;

            using (Stream reqStream = req.GetRequestStream())//using 使用可以释放using段内的内存  
            {
                reqStream.Write(bt, 0, bt.Length);
                reqStream.Flush();
            }
            try
            {
                using (WebResponse res = req.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理   

                    Stream resStream = res.GetResponseStream();

                    StreamReader resStreamReader = new StreamReader(resStream, System.Text.Encoding.UTF8);

                    string resLine;

                    System.Text.StringBuilder resStringBuilder = new System.Text.StringBuilder();

                    while ((resLine = resStreamReader.ReadLine()) != null)
                    {
                        resStringBuilder.Append(resLine + System.Environment.NewLine);
                    }

                    resStream.Close();
                    resStreamReader.Close();

                    return resStringBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;//url错误时候回报错  
            }
        }
        #endregion Post/Get提交调用抓取  
    }
}