﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Security.Claims;
using helps.Service.DataObjects;
using helps.Service.Models;
using helps.Service.Utils;

namespace helps.Service.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class SignInController : ApiController
    {
        public ApiServices Services { get; set; }
        public IServiceTokenHandler handler { get; set; }

        // POST api/SignIn
        public HttpResponseMessage Post(LoginRequest loginRequest)
        {
            helpsDbContext context = new helpsDbContext();
            User account = context.Users
                .Where(a => a.StudentId == loginRequest.StudentId).SingleOrDefault();
            if (account != null)
            {
                byte[] incoming = LoginProviderUtil
                    .hash(loginRequest.Password, account.Salt);

                if (!account.Confirmed)
                {
                    return this.Request.CreateResponse(HttpStatusCode.Unauthorized, "Email has not been confirmed");
                }

                if (LoginProviderUtil.slowEquals(incoming, account.SaltedAndHashedPassword))
                {
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity();
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginRequest.StudentId));
                    LoginResult loginResult = new helpsLoginProvider(handler).CreateLoginResult(claimsIdentity, Services.Settings.MasterKey);
                    var customLoginResult = new helpsLoginResult()
                    {
                        StudentId = account.StudentId,
                        FirstName = account.FirstName,
                        LastName = account.LastName,
                        Email = account.Email,
                        HasLoggedIn = account.HasLoggedIn,
                        AuthToken = loginResult.AuthenticationToken
                    };
                    return this.Request.CreateResponse(HttpStatusCode.OK, customLoginResult);
                }
            }
            return this.Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
        }
    }
}
