using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TetrisApiServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Login")]
    public class LoginController : Controller
    {
        [HttpPost]
        public async Task<LoginRes> Process([FromBody] LoginReq request)
        {
            //var tick = DateTime.Now.Ticks.ToString();                    
            return new LoginRes() { Result = 0, AuthToken = "fake" };

        }

        string CreateAuthToken()
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return token;
        }
                
    }

    public class LoginReq
    {
        public string UserID;
        public string UserPW;
    }

    public class LoginRes
    {
        public short Result;
        public string AuthToken;
    }
}