using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiDocumentUploader.Auth
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDTO request, out string token);
    }
}
