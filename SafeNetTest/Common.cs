using System;
using System.IO;

namespace SafeNetTest
{
    public static class Common
    {
        public static readonly string TestFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "safe_net_registration.json");
    }
}