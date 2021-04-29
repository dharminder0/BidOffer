using System;
using System.Collections.Generic;
using System.Text;

namespace Case.Business
{
    public class OfferRequestDTO
    {
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public List<string> CartoonDimension { get; set; }
    }
}
