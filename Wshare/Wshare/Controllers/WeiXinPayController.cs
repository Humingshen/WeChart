using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class WeiXinPayController : Controller
    {
        wshareEntities db = new wshareEntities();
        /// <summary>
        /// 生成订单页面
        /// </summary>
        /// <returns></returns>
        [WeChat]
        public ActionResult Index(int id,int amount)
        {
            

            return View();
        }

        /// <summary>
        /// 支付方法
        /// </summary>
        /// <param name="OrderID">本地订单ID</param>
        /// <param name="TotalFee">费用 单位 分</param>
        /// <returns></returns>
        [HttpPost]

        public JsonResult Pay(String OrderID, int TotalFee)
        {
            JsonResult js = new JsonResult();
            #region 创建订单
            #region 1。调用统一支付接口形成预订单

            WeiXinCommon.WriteErrorLog("调用统一支付接口形成预订单");
            ResponseUnifiedorder WxOrder = XmlHelper.Deserialize(typeof(ResponseUnifiedorder), CreateOrder((TotalFee).ToString(), OrderID)) as ResponseUnifiedorder;
            //添加数据库方法 将微信返回的预订单号和本地订单关联起来
            #endregion
            #region 2。生成订单及JSAPI提交参数
            WeiXinCommon.WriteErrorLog("生成订单及JSAPI提交参数");
            if (null != WxOrder)
            {
                string param = GetJsApiParameters(WxOrder);
                WeiXinCommon.WriteErrorLog("GetJsApiParameters:" + param);
                js.Data = new { ret = -1, msg = "创建微信订单成功！", data = param };
            }
            else
            {
                js.Data = new { ret = 3, msg = "创建微信订单失败" };
            }
            #endregion
            #endregion
            return js;
        }

        /// <summary>
        /// 获取用户的真正IP
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            string result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "0.0.0.0";
            }
            return result;

        }

        /// <summary>
        /// 调用统一下单接口生成微信预支付订单
        /// </summary>
        /// <param name="money"></param>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        public string CreateOrder(string money, string tradeNo)
        {
            string clientIP = GetIP();
            string WXOpenID = "";
            string data = string.Empty;
            try
            {
                ///接口参数
                SortedList<string, string> dic = new SortedList<string, string>();
                //签名参数
                SortedList<string, string> dicSign = new SortedList<string, string>();
                string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
                string productId = DateTime.Now.Ticks.ToString();
                dicSign.Add("appid", WeiXinCommon._AppId);
                dicSign.Add("mch_id", WeiXinCommon._MerIdPay);

                dicSign.Add("nonce_str", nonceStr);
                dicSign.Add("body", "赞赏作者");
                dicSign.Add("out_trade_no", tradeNo);
                dicSign.Add("total_fee", money);
                dicSign.Add("spbill_create_ip", clientIP);
                dicSign.Add("notify_url", WeiXinCommon.Url + "/WeiXinPay/Notify");
                dicSign.Add("trade_type", "JSAPI");
                dicSign.Add("product_id", productId);
                dicSign.Add("openid", WXOpenID);
                dic.Add("appid", WeiXinCommon._AppId);
                dic.Add("mch_id", WeiXinCommon._MerIdPay);

                dic.Add("nonce_str", nonceStr);
                dic.Add("sign", WeiXinCommon.Sign(dicSign, WeiXinCommon._Key));
                dic.Add("body", "赞赏作者");
                dic.Add("out_trade_no", tradeNo);
                dic.Add("total_fee", money);
                dic.Add("spbill_create_ip", clientIP);
                dic.Add("notify_url", WeiXinCommon.Url + "/My/Notify");
                dic.Add("trade_type", "JSAPI");
                dic.Add("product_id", productId);
                dic.Add("openid", WXOpenID);
                data = WeiXinCommon.sendPost("https://api.mch.weixin.qq.com/pay/unifiedorder", WeiXinCommon.DictionaryToXml(dic));
                WeiXinCommon.WriteErrorLog("CreateOrder:" + data);
            }
            catch (Exception ex)
            {
                WeiXinCommon.WriteErrorLog("CreateOrder" + ex.Message);
            }
            return data;
        }

        /// <summary>
        /// 生成JSAPI参数
        /// </summary>
        /// <returns></returns>
        public string GetJsApiParameters(ResponseUnifiedorder unifiedOrderResult)
        {
            string nonceStr = Guid.NewGuid().ToString().Replace("-", "");
            string TimeStr = WeiXinCommon.GenerateTimeStamp();
            //参数字典
            Dictionary<string, string> dic = new Dictionary<string, string>();
            ///签名参数字典
            SortedList<string, string> dicSign = new SortedList<string, string>();
            dicSign.Add("appId", unifiedOrderResult.appid);
            dicSign.Add("timeStamp", TimeStr);
            dicSign.Add("nonceStr", nonceStr);
            dicSign.Add("package", "prepay_id=" + unifiedOrderResult.prepay_id);
            dicSign.Add("signType", "MD5");

            dic.Add("appId", unifiedOrderResult.appid);
            dic.Add("timeStamp", TimeStr);
            dic.Add("nonceStr", nonceStr);
            dic.Add("package", "prepay_id=" + unifiedOrderResult.prepay_id);
            dic.Add("signType", "MD5");
            ///JSAPI方式不需要openID
            // dic.Add("openid", currUser.WXOpenID);
            dic.Add("paySign", WeiXinCommon.Sign(dicSign, WeiXinCommon._Key));

            string parameters = WeiXinCommon.ToJson(dic);
            WeiXinCommon.WriteErrorLog("GetJsApiParameters:" + parameters);

            return parameters;
        }

        /// <summary>
        /// 支付回调函数
        /// </summary>
        public void Notify()
        {
            try
            {
                //接收从微信后台POST过来的数据
                WeiXinCommon.WriteErrorLog("微信支付返回回调！Notify");
                Stream s = Request.InputStream;
                byte[] buffer = new byte[1024];
                StringBuilder sb = new StringBuilder();
                int count = 0;
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                WeiXinCommon.WriteErrorLog("微信支付返回：" + sb.ToString());
                ResponseOrderQuery query = XmlHelper.Deserialize(typeof(ResponseOrderQuery), sb.ToString()) as ResponseOrderQuery;
                //检查支付结果中transaction_id是否存在
                if (string.IsNullOrWhiteSpace(query.transaction_id))
                {
                    //若transaction_id不存在，则立即返回结果给微信支付后台
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "FAIL");
                    res.Add("return_msg", "支付结果中微信订单号不存在");
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
                if (query.return_code == "SUCCESS" && query.result_code == "SUCCESS")
                {
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "SUCCESS");
                    res.Add("return_msg", "OK");
                    string msg = "";
                    //数据库操作根据 订单号去除数据库订单 判断状态 置订单状态值
                    var pay = db.T_Pay.Find(query.out_trade_no);
                    if (!pay.Status)
                    {
                        pay.Status = true;
                        db.SaveChanges();
                    }
                    //Dal.OrderDal orderdal = new Dal.OrderDal();
                    //Modal.View_OrderInfo orderinfo = orderdal.getOrderViewByOrderID(query.out_trade_no);
                    //if (orderinfo.Status == (int)SysEnum.OrderStatus.待支付)
                    //{
                    //    WeiXinWriteLog("数据库操作：订单号" + query.out_trade_no, "Notify", "");
                    //    msg = "操纵成功！";
                    //    orderdal.PaySuccess(orderinfo.OrderID);
                    //    WeiXinWriteLog(WeiXinDictionaryToXml(res), "Notify", msg);
                    //    
                    //}
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
                else
                {
                    //若订单查询失败，则立即返回结果给微信支付后台
                    SortedList<string, string> res = new SortedList<string, string>();
                    res.Add("return_code", "FAIL");
                    res.Add("return_msg", "查询订单失败！");
                    Response.Write(WeiXinCommon.DictionaryToXml(res));
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WeiXinCommon.WriteErrorLog("出现异常！");
                SortedList<string, string> res = new SortedList<string, string>();
                res.Add("return_code", "FAIL");
                res.Add("return_msg", "出现异常");
                Response.Write(WeiXinCommon.DictionaryToXml(res));
                Response.End();
            }

        }
    }
}