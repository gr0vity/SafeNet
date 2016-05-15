using System.Collections.Generic;

namespace SafeNet.Auth
{
    internal class LauncherResponseData
    {
        internal string token { get; set; }

        internal string encryptedKey { get; set; }

        internal string publicKey { get; set; }

        internal List<string> permissions { get; set; }
    }
}