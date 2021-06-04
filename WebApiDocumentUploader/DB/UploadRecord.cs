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
    [Table("upload_record")]
    public class UploadRecord
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string SavePath { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class UploadRecordResultDto : UploadRecord
    {
        public List<UploadDetail> Details { get; set; }
    }
}
