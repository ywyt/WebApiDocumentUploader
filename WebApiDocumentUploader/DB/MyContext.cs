using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiDocumentUploader.DB
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UploadRecord> UploadRecords { get; set; }
        public DbSet<UploadDetail> UploadDetails { get; set; }
    }
}
