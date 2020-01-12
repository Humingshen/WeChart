using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Wshare.Controllers.DTOs;

namespace Wshare.Controllers
{
    public class FileController : ApiController
    {

        [HttpPost]
        [Route("api/upload")]
        public JsonResult<FileResponse> UploadFile()
        {
            List<string> files = new List<string>();
            //获取参数信息
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;      //定义传统request对象
            string id = request["id"];       //获取请求参数：

            string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/");
            //var dirTempPath = HttpContext.Current.Server.MapPath(saveTempPath);
            HttpFileCollection filelist = HttpContext.Current.Request.Files;
            if (filelist != null && filelist.Count > 0)
            {
                for (int i = 0; i < filelist.Count; i++)
                {
                    HttpPostedFile file = filelist[i];
                    String Tpath = DateTime.Now.ToString("yyyy-MM-dd");
                    string filename = file.FileName;
                    //获取上传文件后缀名
                    string fileExt = filename.Substring(filename.LastIndexOf('.'));
                    string FileName = Guid.NewGuid().ToString() + fileExt;
                    string FilePath = "Content/upload/" + id + "/";
                    DirectoryInfo di = new DirectoryInfo(path+FilePath);
                    if (!di.Exists) { di.Create(); }
                    try
                    {
                        file.SaveAs(path + FilePath + FileName);
                        files.Add("/" + FilePath + FileName);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
            }
            return Json(new FileResponse { errno = 0, data = files });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        [HttpGet]
        public HttpResponseMessage DownloadFile()
        {
            string fileName = "报表模板.xlsx";
            string filePath = HttpContext.Current.Server.MapPath("~/") + "FileRoot\\" + "ReportTemplate.xlsx";
            FileStream stream = new FileStream(filePath, FileMode.Open);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode(fileName)
            };
            response.Headers.Add("Access-Control-Expose-Headers", "FileName");
            response.Headers.Add("FileName", HttpUtility.UrlEncode(fileName));
            return response;
        }
    }
}
