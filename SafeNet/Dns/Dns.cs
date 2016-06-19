
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SafeNet.Auth;
using SafeNet.Exceptions;

namespace SafeNet.Dns
{
    public static class Dns
    {
        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/dns-create-long-name
        /// 
        /// Register a long name for the user. Long names are the public names that can be shared.
        /// Only authorised requests can create a directory.
        /// 
        /// </summary>
        /// <param name="registration">valid registration</param>
        /// <param name="longName">the name to be created</param>
        /// <returns></returns>
        public static async Task<bool> DnsCreateLongName(this SafeRegisterResponse registration, string longName)
        {
            using (var client = registration.Client)
            {
                var resp = await client.PostAsync($"dns/{WebUtility.UrlEncode(longName)}", new StringContent(""));
                return resp.StatusCode == HttpStatusCode.OK;
            }
        }

        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/dns-register-service and https://maidsafe.readme.io/docs/dns
        /// 
        /// Not clear on the interpretation of usePut just yet
        /// 
        /// Register a service for a long name.
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="longName">The long name to which the service has to be added.</param>
        /// <param name="serviceName">Name of the service to be added.</param>
        /// <param name="serviceHomeDirPath">The full path of the directory that has to be associated to the service.</param>
        /// <param name="isPathShared">If the path is shared then the directory is created within the SAFE Drive, or else the directory will be created in the application's root directory.</param>
        /// <param name="usePut">if true, use PUT if false, use POST</param>
        /// <returns></returns>
        public static async Task<bool> DnsRegisterService(this SafeRegisterResponse registration, string longName,
            string serviceName, string serviceHomeDirPath, bool isPathShared, bool usePut)
        {
            var payloadStr = JsonConvert.SerializeObject(
                    new RegisterService(
                        longName
                        , serviceName
                        , serviceHomeDirPath
                        , isPathShared
                ));

            var encryptedPayload = registration.EncryptPayload(payloadStr);

            var payloadObj = new StringContent(encryptedPayload, Encoding.UTF8, "text/plain");

            HttpResponseMessage resp;

            using (var client = registration.Client)
            {
                if (usePut)
                {
                    resp = await client.PutAsync("dns", payloadObj);
                }
                else
                {
                    resp = await client.PostAsync("dns", payloadObj);
                }
                 

                return resp.StatusCode == HttpStatusCode.OK;
            }
        }


        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/dns-list-long-names
        /// 
        /// List the long names created by the user.
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public static async Task<List<string>> DnsListLongNames(this SafeRegisterResponse registration)
        {
            using (var client = registration.Client)
            {
                var resp = await client.GetAsync("dns/");
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    throw new AuthException(resp); 
                }
                var respString = await resp.Content.ReadAsStringAsync(); 
                var decrypted = registration.DecryptResponse(respString);

                return JsonConvert.DeserializeObject<List<string>>(decrypted); 
            }
        }

        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/dns-list-services
        /// 
        /// List the service names registered for a long name.
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="longName"></param>
        /// <returns></returns>
        public static async Task<List<string>> DnsListServices(this SafeRegisterResponse registration, string longName)
        {
            using (var client = registration.Client)
            {
                var resp = await client.GetAsync($"dns/{WebUtility.UrlEncode(longName)}");

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    throw new AuthException(resp);
                }
                var decrypted = registration.DecryptResponse(await resp.Content.ReadAsStringAsync());

                return JsonConvert.DeserializeObject<List<string>>(decrypted);
            }
        }
    }
}
