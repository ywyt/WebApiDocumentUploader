using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiDocumentUploader.Auth;
using WebApiDocumentUploader.DB;

namespace WebApiDocumentUploader.DBService
{
    public class UserService : IUserService
    {
        public MyContext db;
        public UserService(MyContext myContext) 
        {
            db = myContext;
        }
        public bool IsValid(LoginRequestDTO req, out User user)
        {
            user = null;
            if (string.IsNullOrEmpty(req.Password))
                return false;

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(req.Password);
            string result = BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", "");

            user = db.Users.Where(o => o.Name == req.Username).FirstOrDefault();
            if (result?.Equals(user?.Pwd) != true)
                return false;
            else
                return true;
        }
    }
}
