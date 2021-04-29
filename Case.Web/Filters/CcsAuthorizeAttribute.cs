using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Case.Business;

namespace Cases.Web.Api.Filters
{
    public class CcsAuthorizeAttribute : ActionFilterAttribute {
        /// <summary>
        /// this will have default bearer
        /// Default Bearer authorization key is (AuthorizationBearer) or pass a new BearerKey to attribute properties
        /// </summary>
        public string BearerKey { get; set; }

        public bool ValidationRequired { get; set; } = true;
        /// <summary>
        /// check for authorize caller by header value
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext) {
            var configuration = actionContext.HttpContext.RequestServices.GetService<IConfiguration>();
            var authorizationEnabled = bool.Parse(configuration["AuthorizationEnabled"]);
            if (!authorizationEnabled) {
                base.OnActionExecuting(actionContext);
                return;
            }

            IEnumerable<string> values = actionContext.HttpContext.Request.Headers["authorization"];
            var defaultAuthorizationValue = configuration["AuthorizationBearer"];
            var bearerAutorizeKeys = !string.IsNullOrWhiteSpace(BearerKey) ? BearerKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;
            var bearerAuthorizeValues = new List<string>();
            if (bearerAutorizeKeys != null && bearerAutorizeKeys.Any()) {
                bearerAutorizeKeys.ToList().ForEach(k => {
                    if (!string.IsNullOrWhiteSpace(configuration[k]))
                        bearerAuthorizeValues.Add(configuration[k]);
                });
            }

            if (actionContext.HttpContext.Request.Headers["authorization"].Any() &&
                (values.First() == defaultAuthorizationValue ||
                 (bearerAuthorizeValues != null && bearerAuthorizeValues.Contains(values.First())))) {
                if (ValidationRequired) {
                    var token = actionContext.HttpContext.Request.Headers["token"];
                   if (!string.IsNullOrEmpty(token)) {
                        var userToken = GetUserTokenFromHeader(token, configuration);
                        var userValidForClient = new AccountsExternalService(configuration).ValidateUserByToken(userToken);
                        if (userValidForClient) {
                            base.OnActionExecuting(actionContext);
                            return;
                        }
                        actionContext.Result = new ObjectResult("unauthorized") { StatusCode = 401 };
                    }
                    else
                        actionContext.Result = new ObjectResult("unauthorized") {StatusCode = 401};
                }
                base.OnActionExecuting(actionContext);
                return;
            }

            // if public call return forbidden 403 instead of unauthorized
            actionContext.Result = !string.IsNullOrWhiteSpace(BearerKey) ? new ObjectResult("forbidden") { StatusCode = 403 } : new ObjectResult("unauthorized") { StatusCode = 401 };
        }

        protected bool ValidateUserByToken(string userToken)
        {
            if (userToken.Equals("offer")) return true;
            else
                return false;
        }

        protected string GetUserTokenFromHeader(string token, IConfiguration configuration) {
            try {
                var encryptionEnabled = Convert.ToBoolean(configuration["UserTokenEncryptionEnabled"]);
                if (encryptionEnabled) {
                    var decryptedToken = JwtSecurityService.Decrypt(token);
                    if (!string.IsNullOrWhiteSpace(decryptedToken)) {
                        var userToken = JwtSecurityService.Decode(decryptedToken);
                        return userToken;
                    }
                }

                return token;
            }
            catch(Exception exception) {
                return null;
            }
        }
    }
}
