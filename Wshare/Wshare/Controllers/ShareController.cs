using QRCoder;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class ShareController : Controller
    {

        [WeChat]
        // GET: Share
        public ActionResult Index()
        {

            wshareEntities db = new wshareEntities();
            var user = db.T_User.Find(Lib.UserId);
            if (user.IsEnable)
            {
                ViewBag.Score = user.Score;
                ViewBag.Count = db.T_Share.Where(i => i.FromId == user.Id && i.IsBuy).Count();

                ViewBag.Image = GetPic(GetQRCode("http://edu.lunwenba.com/read/?id=" + Lib.UserId, 10));
                return View();
            }
            return Redirect("/Buy/");
        }

        private string GetPic(Bitmap bitmap)
        {
            //Bitmap
            Bitmap map = new Bitmap(Server.MapPath("\\Content\\bg.jpg"));
            //Graphics
            Graphics gp = Graphics.FromImage(map);
            Rectangle rtl = new Rectangle(345, 1340, bitmap.Width, bitmap.Height);
            gp.DrawImage(bitmap, rtl, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            gp.Dispose();
            return ToBase64(map);
        }


        public Bitmap GetQRCode(string plainText, int pixel)
        {
            var generator = new QRCodeGenerator();
            var qrCodeData = generator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCoder.QRCode(qrCodeData);

            var bitmap = qrCode.GetGraphic(pixel);
            return bitmap;
        }

        public string ToBase64(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.DrawingCore.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}