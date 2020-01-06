using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wshare.Controllers.DTOs
{
    public class BaseResponse
    {
        public string msg { get; set; } = "";
        public int code { get; set; } = 0;
        public int count { get; set; }

        public object data { get; set; }
    }

    public class FileResponse
    {
        public int errno { get; set; } = 0;
        public object data { get; set; }
    }
}
