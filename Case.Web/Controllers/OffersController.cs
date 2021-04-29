using Case.Business;
using Cases.Web.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Cases.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        IExternalProviderService _externalProviderService;

        public OffersController(IExternalProviderService externalProviderService)
        {
            _externalProviderService = externalProviderService;
        }

        [HttpPost]
        [CcsAuthorize(ValidationRequired = false)]
        [Route("BidOffer")]
        public object BidOffer(OfferRequestDTO offerRequestDTO)
        {
            if (offerRequestDTO == null)
            {
                return NotFound();
            }
            return _externalProviderService.ProcessRequest(offerRequestDTO.SourceAddress, offerRequestDTO.DestinationAddress, offerRequestDTO.CartoonDimension);
        }


       
    }
}
