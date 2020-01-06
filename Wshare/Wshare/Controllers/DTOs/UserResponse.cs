using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wshare.Controllers.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }

        [JsonProperty("nickname")]
        public string NickName { get; set; }
        public string Sex { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Headimgurl { get; set; }

        public DateTime CreateTime { get; set; }
    }
}