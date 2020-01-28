using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wshare.Controllers.DTOs
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string NickName { get; set; }
        public string Headimgurl { get; set; }
        public DateTime Created { get; set; }
        public int ArticleId { get; set; }
        public string Contents { get; set; }
        public int State { get; set; }
        public string Reply { get; set; }
        public DateTime? Updated { get; set; }

        public int Agree { get; set; }
        public int Agree2 { get; set; }
        public string Praised { get; set; }
        public string Praised2 { get; set; }

    } 
    
    public class VisitorResponse
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string ArtTitle { get; set; }
        public int? UserId { get; set; }
        public string NickName { get; set; }
        public int? ShareId { get; set; }
        public string ShareNickName { get; set; }
        public DateTime Created { get; set; }

    }

    public class ReplyRequest
    {
        public int Id { get; set; }
        public string Reply { get; set; }
    }
}