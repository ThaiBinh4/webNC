//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webBinh.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public int id_comment { get; set; }
        public string id_user { get; set; }
        public string content { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
    
        public virtual nguoiDung nguoiDung { get; set; }
    }
}
