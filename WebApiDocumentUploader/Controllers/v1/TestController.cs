#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SevenZipExtractor;
using UploadStream;
using WebApiDocumentUploader.DB;
using WebApiDocumentUploader.DBService;
using WebApiDocumentUploader.Model.DTO;
using WebApiDocumentUploader.Model.DTO.Developer;
using WebApiDocumentUploader.Services;

namespace WebApiDocumentUploader.Controllers.v1
{
    [ApiController]
    //[Authorize]
    public class TestController : BaseController
    {
        private readonly ILogger<UploadStreamController> _logger;
        private readonly UploadStreamService _uploadStreamService;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly IUploadService _uploadService;

        const int BUF_SIZE = 4096;

        public TestController(ILogger<UploadStreamController> logger, 
            UploadStreamService uploadStreamService,
            IWebHostEnvironment environment,
            IUploadService uploadService)
        {
            _logger = logger;
            _uploadStreamService = uploadStreamService;
            _hostingEnvironment = environment;
            _uploadService = uploadService;
        }
        
        [HttpPost("collection_of_file")]
        [ProducesResponseType(typeof(BaseDeveloperResponse), 200)]
        [DisableRequestSizeLimit]
        [Obsolete]
        public async Task<IActionResult> ControllerModelStreamBindingEnabled(UploadStreamDto model) {
            
            byte[] buffer = new byte[BUF_SIZE];
            List<IFormFile> files = new List<IFormFile>();

            var streamModel = await this.StreamFiles<UploadStreamClass>(async x => {
                using (var stream = x.OpenReadStream())
                    while (await stream.ReadAsync(buffer, 0, buffer.Length) > 0) ;
                files.Add(x);
            });

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(new
            {
                //model, 
                Received = streamModel, 
                //Files = files
                Files = files.Select(x => new {
                    x.Name,
                    x.FileName,
                    x.ContentDisposition,
                    x.ContentType,
                    x.Length
                })
            });
        }

        [HttpGet("Unzip")]
        public IActionResult Unzip()
        {
            // "C:\Program Files\7-Zip\7z" x 中文.zip -oC:\Users\zephyr\Desktop\test\testnew\
            string filePath = @"C:\Users\zephyr\Desktop\test\中文.zip";
            string username = "用户姓名";
            string extractPath = Path.Combine(@"C:\Users\zephyr\Desktop\test", username);
            StreamModel model = new StreamModel() { ImagePaths = new List<string>() };
            List<UploadDetail> uploadDetails = new List<UploadDetail>();
            using (var zip = ZipFile.Open(filePath, ZipArchiveMode.Update, Encoding.GetEncoding("utf-8")))
            {
                Uri baseUri = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
                foreach (var entry in zip.Entries)
                {
                    var detail = new UploadDetail
                    {
                        RecordId = 1,
                        FileName = entry.Name,
                        Path = Path.Combine(@"C:\Users\zephyr\Desktop\test", username, entry.FullName)
                    };
                    Uri myUri = new Uri(baseUri, detail.Path);
                    model.ImagePaths.Add(myUri.ToString());
                    uploadDetails.Add(detail);
                    //string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                    //// Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                    //// are case-insensitive.
                    //if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                    //    entry.ExtractToFile(destinationPath, true);
                }
            }
            Encoding _encoding = Encoding.GetEncoding("utf-8");
            ZipFile.ExtractToDirectory(filePath, extractPath, _encoding, true);
            return Ok(new { uploadDetails = uploadDetails, model = model });
        }

