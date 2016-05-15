using System.Collections.Generic;

namespace SafeNet.Auth
{
    internal class LauncherResponseData
    {
        public string token { get; set; }

        public string encryptedKey { get; set; }

        public string publicKey { get; set; }

        public List<string> permissions { get; set; }
    }
}