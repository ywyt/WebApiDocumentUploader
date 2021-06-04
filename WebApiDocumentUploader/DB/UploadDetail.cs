using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiDocumentUploader.DB
{
    //添加模型类
    [Table("upload_detail")]
    public class UploadDetail
    {
        [Key]
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string SavePath { get; set; }

        /// <summary>
        /// 网站地址，不与数据库关联
        /// </summary>
        [NotMapped]
        public Uri BaseUri { get; set; }
        public string WebPath 
        { 
            get 
            { 
                Uri myUri = new Uri(BaseUri, Path);  
                return myUri.ToString(); 
            } 
        }
    }
}
