using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiDocumentUploader.Auth;
using WebApiDocumentUploader.DB;

namespace WebApiDocumentUploader.DBService
{
    public interface IUploadService
    {
        UploadRecord UploadRecord(string userName, string fileName, string path, string savePath);
        Task UploadDetailAsync(List<UploadDetail> list);
        /// <summary>
        /// 最近一次上传记录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        UploadRecordResultDto GetLastUploadHistory(string userName);
        /// <summary>
        /// 上传记录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        List<UploadRecordResultDto> GetUploadHistory(string userName);
    }
}
