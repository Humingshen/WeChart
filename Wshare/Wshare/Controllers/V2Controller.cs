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
        [Route("api/v2/comments")]
        public JsonResult<BaseResponse> comment(int articleId, int page = 1, int limit = 10)
        {
            var us = _context.T_User.Find(Lib.UserId);
            var agree = from c in _context.T_Agree
                        where c.UserId == us.Id && c.IsAgree
                        select c.CommentId;
            var agree2 = from c in _context.T_Agree
                         where c.UserId == us.Id && c.IsAgree2
                         select c.CommentId;
            var comments = from u in _context.T_Comment
                           where u.ArticleId == articleId && u.State==1
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
                               Agree = u.T_Agree.Where(o => o.IsAgree).Count(),
                               Agree2 = u.T_Agree.Where(o => o.IsAgree2).Count(),
                               Praised = agree.Contains(u.Id) ? "praised" : "",
                               Praised2 = agree2.Contains(u.Id) ? "praised" : "",
                           };
            var query = comments.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = comments.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [Route("api/v2/comment")]
        [HttpPost]
        public JsonResult<BaseResponse> commentPost(ReqComment model)
        {
            try
            {
                var us = _context.T_User.Find(Lib.UserId);
                T_Comment com = new T_Comment()
                {
                    ArticleId = model.ArticleId,
                    Contents = model.Contents,
                    Created = DateTime.Now,
                    Reply = "",
                    State = 0,
                    UserId = us.Id,
                };
                _context.T_Comment.Add(com);
                _context.SaveChanges();

                return Json(new BaseResponse { code = 0, data = null, msg = "success", count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse { code = 500, data = null, msg = ex.Message, count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
        }


        [HttpGet]
        [Route("api/v2/agree")]
        public JsonResult<BaseResponse> agree(int commentId, int index)
        {
            try
            {
                var us = _context.T_User.Find(Lib.UserId);
                var agree = _context.T_Agree.Where(o => o.CommentId == commentId && o.UserId == us.Id).FirstOrDefault();
                if (agree == null)
                {
                    agree = new T_Agree() { CommentId = commentId, Created = DateTime.Now, IsAgree = false, IsAgree2=false, UserId = us.Id };
                    _context.T_Agree.Add(agree);
                }
                int c=0;
                if (index == 0)
                {
                    agree.IsAgree = !agree.IsAgree;
                }
                else
                {
                    agree.IsAgree2 = !agree.IsAgree2;
                }
                _context.SaveChanges();

                c = _context.T_Agree.Where(o => o.CommentId == commentId && (index == 0 ? o.IsAgree : o.IsAgree2)).Count();

                return Json(new BaseResponse { code = 0, data = c, msg = "", count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            catch (Exception ex)
            {

                return Json(new BaseResponse { code = 500, data = null, msg = ex.Message, count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
        }


        [HttpGet]
        [Route("api/v2/code")]
        public JsonResult<BaseResponse> Code(int articleId)
        {
            try
            {
                var us = _context.T_User.Find(Lib.UserId);
                if (!_context.T_Authorize.Any(o => o.ArticleId == articleId && o.UserId == us.Id))
                {
                    // 是否打赏过
                    if (_context.T_Pay.Any(o => o.UserId == us.Id && o.ArticleId == articleId))
                    {
                        // 生成 打赏
                        T_Authorize auth = new T_Authorize()
                        {
                            Code = Guid.NewGuid().ToString().ToUpper().Substring(0, 6),
                            Created = DateTime.Now,
                            State = 0,
                            Tag = 1,
                            UserId = us.Id,
                            ArticleId = articleId
                        };
                        _context.T_Authorize.Add(auth);
                        _context.SaveChanges();

                    }
                    else
                    {
                        // 检测分享人数 达标生成
                        var vis = _context.T_Visitors.Where(i => i.ArticleId == articleId && i.ShareId == us.Id);
                        if (vis.Count() >= 10)
                        {
                            // 生成 打赏
                            T_Authorize auth = new T_Authorize()
                            {
                                Code = Guid.NewGuid().ToString().ToUpper().Substring(0,6),
                                Created = DateTime.Now,
                                State = 0,
                                Tag = 0,
                                UserId = us.Id,
                                ArticleId = articleId
                            };
                            _context.T_Authorize.Add(auth);
                            _context.SaveChanges();
                        }
                        else
                        {
                            return Json(new BaseResponse { code = 300, data = null, msg = "未达到分享10人", count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                        }
                    }
                }
                var au = _context.T_Authorize.Where(i => i.ArticleId == articleId && i.UserId == us.Id).FirstOrDefault();

                return Json(new BaseResponse { code = 0, data = new { au.Id, au.State, au.Tag,au.Code}, msg = "", count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse { code = 500, data = null, msg = ex.Message, count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
        }
        [HttpGet]
        [Route("api/v2/opencode")]
        public JsonResult<BaseResponse> OpenCode(int authorizeId)
        {
            try
            {
                var auth = _context.T_Authorize.Find(authorizeId);
                if (auth.State == 0)
                {
                    auth.State = 1;
                    _context.SaveChanges();
                    return Json(new BaseResponse { code = 0, data = null, msg = "激活成功", count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                }
                else
                {
                    return Json(new BaseResponse { code = 400, data = null, msg = "激活失败", count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                }

            }
            catch (Exception ex)
            {

                return Json(new BaseResponse { code = 500, data = null, msg = ex.Message, count = 1 }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
        }
    }
}