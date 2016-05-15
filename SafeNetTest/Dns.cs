using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeNet.Auth;
using SafeNet.Dns;

namespace SafeNetTest
{
    [TestFixture]
    public static class Dns
    {
        public static async Task<SafeRegisterResponse> GetTestRegistration()
        {
            return await SafeRegisterResponse.ReadFromFile(Common.TestFilePath); 
        }

        [Test]
        public static async Task DnsCreate()
        {
            var reg = await GetTestRegistration();

            Assert.IsTrue(await reg.DnsCreateLongName(DateTime.Now.ToOADate().ToString().Replace(".", "_"))); 
        }

        [Test]
        public static async Task DnsList()
        {
            var reg = await GetTestRegistration();

            foreach (var name in await reg.DnsListLongNames())
            {
                Console.WriteLine(name); 
            }
        }
    }
}