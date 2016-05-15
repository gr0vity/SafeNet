using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SafeNet.Auth
{
    public class SafeRegisterResponse
    {
        public string Token { get; }

        public byte[] SymmetricKey { get; }

        public byte[] SymmetricNonce { get; }

        public SafeRegisterResponse(string token, byte[] symmetricKey, byte[] symmetricNonce)
        {
            Token = token;
            SymmetricKey = symmetricKey;
            SymmetricNonce = symmetricNonce;
        }

        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/is-token-valid
        /// 
        /// Applications can cache the tokens and keys to avoid requiring regular access requests via the Launcher. 
        /// The applications can invoke this API to confirm whether the obtained token is still valid.
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        public async Task<bool> Check()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Auth.LAUNCHER_BASE_URL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                var launcherResponse = await client.GetAsync("auth");
                return launcherResponse.StatusCode == HttpStatusCode.OK;
            }
        }

        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/revoke-token
        /// 
        /// Removes the token from the SAFE Launcher.
        /// 
        /// Applications can invalidate the token with the Launcher. The Authorization header must be present in the request.
        /// </summary>
        /// <param name="safeRegisterResponse"></param>
        /// <returns></returns>
        public async Task<bool> Unregister()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Auth.LAUNCHER_BASE_URL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                var launcherResponse = await client.DeleteAsync("auth");
                return launcherResponse.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}