
using System;

namespace Wshare.Controllers.DTOs
{
    public class ArtResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Cover { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public int Visitors { get; set; }
        public int State { get; set; }
    }

    public class ArtRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Cover { get; set; }

        public string Contents { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public int Visitors { get; set; }
        public int State { get; set; }
    }

    public class FileRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public DateTime Created { get; set; }
    }
}