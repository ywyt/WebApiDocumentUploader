using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiDocumentUploader.DB
{
    //添加模型类
    [Table("user_login")]
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Pwd { get; set; }
        public int IsOpen { get; set; }
        public string RoleId { get; set; }
        public string OfficeId { get; set; }
        public string Version { get; set; }
        public string CompanyId { get; set; }

    }
}
