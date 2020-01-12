using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Wshare.Controllers.DTOs;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class V2Controller : ApiController
    {
        private readonly wshareEntities _context;

        public V2Controller()
        {
            _context = new wshareEntities();
        }

        [HttpGet]
        [Route("api/v2/article")]
        public JsonResult<BaseResponse> article(int page = 1, int limit = 10, string key = "")
        {
            var arts = from u in _context.T_Article
                       where u.Title.Contains(key) || string.IsNullOrEmpty(key)
                       select new ArtResponse()
                       {
                           Author = u.Author,
                           Cover = u.Cover,
                           Created = u.Created,
                           State = u.State,
                           SubTitle = u.SubTitle,
                           Tags = u.Tags,
                           Title = u.Title,
                           Visitors = u.Visitors,
                           Id = u.Id
                       };
            int count = arts.Count();
            if (count > 0)
            {
                if (count / limit < page)
                {
                    page = 1;
                }
            }
            var query = arts.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = count }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
        [HttpGet]
        [Route("api/v2/comment")]
        public JsonResult<BaseResponse> comment(int articleId, int page = 1, int limit = 10)
        {
            var comments = from u in _context.T_Comment
                           where u.ArticleId == articleId
                           select new CommentResponse()
                           {
                               ArticleId = u.ArticleId,
                               Contents = u.Contents,
                               Created = u.Created,
                               Id = u.Id,
                               Reply = u.Reply,
                               State = u.State,
                               Updated = u.Updated,
                               UserId = u.UserId,
                               Headimgurl = u.T_User == null ? "" : u.T_User.Headimgurl,
                               NickName = u.T_User == null ? "" : u.T_User.Nickname,
                               Agree = u.T_Agree.Count,
                               Agree2 = u.T_Agree.Count
                           };
            var query = comments.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = comments.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}