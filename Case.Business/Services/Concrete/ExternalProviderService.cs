using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Case.Business;

 namespace Case.Business
{
    public class ExternalProviderService : IExternalProviderService
    {
        public ExternalProviderService()
        {
        }
        public OfferResponseDTO ProcessRequest(string contact, string destination, List<string> packages)
        {

            var task = Task.Run(async () => await ExecuteAllProviders(contact, destination, packages));
            if (task.Result > 0) {
                return new OfferResponseDTO { Offer = task.Result };
            }
            return null;
            
        }


        private static async Task<int> ExecuteAllProviders(string contact, string destination, List<string> packages)
        {
            List<int> lstOutput = new List<int>();
            var apiResult1 = ExecuteAPI1(contact, destination, packages);
            var apiResult2 = ExecuteAPI2(contact, destination, packages);
            var apiResult3 = ExecuteAPI3(contact, destination, packages);
            await Task.WhenAll(apiResult1, apiResult2, apiResult3);
            if (apiResult1.Result != 0)  lstOutput.Add(apiResult1.Result);
            if (apiResult2.Result != 0)  lstOutput.Add(apiResult2.Result);
            if (apiResult3.Result != 0)  lstOutput.Add(apiResult3.Result);
            if (lstOutput.Count > 0)
            {
                lstOutput.Sort();
                return lstOutput[0];
            }

            return 0;
        }

        public static async Task<int> ExecuteAPI1(string contact, string destination, List<string> packages)
        {
            var request = new
            {
                contactAddress = contact,
                warehouseAddress = destination,
                PackageDimension = packages
            };

            var urlPath = $"api/CallAPI1/";
            var apiBaseUrl = "http://ap1.com";
            try
            {
                HttpService _coreHttpService = new HttpService(apiBaseUrl);
                _coreHttpService.AddHeader("Authorization", "AuthApi1");
                var result = await _coreHttpService.Post<object>(urlPath, request);
                int total = 0;
                int.TryParse(result.ToString(), out total);
                return total;
            }
            catch (Exception ex)
            {
                return 50;//Todo:  temporarily output added 
            }

        }

        private static async Task<int> ExecuteAPI2(string contact, string destination, List<string> packages)
        {
            var request = new
            {
                consignee = contact,
                consignor = destination,
                cartons = packages
            };

            var urlPath = $"api/CallAPI2/";
            var apiBaseUrl = "http://ap2.com";
            try
            {
                HttpService _coreHttpService = new HttpService(apiBaseUrl);
                _coreHttpService.AddHeader("Authorization", "AuthApi2");
                var result = await _coreHttpService.Post<object>(urlPath, request);
                int total = 0;
                int.TryParse(result.ToString(), out total);
                return total;
            }
            catch (Exception ex)
            {
                return 40;//Todo:  temporarily output added 
            }
        }

        private static async Task<int> ExecuteAPI3(string contact, string destination, List<string> packages)
        {
            API3RequestDTO aPI3RequestDTO = new API3RequestDTO
            {
                Source = contact,
                Destination = destination,
                Packages = new Packages
                {
                    Package = packages
                }
            };
            string xml = aPI3RequestDTO.ToXmlString(true, true);

            var urlPath = $"api/CallAPI3/";
            var apiBaseUrl = "http://ap3.com";
            try
            {
                HttpService _coreHttpService = new HttpService(apiBaseUrl);
                _coreHttpService.AddHeader("Authorization", "AuthApi3");
                var xmldoc = await _coreHttpService.PostXMLData(urlPath, xml);
                var result = xmldoc.SelectSingleNode("/quote").Value;
                int total = 0;
                int.TryParse(result.ToString(), out total);
                return total;
            }
            catch (Exception ex)
            {
                return 20;//Todo:  temporarily output added 
            }
        }

    }
}
