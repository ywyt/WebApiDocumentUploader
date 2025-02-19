﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiDocumentUploader.Auth;
using WebApiDocumentUploader.DB;
using WebApiDocumentUploader.Helper;

namespace WebApiDocumentUploader.DBService
{
    public class UploadService : IUploadService
    {
        public MyContext db;
        public UploadService(MyContext myContext) 
        {
            db = myContext;
        }

        public UploadRecord UploadRecord(string userName, string fileName, string path, string savePath)
        {
            User user = db.Users.FirstOrDefault(o => o.Name == userName);
            UploadRecord upload = new UploadRecord()
            {
                UserId = user?.UserId,
                UserName = userName,
                FileName = fileName,
                Path = path,
                SavePath = savePath,
                CreateTime = DateTime.Now
            };
            db.UploadRecords.Add(upload);
            db.SaveChanges();
            return upload;
        }

        public async Task UploadDetailAsync(List<UploadDetail> list)
        {
            await db.UploadDetails.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// 最近一次上传记录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UploadRecordResultDto GetLastUploadHistory(string userName)
        {
            User user = db.Users.FirstOrDefault(o => o.Name == userName);
            if (user == null)
                return null;
            var record = db.UploadRecords.Where(o => o.UserName == userName).OrderByDescending(o => o.CreateTime).FirstOrDefault();
            if (record == null)
                return null;
            var list = db.UploadDetails.Where(o => o.RecordId == record.Id).ToList();
            var result = AutoMapHelper.Map<UploadRecord, UploadRecordResultDto>(record);
            result.Details = list;
            return result;
        }

        /// <summary>
        /// 上传记录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<UploadRecordResultDto> GetUploadHistory(string userName)
        {
            User user = db.Users.FirstOrDefault(o => o.Name == userName);
            if (user == null)
                return null;
            var records = db.UploadRecords.Where(o => o.UserName == userName).OrderByDescending(o => o.CreateTime);
            if (records == null)
                return null;
            var list = db.UploadDetails.Where(d => records.Any(o => o.Id == d.RecordId)).ToList();
            List<UploadRecordResultDto> results = new List<UploadRecordResultDto>();
            records.ToList().ForEach(o => 
            {
                var record = AutoMapHelper.Map<UploadRecord, UploadRecordResultDto>(o);
                record.Details = list.Where(l => l.RecordId == o.Id).ToList();
                results.Add(record);
            });

            return results;
        }
    }
}
