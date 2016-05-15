using System;
using System.IO;
using NUnit.Framework;
using System.Threading.Tasks;
using SafeNet.Auth;

namespace SafeNetTest
{
    [TestFixture]
    public class Auth
    {
        [Test]
        public async Task TestAuth()
        {
            var app = new AppDetails(
                "My Demo App",
                "0.0.1",
                "test",
                "org.test.me",
                permissions: new System.Collections.Generic.List<string> { "SAFE_DRIVE_ACCESS" }
                );

            var registration = await app.Register();


            var valid = await registration.Check();

            Assert.IsTrue(valid);
            
            await registration.SaveToFile(Common.TestFilePath); 
        }
    }
}