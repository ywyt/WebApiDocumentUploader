using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiDocumentUploader.Auth;
using WebApiDocumentUploader.DB;

namespace WebApiDocumentUploader.DBService
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req, out User user);
    }
}
