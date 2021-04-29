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
        public int ProcessRequest(string contact, string destination, List<string> packages)
        {

            var task = Task.Run(async () => await ExecuteAllProviders(contact, destination, packages));
            var result = task.Result;
            return result;
        }


        private static async Task<int> ExecuteAllProviders(string contact, string destination, List<string> packages)
        {
            List<int> listOutput = new List<int>();
            var apiResult1 = ExecuteAPI1(contact, destination, packages);
            var apiResult2 = ExecuteAPI2(contact, destination, packages);
            var apiResult3 = ExecuteAPI3(contact, destination, packages);
            await Task.WhenAll(apiResult1, apiResult2, apiResult3);
            listOutput.Add(apiResult1.Result);
            listOutput.Add(apiResult2.Result);
            listOutput.Add(apiResult3.Result);
            if (listOutput.Count > 0)
            {
                listOutput.Sort();
                return listOutput[0];
            }
            return 0;
        }

        private static async Task<int> ExecuteAPI1(string contact, string destination, List<string> packages)
        {
            var request = new
            {
                contactAddress = contact,
                warehouseAddress = destination,
                PackageDimension = packages
            };

            var url = $"api/CallAPI1/";
            var apiUrl = "http://ap1.com";
            try
            {
                HttpService _coreHttpService = new HttpService(apiUrl);
                _coreHttpService.AddHeader("Authorization", "AuthApi1");
                var result = await _coreHttpService.Post<object>(url, request);
                int total = 0;
                int.TryParse(result.ToString(), out total);
                return total;
            }
            catch (Exception ex)
            {
                return 50;
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

            var url = $"api/CallAPI2/";
            var apiUrl = "http://ap2.com";
            try
            {
                HttpService _coreHttpService = new HttpService(apiUrl);
                _coreHttpService.AddHeader("Authorization", "AuthApi2");
                var result = await _coreHttpService.Post<object>(url, request);
                int total = 0;
                int.TryParse(result.ToString(), out total);
                return total;
            }
            catch (Exception ex)
            {
                return 40;
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

            var url = $"api/CallAPI3/";
            var apiUrl = "http://ap3.com";
            try
            {
                HttpService _coreHttpService = new HttpService(apiUrl);
                _coreHttpService.AddHeader("Authorization", "AuthApi3");
                var xmldoc = await _coreHttpService.PostXMLData(url, xml);
                var result = xmldoc.SelectSingleNode("/quote").Value;
                int total = 0;
                int.TryParse(result.ToString(), out total);
                return total;
            }
            catch (Exception ex)
            {
                return 20;
            }
        }

    }
}
