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

        /// <summary>
        /// Authorization Token Required
        /// </summary>
        /// <param name="offerRequestDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [CcsAuthorize(ValidationRequired = false)]
        [Route("BidOffer")]
        public object BidOffer(OfferRequestDTO offerRequestDTO)
        {
            if (offerRequestDTO == null)
            {
                return new NotFoundResult();
            }

            var response = _externalProviderService.ProcessRequest(offerRequestDTO.SourceAddress, offerRequestDTO.DestinationAddress, offerRequestDTO.CartoonDimension);

            if (response == null)
            {
                return new NotFoundResult();
            }
            return response;
        }


        /// <summary>
        /// Authorization + Token Header required 
        /// </summary>
        /// <param name="offerRequestDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [CcsAuthorize(ValidationRequired = true)]
        [Route("v2/BidOffer")]
        public object BidOfferV2(OfferRequestDTO offerRequestDTO)
        {
            if (offerRequestDTO == null)
            {
                return new NotFoundResult();
            }

            var response = _externalProviderService.ProcessRequest(offerRequestDTO.SourceAddress, offerRequestDTO.DestinationAddress, offerRequestDTO.CartoonDimension);

            if (response == null)
            {
                return new NotFoundResult();
            }
            return response;
        }
    }
}
