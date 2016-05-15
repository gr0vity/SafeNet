namespace SafeNet.Dns
{
    internal class RegisterService
    {
        public string longName { get; }

        public string serviceName { get; }

        public string serviceHomeDirPath { get; }

        public bool isPathShared { get; }

        public RegisterService(string longName, string serviceName, string serviceHomeDirPath, bool isPathShared)
        {
            this.longName = longName;
            this.serviceName = serviceName;
            this.serviceHomeDirPath = serviceHomeDirPath;
            this.isPathShared = isPathShared;
        }
    }
}