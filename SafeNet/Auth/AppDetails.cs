using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SafeNet.Exceptions;

namespace SafeNet.Auth
{
    public class AppDetails
    {
        public string Name { get; }

        public string Version { get; }

        public string Vendor { get; }

        public string Id { get; }

        public List<string> Permissions { get; }

        private App App => new App
        {
            name = Name,
            version = Version,
            vendor = Vendor,
            id = Id,
        };

        private Data GetData(byte[] publicKey, byte[] nonce)
        {
            return new Data
            {
                app = App,
                publicKey = Convert.ToBase64String(publicKey),
                nonce = Convert.ToBase64String(nonce),
                permissions = Permissions
            };
        }

        public AppDetails(string name, string version, string vendor, string id, List<string> permissions)
        {
            Name = name;
            Version = version;
            Vendor = vendor;
            Id = id;
            Permissions = permissions;
        }

        /// <summary>
        /// implementation of https://maidsafe.readme.io/docs/auth
        /// 
        /// An application exchanges data with the SAFE Launcher using symmetric key encryption. The symmetric key is session based and is securely transferred from the SAFE Launcher to the application using ECDH Key Exchange. Applications will generate an asymmetric key pair and a nonce for ECDH Key Exchange with the SAFE Launcher.
        /// The application will initiate the authorisation request with the generated nonce and public key, along with information about the application and the required permissions.The SAFE Launcher will prompt to the user with the application information along with the requested permissions. Once the user authorises the request, the symmetric keys for encryption are received.If the user denies the request then the SAFE Launcher sends an unauthorised error response.
        /// </summary>
        /// <param name="appDetails"></param>
        /// <returns></returns>
        public async Task<SafeRegisterResponse> Register()
        {
            var keypair = Sodium.PublicKeyBox.GenerateKeyPair();
            var nonce = Sodium.PublicKeyBox.GenerateNonce();
            var data = GetData(keypair.PublicKey, nonce);
            var payload = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Auth.LAUNCHER_BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var resp = await client.PostAsync("auth", new StringContent(payload, Encoding.UTF8, "application/json"));
                var content = resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    throw new AuthException(resp);
                }

                var launcherResponse = JsonConvert.DeserializeObject<LauncherResponseData>(await content);

                // This is the encrypted symmetric key and nonce Launcher has passed us, duely encrypted
                // with the Asymmetric keys we gave it earlier so that no one can snoop on it. Convert from
                // base64 encoded String.
                var encryptedSymmetricKeyNonce = Convert.FromBase64String(launcherResponse.encryptedKey);

                // This is Launcher's Public Asymmetric Key - We will use this for decrypting the above.
                var launcherPublicKey = Convert.FromBase64String(launcherResponse.publicKey);

                var decryptedSymmetricKeyNonce = Sodium.PublicKeyBox.Open(encryptedSymmetricKeyNonce, nonce, keypair.PrivateKey, launcherPublicKey);



                return new SafeRegisterResponse(
                    launcherResponse.token
                    , decryptedSymmetricKeyNonce
                    , decryptedSymmetricKeyNonce
                    );
            }
        }
    }
}