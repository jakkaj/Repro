using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Foundation;
using NUnit.Framework;
using ReproTests.Model;
using ReproTests.Tests.Base;
using UIKit;
using XamlingCore.Portable.Data.Serialise;

namespace ReproTests.Tests
{
    [TestFixture]
    public class ReproTest : TestBase
    {
        [Test]
        public void ReproNoAsync()
        {
            var serialiser = Resolve<IEntitySerialiser>();
            var localSorage = Resolve<ILocalStorage>();
            var listOfEntities = new List<TestItem>();



            for (var i = 0; i < 250; i++)
            {
                var newEnt = new TestItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Person " + i.ToString(),
                    Time = DateTime.Now
                };

                listOfEntities.Add(newEnt);

                var serialised = serialiser.Serialise(listOfEntities);

                localSorage.SaveStringUTFNoAsync("testFile", serialised);

                var readFile = localSorage.LoadStringUTFNoAsync("testFile");

                Console.Write(i);

                if (serialised != readFile)
                {
                    Debug.WriteLine("Serialised: {0}", serialised);
                    Debug.WriteLine("Read: {0}", readFile);
                    Debugger.Break();
                }
                //Assert.AreEqual(serialised, readFile);
            }

        }

        [Test]
        public void ReproAsyncStream()
        {
            var serialiser = Resolve<IEntitySerialiser>();
            var localSorage = Resolve<ILocalStorage>();
            var listOfEntities = new List<TestItem>();


            var msr = new ManualResetEvent(false);

            Task.Run(async () =>
            {
                for (var i = 0; i < 250; i++)
                {
                    var newEnt = new TestItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Person " + i.ToString(),
                        Time = DateTime.Now
                    };

                    listOfEntities.Add(newEnt);

                    var serialised = serialiser.Serialise(listOfEntities);

                    await localSorage.SaveString("testFile4", serialised);

                    //await Task.Delay(3000);

                    var readFile = await localSorage.LoadString("testFile4");

                    Console.Write(i);

                    if (serialised != readFile)
                    {
                        Debug.WriteLine(string.Format("Serialised: {0}", serialised));
                        Debug.WriteLine(string.Format("Read: {0}", readFile));
                        Debugger.Break();
                    }
                    //Assert.AreEqual(serialised, readFile);
                }
                msr.Set();
            });

            var msrResult = msr.WaitOne(20000);
            Assert.IsTrue(msrResult, "MSR not set, means assertion failed in task");
        }
        [Test]
        public void ReproAsync()
        {
            var serialiser = Resolve<IEntitySerialiser>();
            var localSorage = Resolve<ILocalStorage>();
            var listOfEntities = new List<TestItem>();


            var msr = new ManualResetEvent(false);

            Task.Run(async () =>
           {
               for (var i = 0; i < 250; i++)
               {
                   var newEnt = new TestItem
                   {
                       Id = Guid.NewGuid(),
                       Name = "Person " + i.ToString(),
                       Time = DateTime.Now
                   };

                   listOfEntities.Add(newEnt);

                   var serialised = serialiser.Serialise(listOfEntities);

                   await localSorage.SaveStringUTF("testFile2", serialised);

                   //await Task.Delay(3000);

                   var readFile = await localSorage.LoadStringUTF("testFile2");

                   Console.Write(i);

                   if (serialised != readFile)
                   {
                       Debug.WriteLine("Serialised: {0}", serialised);
                       Debug.WriteLine("Read: {0}", readFile);
                       Debugger.Break();
                   }
                   //Assert.AreEqual(serialised, readFile);
               }
               msr.Set();
           });

            var msrResult = msr.WaitOne(20000);
            Assert.IsTrue(msrResult, "MSR not set, means assertion failed in task");
        }


        [Test]
        public void Repro()
        {
            var serialiser = Resolve<IEntitySerialiser>();
            var localSorage = Resolve<ILocalStorage>();
            var listOfEntities = new List<TestItem>();

            for (var i = 0; i < 250; i++)
            {
                var newEnt = new TestItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Person " + i.ToString(),
                    Time = DateTime.Now
                };

                listOfEntities.Add(newEnt);

                var serialised = serialiser.Serialise(listOfEntities);

                Task.Run(async () => await localSorage.SaveStringUTF("testFile3", serialised));

                var tResult = Task.Run(async () => await localSorage.LoadStringUTF("testFile3"));
                var readFile = tResult.Result;

                Console.Write("Count: {0}", i);

                if (serialised != readFile)
                {
                    Console.Write("Serialised: {0}", serialised);
                    Console.Write("Read: {0}", readFile);
                    Debugger.Break();
                }
                //Assert.AreEqual(serialised, readFile);
            }
        }

        public class TestItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime Time { get; set; }
        }
    }
}