using Microsoft.Extensions.Configuration;
using System;

namespace Case.Business
{
    public class AccountsExternalService
    {
        public AccountsExternalService(IConfiguration configuration)
        { 
        
        }
        public bool ValidateUserByToken(string userToken)
        {
            if (userToken.Equals("offer")) return true;
            else
                return false;
        }
    }
}