        [HttpGet("Unzip2")]
        public IActionResult Unzip2()
        {
            // "C:\Program Files\7-Zip\7z" x 中文.zip -oC:\Users\zephyr\Desktop\test\testnew\
            string filePath = @"C:\Users\zephyr\Desktop\test\中文.zip";
            string username = "用户姓名";
            string extractPath = Path.Combine(@"C:\Users\zephyr\Desktop\test", username);
            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);
            StreamModel model = new StreamModel() { ImagePaths = new List<string>() };
            List<UploadDetail> uploadDetails = new List<UploadDetail>();
            using (ArchiveFile archiveFile = new ArchiveFile(filePath))
            {
                foreach (Entry entry in archiveFile.Entries)
                {
                    Console.WriteLine(entry.FileName);
                    uploadDetails.Add(new UploadDetail { SavePath= entry.IsFolder.ToString(),  FileName = entry.FileName, Path = entry.Size.ToString() });
                    //// extract to file
                    //entry.Extract(entry.FileName);

                    //// extract to stream
                    //MemoryStream memoryStream = new MemoryStream();
                    //entry.Extract(memoryStream);
                    string name = Path.GetFileName(entry.FileName);
                    string fileName = Path.Combine(extractPath, name);
                    using (FileStream fileStream = System.IO.File.Create(fileName))
                    {
                        entry.Extract(fileStream);
                    }

                }
            }
            //using (ArchiveFile archiveFile = new ArchiveFile(filePath))
            //{
            //    // extract all
            //    archiveFile.Extract(extractPath, true);
            //}
            return Ok(new { uploadDetails = uploadDetails, model = model });
        }

