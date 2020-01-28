using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Wshare.Models
{
    public class WXShareModel
    {
        public string appId { get; set; }
        public string nonceStr { get; set; }
        public long timestamp { get; set; }

        public string signature { get; set; }

        public string ticket { get; set; }
        public string url { get; set; }

        public void MakeSign()
        {
            var string1Builder = new StringBuilder();
            string1Builder.Append("jsapi_ticket=").Append(ticket).Append("&")
                         .Append("noncestr=").Append(nonceStr).Append("&")
                         .Append("timestamp=").Append(timestamp).Append("&")
                         .Append("url=").Append(url);
            var string1 = string1Builder.ToString();
            signature = Sha1(string1, Encoding.Default);

        }

        public string Sha1(string orgStr, Encoding encode)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = encode.GetBytes(orgStr);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            sha1.Dispose();
            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result;
        }

        /// <summary>
        /// 获取access_token
        /// </summary>
        public string GetAccessToken()
        {
            string strJson = HttpRequestUtil.RequestUrl(string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", WeiXinCommon._AppId, WeiXinCommon._AppSecret));
            WeiXinCommon.WriteErrorLog(strJson);
            var obj = JsonConvert.DeserializeObject<TokenResult>(strJson);
            return obj.access_token;
        }
        

        public jsapiTicketModel GetJsApiTicket(string accessToken)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", accessToken);
            string strJson = HttpRequestUtil.RequestUrl(url);
            WeiXinCommon.WriteErrorLog(strJson);
            return JsonConvert.DeserializeObject<jsapiTicketModel>(strJson);
        }
    }

    public class TokenResult{

        public string access_token { get; set; }
        public int expires_in { get; set; }
        public DateTime created { get; set; }
    }

    public class jsapiTicketModel
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }

        public string ticket { get; set; }

        public string expires_in { get; set; }
    }

    public class HttpRequestUtil
    {
        #region 请求Url，不发送数据
        /// <summary>
        /// 请求Url，不发送数据
        /// </summary>
        public static string RequestUrl(string url)
        {
            return RequestUrl(url, "POST");
        }
        #endregion

        #region 请求Url，不发送数据
        /// <summary>
        /// 请求Url，不发送数据
        /// </summary>
        public static string RequestUrl(string url, string method)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = method;
            request.ContentType = "text/html";
            request.Headers.Add("charset", "utf-8");

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream, Encoding.Default);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();
            return content;
        }
        #endregion

        #region 请求Url，发送json数据
        /// <summary>
        /// 请求Url，发送json数据
        /// </summary>
        public static string RequestUrlSendMsg(string url, string method, string JSONData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JSONData);
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = method;
            request.ContentType = "text/html";
            request.Headers.Add("charset", "utf-8");
            Stream reqstream = request.GetRequestStream();
            reqstream.Write(bytes, 0, bytes.Length);
            //声明一个HttpWebRequest请求  
            request.Timeout = 90000;
            //设置连接超时时间  
            request.Headers.Set("Pragma", "no-cache");
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream, Encoding.Default);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();
            return content;
        }
        #endregion
    }
}