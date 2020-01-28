using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Wshare.Controllers.DTOs;
using Wshare.Models;

namespace Wshare.Controllers
{
    public class V1Controller : ApiController
    {
        private readonly wshareEntities _context;

        public V1Controller()
        {
            _context = new wshareEntities();
        }

        [HttpPost]
        [Route("api/v1/login")]
        public JsonResult<BaseResponse> Login(LoginRequest model)
        {
            var users = from a in _context.T_Admin
                        where a.LoginId == model.UserName && a.PassWord == model.PassWord
                        select a;
                       
            return Json(new BaseResponse { code = 0, data = users, msg = "", count = users.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/users")]
        public JsonResult<BaseResponse> Users(int page=1,int limit=10)
        {
            var users = from u in _context.T_User
                        select new UserResponse()
                        {
                            City = u.City,
                            Country = u.Country,
                            Headimgurl = u.Headimgurl,
                            NickName = u.Nickname,
                            Sex = u.Sex,
                            Id = u.Id,
                            Province = u.Province,
                            CreateTime = u.CreateTime
                        };
            var query = users.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);
            
            return Json(new BaseResponse { code = 0, data = query, msg = "", count = users.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/users")]
        public JsonResult<BaseResponse> Users(string nickname,string sex,int page= 1, int limit = 10)
        {
            var users = from u in _context.T_User
                        where (string.IsNullOrEmpty(nickname) || u.Nickname == nickname) && (string.IsNullOrEmpty(sex) || u.Sex == sex)
                        select new UserResponse()
                        {
                            City = u.City,
                            Country = u.Country,
                            Headimgurl = u.Headimgurl,
                            NickName = u.Nickname,
                            Sex = u.Sex,
                            Id = u.Id,
                            Province = u.Province,
                            CreateTime = u.CreateTime
                        };
            int count = users.Count();
            if (count > 0)
            {
                if (count / limit < page)
                {
                    page = 1;
                }
            }
            var query = users.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = count }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/articles")]
        public JsonResult<BaseResponse> Article(int page = 1, int limit = 10)
        {
            var arts = from u in _context.T_Article
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
            var query = arts.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);
            
            return Json(new BaseResponse { code = 0, data = query, msg = "", count = arts.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/articles")]
        public JsonResult<BaseResponse> Article(string keyword, int page = 1, int limit = 10)
        {
            var arts = from u in _context.T_Article
                       where u.Title.Contains(keyword) || string.IsNullOrEmpty(keyword)
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
        [Route("api/v1/ashow")]
        public JsonResult<BaseResponse> AShow(int id,int state)
        {
            try
            {
                var obj = _context.T_Article.Find(id);
                obj.State = state;
                _context.SaveChanges();
                return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/v1/comments")]
        public JsonResult<BaseResponse> Comment(int page = 1, int limit = 10)
        {
            var comments = from u in _context.T_Comment
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
                               NickName = u.T_User == null ? "" : u.T_User.Nickname
                           };
            var query = comments.OrderBy(o => o.State).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = comments.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/comments")]
        public JsonResult<BaseResponse> Comment(string keyword, int page = 1, int limit = 10)
        {
            var comments = from u in _context.T_Comment
                           where u.Contents.Contains(keyword) || string.IsNullOrEmpty(keyword)
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
                               NickName = u.T_User == null ? "" : u.T_User.Nickname
                           };
            int count = comments.Count();
            if (count > 0)
            {
                if (count / limit < page)
                {
                    page = 1;
                }
            }
            var query = comments.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = count }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/cshow")]
        public JsonResult<BaseResponse> CShow(int id,int state)
        {
            try
            {
                var obj = _context.T_Comment.Find(id);
                obj.State = state;
                _context.SaveChanges();
                return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/v1/creply")]
        public JsonResult<BaseResponse> CReply(ReplyRequest model)
        {
            try
            {
                var obj = _context.T_Comment.Find(model.Id);
                obj.Reply = model.Reply;
                obj.Updated = DateTime.Now;
                _context.SaveChanges();
                return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/v1/cdel")]
        public JsonResult<BaseResponse> CDel(ReplyRequest model)
        {
            try
            {
                var obj = _context.T_Comment.Find(model.Id);
                _context.T_Comment.Remove(obj);
                _context.SaveChanges();
                return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/v1/visitors")]
        public JsonResult<BaseResponse> Visitor(int page = 1, int limit = 10)
        {
            var visitors = from u in _context.T_Visitors
                           select new VisitorResponse()
                           {
                               UserId = u.UserId,
                               ArticleId = u.ArticleId,
                               ArtTitle = u.T_Article.Title,
                               Created = u.Created,
                               Id = u.Id,
                               NickName = u.T_User == null ? "" : u.T_User.Nickname,
                               ShareId = u.ShareId,
                               ShareNickName = u.T_User1 == null ? "" : u.T_User1.Nickname
                           };
            var query = visitors.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = visitors.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }


        [HttpGet]
        [Route("api/v1/authcode")]
        public JsonResult<BaseResponse> Authorize(int page = 1, int limit = 10)
        {
            var visitors = from u in _context.T_Authorize
                           select new
                           {
                               Id = u.Id,
                               NickName = u.T_User.Nickname,
                               Code = u.Code,
                               Created = u.Created,
                               Tag = u.Tag,
                               State = u.State
                           };
            var query = visitors.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = visitors.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpGet]
        [Route("api/v1/files")]
        public JsonResult<BaseResponse> Files(int page = 1, int limit = 10)
        {
            var files = from u in _context.T_Files
                           select u;
            var query = files.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = files.Count() }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
        
        [HttpGet]
        [Route("api/v1/files")]
        public JsonResult<BaseResponse> Files(string keyword,int page = 1, int limit = 10)
        {
            var files = from u in _context.T_Files
                        where u.Title.Contains(keyword) || string.IsNullOrEmpty(keyword)
                        select u;
            int count = files.Count();
            if (count > 0)
            {
                if (count / limit < page)
                {
                    page = 1;
                }
            }
            var query = files.OrderByDescending(o => o.Id).Skip((page - 1) * limit).Take(limit);

            return Json(new BaseResponse { code = 0, data = query, msg = "", count = count }, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        [HttpPost]
        [Route("api/v1/files")]
        public JsonResult<BaseResponse> Files(FileRequest model)
        {
            try
            {
                T_Files file;
                if (model.Id == 0)
                {
                    file = new T_Files()
                    {
                        Title = model.Title,
                        FileName = model.FileName,
                        Created = DateTime.Now,
                    };
                    _context.T_Files.Add(file);
                }
                else
                {
                    file = _context.T_Files.Find(model.Id);
                    file.Title = model.Title;
                    file.FileName = model.FileName;
                }
                _context.SaveChanges();
                return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {

                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/v1/fileDel")]
        public JsonResult<BaseResponse> FileDel(FileRequest model)
        {
            try
            {
                var obj = _context.T_Files.Find(model.Id);
                _context.T_Files.Remove(obj);
                _context.SaveChanges();
                return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
        }
        
        [HttpPost]
        [Route("api/v1/article")] 
        public JsonResult<BaseResponse> Article(ArtRequest model)
        {
            try
            {
                T_Article art = _context.T_Article.Find(model.Id);
                art = art ?? new T_Article();

                if (model.Id == 0)
                {
                    art.Title = model.Title;
                    art.Author = model.Author;
                    art.Tags = model.Tags;
                    art.SubTitle = model.SubTitle;
                    art.Updated = DateTime.Now;
                    art.State = model.State;
                    art.Visitors = model.Visitors;
                    art.Created = DateTime.Now;
                    art.Cover = "";
                    art.Contents = "";
                    art.Source = "";
                    art.Reading = 0;

                    _context.T_Article.Add(art);
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Contents))
                    {
                        art.Contents = model.Contents;
                        art.Cover = model.Cover ?? art.Cover;
                    }
                    else
                    {
                        art.Title = model.Title;
                        art.Author = model.Author;
                        art.Tags = model.Tags;
                        art.SubTitle = model.SubTitle;
                        art.Updated = DateTime.Now;
                        art.State = model.State;
                        art.Visitors = model.Visitors;
                        art.Created = model.Created;
                        art.Cover = "";
                        art.Contents = "";
                        art.Source = "";
                        art.Reading = 0;
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new BaseResponse() { code = 500, msg = ex.Message });
            }
            return Json(new BaseResponse() { code = 200, msg = "操作成功！" });
        }
    }
}