        [HttpGet("Unzip3")]
        public IActionResult Unzip3()
        {
            // "C:\Program Files\7-Zip\7z" x 中文.zip -oC:\Users\zephyr\Desktop\test\testnew\
            string filePath = @"C:\Users\zephyr\Desktop\test\中文.zip";
            string username = "用户姓名";
            string extractPath = Path.Combine(@"C:\Users\zephyr\Desktop\test", username);
            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);
            StreamModel model = new StreamModel() { ImagePaths = new List<string>() };
            List<UploadDetail> uploadDetails = new List<UploadDetail>();
            using (var zip = ZipFile.Open(filePath, ZipArchiveMode.Update, Encoding.GetEncoding("utf-8")))
            {
                Uri baseUri = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
                foreach (var entry in zip.Entries)
                {
                    byte[] nmbyte = Encoding.GetEncoding("CP437").GetBytes(entry.Name);
                    string str = System.Text.Encoding.Default.GetString(nmbyte);
                    var detail = new UploadDetail
                    {
                        RecordId = 1,
                        FileName = str,
                        Path = Path.Combine(@"C:\Users\zephyr\Desktop\test", username, entry.FullName)
                    };
                    Uri myUri = new Uri(baseUri, detail.Path);
                    model.ImagePaths.Add(myUri.ToString());
                    uploadDetails.Add(detail);
                    //string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                    //// Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                    //// are case-insensitive.
                    //if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                    //    entry.ExtractToFile(destinationPath, true);
                }
            }
            //Encoding _encoding = Encoding.GetEncoding(850);
            //ZipFile.ExtractToDirectory(filePath, extractPath, _encoding, true);
            return Ok(new { uploadDetails = uploadDetails, model = model });
        }

        [HttpPost("uploadzip")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ControllerModelStreamBigZipFile()
        {
            if (!ModelState.IsValid)
                return BadRequest();

            byte[] buffer = new byte[BUF_SIZE];
            List<IFormFile> files = new List<IFormFile>();

            string username = Response.HttpContext.User.Identity.Name;

            // 文件目录
            string uploads = Path.Combine(_hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath, "uploads", username);

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // 文件保存路径
            string filePath = string.Empty;
            string fileName = string.Empty;
            var model = await this.StreamFiles<StreamModel>(async x =>
            {
                fileName = x.FileName;
                filePath = Path.Combine(uploads, x.FileName);
                // 不存在则新建该文件
                if (!System.IO.File.Exists(filePath))
                {
                    var fs = System.IO.File.Create(filePath);
                    fs.Close();
                }
                // 从头写入该文件，覆盖
                using (var outputStream = System.IO.File.OpenWrite(filePath))
                {
                    using (var stream = x.OpenReadStream())
                    {
                        // 读入缓冲区的字节总数
                        int lenth = 0;
                        // 边读边写
                        while ((lenth = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await outputStream.WriteAsync(buffer, 0, lenth);
                        }
                    }
                    await outputStream.FlushAsync();
                }
                files.Add(x);
            });
            string rootpath = Path.Combine("uploads", username, fileName);
            var uploadRecord =  _uploadService.UploadRecord(username, fileName, rootpath, filePath);

            if (System.IO.Path.GetExtension(filePath) == ".zip")
            {
                // 文件目录
                string extractPath = Path.Combine(_hostingEnvironment.WebRootPath ?? _hostingEnvironment.ContentRootPath, "images", username);

                if (!Directory.Exists(extractPath))
                    Directory.CreateDirectory(extractPath);

                model.ImagePaths = new List<string>();
                List<UploadDetail> uploadDetails = new List<UploadDetail>();
                using (var zip = ZipFile.Open(filePath, ZipArchiveMode.Update, Encoding.GetEncoding("utf-8")))
                {
                    Uri baseUri = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
                    foreach (var entry in zip.Entries)
                    {
                        var detail = new UploadDetail
                        {
                            RecordId = uploadRecord.Id,
                            FileName = entry.Name,
                            Path = Path.Combine("images", username, entry.FullName),
                            SavePath = Path.Combine(extractPath, entry.FullName)
                        };
                        Uri myUri = new Uri(baseUri, detail.Path);
                        model.ImagePaths.Add(myUri.ToString());
                        uploadDetails.Add(detail);
                        //string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                        //// Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        //// are case-insensitive.
                        //if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                        //    entry.ExtractToFile(destinationPath, true);
                    }
                }
                Encoding _encoding = Encoding.GetEncoding("utf-8");
                ZipFile.ExtractToDirectory(filePath, extractPath, _encoding, true);
                await _uploadService.UploadDetailAsync(uploadDetails);
            }

            model.filepath = rootpath;

            return Ok(new
            {
                Model = model,
                Files = files.Select(x => new {
                    x.Name,
                    x.FileName,
                    x.ContentDisposition,
                    x.ContentType,
                    x.Length
                })
            });
        }

        /// <summary>
        /// 最近一次上传记录
        /// </summary>
        /// <returns></returns>
        [HttpPost("lasthistory")]
        public IActionResult GetLastUploadHistory() 
        {
            string username = Response.HttpContext.User.Identity.Name;
            var record = _uploadService.GetLastUploadHistory(username);
            if (record == null)
                return Ok();
            record.Details.ForEach(o => o.BaseUri = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"));
            return Ok(record);
        }

        [HttpPost("stream")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ControllerModelStream()
        {
            byte[] buffer = new byte[BUF_SIZE];
            List<IFormFile> files = new List<IFormFile>();

            var model = await this.StreamFiles<StreamModel>(async x => {
                using (var stream = x.OpenReadStream())
                    while (await stream.ReadAsync(buffer, 0, buffer.Length) > 0)
                        ;
                files.Add(x);
            });

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(new
            {
                Model = model,
                Files = files.Select(x => new {
                    x.Name,
                    x.FileName,
                    x.ContentDisposition,
                    x.ContentType,
                    x.Length
                })
            });
        }

        [Obsolete]
        [HttpPost("stream_modelDisabled")]
        //[DisableFormModelBinding]
        public async Task<IActionResult> ControllerModelStreamModelDisabled([FromForm]UploadModel uploadModel) {
            byte[] buffer = new byte[BUF_SIZE];
            List<IFormFile> files = new List<IFormFile>();

            var model = await this.StreamFiles<StreamModel>(async x => {
                using (var stream = x.OpenReadStream())
                    while (await stream.ReadAsync(buffer, 0, buffer.Length) > 0)
                        ;
                files.Add(x);
            });

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(new {
                Model = model,
                Files = files.Select(x => new {
                    x.Name,
                    x.FileName,
                    x.ContentDisposition,
                    x.ContentType,
                    x.Length
                })
            });
        }
        
    }
}
#endif