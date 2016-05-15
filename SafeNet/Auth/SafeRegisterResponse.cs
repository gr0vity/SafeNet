using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sodium;

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

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SafeRegisterResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SafeRegisterResponse>(json);
        }

        public async Task SaveToFile(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                await writer.WriteAsync(ToJson());
            }
        }

        public static async Task<SafeRegisterResponse> ReadFromFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return FromJson(await reader.ReadToEndAsync());
            }
        }

        public string EncryptPayload(string payload)
        {
            return Convert.ToBase64String(SecretBox.Create(payload, SymmetricNonce, SymmetricKey));
        }

        public string DecryptResponse(string response)
        {
            var bytes = Convert.FromBase64String(response);
            var decrypted = SecretBox.Open(bytes, SymmetricNonce, SymmetricKey);

            return Encoding.UTF8.GetString(decrypted);
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
            using (var client = Client)
            {
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
            using (var client = Client)
            {
                var launcherResponse = await client.DeleteAsync("auth");
                return launcherResponse.StatusCode == HttpStatusCode.OK;
            }
        }


        /// <summary>
        /// configured HttpClient with auth header and base url set
        /// 
        /// </summary>
        [JsonIgnore]
        public HttpClient Client
        {
            get
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(Auth.LAUNCHER_BASE_URL);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                return client;
            }
        }
    }
}