using System.Collections.Generic;

namespace Case.Business
{
    public interface IExternalProviderService
    {
        int ProcessRequest(string contact, string destination, List<string> packages);
    }
}