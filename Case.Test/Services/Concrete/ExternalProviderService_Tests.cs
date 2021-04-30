using Case.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentAssertions;
namespace Case.Test
{
    [TestClass()]
    public class ExternalProviderService_Tests : TestBase
    {
        public readonly IExternalProviderService _externalProviderService;
        public ExternalProviderService_Tests()
        {
            _externalProviderService = Resolve<IExternalProviderService>();
        }

        [TestMethod()]
        public void ProcessRequest_ShouldReturnCorrectOutput()
        {
            var result = _externalProviderService.ProcessRequest("Delhi", "Chandigarh", new List<string> { "203", "255" });

            PrintOutput(result);
            result.Should().NotBeNull();
        }
    }
}
