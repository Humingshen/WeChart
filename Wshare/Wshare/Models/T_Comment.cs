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
    
    public partial class T_Comment
    {
        public T_Comment()
        {
            this.T_Agree = new HashSet<T_Agree>();
        }
    
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public System.DateTime Created { get; set; }
        public int ArticleId { get; set; }
        public string Contents { get; set; }
        public int State { get; set; }
        public string Reply { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    
        public virtual ICollection<T_Agree> T_Agree { get; set; }
        public virtual T_Article T_Article { get; set; }
        public virtual T_User T_User { get; set; }
    }
}
