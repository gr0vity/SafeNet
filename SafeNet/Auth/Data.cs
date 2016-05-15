using System.Collections.Generic;

namespace SafeNet.Auth
{
    internal class Data
    {
        internal App app { get; set; }

        internal string publicKey { get; set; }

        internal string nonce { get; set; }

        internal List<string> permissions { get; set; }
    }
}