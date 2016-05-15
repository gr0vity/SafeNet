using System.Collections.Generic;

namespace SafeNet.Auth
{
    internal class Data
    {
        public App app { get; set; }

        public string publicKey { get; set; }

        public string nonce { get; set; }

        public List<string> permissions { get; set; }
    }
}