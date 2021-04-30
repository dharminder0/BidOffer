using System.Collections.Generic;

namespace Case.Business
{
    public interface IExternalProviderService
    {
        OfferResponseDTO ProcessRequest(string contact, string destination, List<string> packages);
    }
}