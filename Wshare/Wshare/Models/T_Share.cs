//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wshare.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_Share
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FromId { get; set; }
        public System.DateTime Created { get; set; }
        public bool IsBuy { get; set; }
    
        public virtual T_Share T_Share1 { get; set; }
        public virtual T_Share T_Share2 { get; set; }
    }
}