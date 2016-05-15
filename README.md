# SAFE.net a SAFE network client library written in C#

This is a partial implementation of the SAFE launcher API in C#. Pull requests are welcome and appreciated.

## Examples 

### App Authentication 


            var app = new AppDetails(
                "My Demo App",
                "0.0.1",
                "test",
                "org.test.me",
                permissions: new System.Collections.Generic.List<string>()
                );

            var registration = await app.Register();


            var valid = await registration.Check();

            Assert.IsTrue(valid);

            var unregistered = await registration.Unregister();

            Assert.IsTrue(unregistered);

## Work In progress, features to be implemented

All features from the official [launcher documentation](https://maidsafe.readme.io/)

* NFS
* DNS


