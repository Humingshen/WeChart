using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Wshare.Models
{
    public class WeiXinCommon
    {
        /// <summary>
        /// 同步对象，保证对日志文件的同步性
        /// </summary>
        private static object _Syn = new object();
        /// <summary>
        /// 公众号APPID
        /// </summary>
        public static readonly string _AppId = "wx3b571edd4c6f73fd";
        /// <summary>
        /// 开发者密码
        /// </summary>
        public static readonly string _AppSecret = "95dcc9f577c1362c08e4bd9e0d9d3183";
        /// <summary>
        /// 支付商户ID
        /// </summary>
        public static readonly string _MerIdPay = "1487052762";
        /// <summary>
        /// 支付签名API密钥
        /// </summary>
        public static readonly string _Key = "95dcc9f577c1362c08e4bd9e0d9d3183";

        public static readonly string Url = "http://edu.lunwenba.com";

        /// <summary>
        /// 字典转换成JSON字符串 JSAPI支付时用到
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        public static string ToJson(Dictionary<String, string> D)
        {
            string jsonStr = JsonMapper.ToJson(D);
            return jsonStr;

        }
        /// <summary>
        /// 字典转换成JSON字符串 JSAPI支付时用到
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        public static string ToJson(Dictionary<String, object> D)
        {
            string jsonStr = JsonMapper.ToJson(D);
            return jsonStr;

        }

        /// <summary>
        /// 获取统一下单接口的 timeStamp 值
        /// </summary>
        /// <returns></returns>
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 把json字符串转成对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="data">json字符串</param>
        public static T Deserialize<T>(string data)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Deserialize<T>(data);
        }

        /// <summary>
        /// 写日志用于调试阶段
        /// </summary>
        /// <param name="logStr"></param>
        /// <returns></returns>
        public static bool WriteErrorLog(string logStr)
        {
            lock (_Syn)
            {
                string logDir = "C:\\RunLog\\";
                if (!System.IO.Directory.Exists(logDir))
                {
                    System.IO.Directory.CreateDirectory(logDir);
                }
                string strFileName = logDir + DateTime.Now.Date.ToShortDateString().Replace("/", "_") + ".ini";
                try
                {
                    if (!System.IO.File.Exists(strFileName))
                    {
                        System.IO.File.Create(strFileName).Close();
                    }
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileName, true, Encoding.Default);
                    sw.WriteLine(logStr);
                    sw.Flush();
                    sw.Close();
                    return true;
                }
                catch (System.Exception err)
                {
                    System.Diagnostics.Trace.WriteLine(err);
                }
                return false;
            }
        }
        /// <summary>
        /// 发送 Http请求  (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <param name="anchor">描点</param>
        /// <returns>响应内容</returns>
        public static string SendRequest(string url, IDictionary<string, string> parameters, string method, string anchor = "", string type = "application/x-www-form-urlencoded")
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 15000;
                    req.ContentType = string.Format("{0};charset=utf-8", type);
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                try
                {
                    //创建请求
                    string newUrl = url + "?" + BuildQuery(parameters, "utf8") + anchor;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newUrl);
                    WriteErrorLog(newUrl);
                    //GET请求
                    request.Method = "GET";
                    request.ReadWriteTimeout = 15000;
                    request.ContentType = "text/html;charset=UTF-8";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("UTF-8"));

                    //返回内容
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }


        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// 调用微信获取Code接口并返回跳转链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public static string GetUrl(string url, Dictionary<string, string> parameters, string anchor)
        {
            try
            {
                //创建请求
                string newUrl = url + "?" + BuildQuery(parameters, "utf8") + anchor;
                return newUrl;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        public static string sendPost(string url, string parameters)
        {
            ServicePointManager.DefaultConnectionLimit = 200;
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            System.IO.Stream reqStream = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version10;
                req.Timeout = 15000;
                req.ContentType = string.Format("text/xml");
                byte[] postData = Encoding.UTF8.GetBytes(parameters);
                reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                rsp = (HttpWebResponse)req.GetResponse();
                Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                return GetResponseAsString(rsp, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (reqStream != null) reqStream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        public static string sendPost(string url, IDictionary<string, string> parameters, string method)
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 15000;
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                //创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));

                //GET请求
                request.Method = "GET";
                request.ReadWriteTimeout = 15000;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

                //返回内容
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))//&& !string.IsNullOrEmpty(value)
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (encode == "gb2312")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    }
                    else if (encode == "utf8")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    }
                    else
                    {
                        postData.Append(value);
                    }
                    hasParam = true;
                }
            }
            return postData.ToString();
        }

        /// <summary>
        /// 请求参数转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DictionaryToXml(SortedList<string, string> data)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<xml>");
            foreach (var item in data.Keys)
            {
                if (!string.IsNullOrWhiteSpace(item) && !string.IsNullOrWhiteSpace(data[item]))
                {
                    result.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", item, data[item]);
                }
            }
            result.Append("</xml>");
            return result.ToString();
        }


        /// <summary>
        /// 获取sign
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string Sign(IDictionary<string, string> dict, string key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                    continue;
                sb.AppendFormat("{0}={1}&", item.Key, item.Value);
            }
            sb.Append("key=" + key);
            var bytesToHash = Encoding.UTF8.GetBytes(sb.ToString()); //注意，必须是UTF-8。
            var hashResult = ComputeMD5Hash(bytesToHash);
            var hash = BytesToString(hashResult, true);
            return hash;
        }

        /// <summary>
        /// 字节转换成字符串类型
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] input, bool lowercase = true)
        {
            if (input == null || input.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(input.Length * 2);
            for (var i = 0; i < input.Length; i++)
            {
                sb.AppendFormat(lowercase ? "{0:x2}" : "{0:X2}", input[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 字节转换为md5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static byte[] ComputeMD5Hash(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var md5 = new MD5CryptoServiceProvider())
            {
                var result = md5.ComputeHash(input);
                return result;
            }
        }
    }

    /// <summary>
    /// 微信accessToken
    /// </summary>
    public class ResponseAccessTokenModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }

    /// <summary>
    ///微信返回用户信息
    /// </summary>
    public class ResponseUserInfoModel
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string[] privilege { get; set; }
        public string language { get; set; }
    }


    /// <summary>
    /// 统一下单接口返回支付信息
    /// </summary>
    [Serializable, XmlRoot("xml"), XmlType("xml")]
    public class ResponseUnifiedorder
    {
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        //以下字段在return_code和result_code都为SUCCESS的时候有返回
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        ///签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 用户错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string prepay_id { get; set; }
        /// <summary>
        /// trade_type为NATIVE时有返回，用于生成二维码，展示给用户进行扫码支付
        /// </summary>
        public string code_url { get; set; }
    }

    /// <summary>
    /// 返回支付结果
    /// </summary>
    [Serializable, XmlRoot("xml"), XmlType("xml")]
    public class ResponseOrderQuery
    {
        /// <summary>
        /// SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        //以下字段在return_code和result_code都为SUCCESS的时候有返回
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        ///签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 用户错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 用户在商户appid下的唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        public string is_subscribe { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// SUCCESS—支付成功 REFUND—转入退款 NOTPAY—未支付 CLOSED—已关闭 REVOKED—已撤销（刷卡支付） USERPAYING--用户支付中 PAYERROR--支付失败(其他原因，如银行返回失败)
        /// </summary>
        public string trade_state { get; set; }
        /// <summary>
        /// 付款银行类型
        /// </summary>
        public string bank_type { get; set; }
        /// <summary>
        /// 标价金额
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额。
        /// </summary>
        public string settlement_total_fee { get; set; }
        /// <summary>
        /// 标价币种
        /// </summary>
        public string fee_type { get; set; }
        /// <summary>
        /// 现金支付金额
        /// </summary>
        public string cash_fee { get; set; }
        /// <summary>
        /// 现金支付币种类型
        /// </summary>
        public string cash_fee_type { get; set; }
        /// <summary>
        /// 代金券金额
        /// </summary>
        public string coupon_fee { get; set; }
        /// <summary>
        /// 代金券使用数量
        /// </summary>
        public string coupon_count { get; set; }
        /// <summary>
        /// 代金券类型
        /// </summary>
        //public int coupon_type_$n { get; set; }
        /// <summary>
        /// 单个代金券支付金额 单个代金券支付金额, $n为下标，从0开始编号
        /// </summary>
        //public int coupon_fee_$n { get; set; }
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string time_end { get; set; }
        /// <summary>
        /// 交易状态描述
        /// </summary>
        public string trade_state_desc { get; set; }
    }


    /// <summary>
    /// xml帮助类
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            string result = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    XmlSerializer xml = new XmlSerializer(type);
                    xml.Serialize(sw, obj);
                    result = sb.ToString();
                }
            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}