using GreenhouseController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class DataConsumerTests
    {
        [TestMethod]
        public void TestReceiveGreenhouseData()
        {
            // Make the fake collection
            BlockingCollection<byte[]> mockCollection = new BlockingCollection<byte[]>();

            // Add stuff to the fake collection
            for (int i = 0; i < 6; i++)
            {
                mockCollection.Add(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new DataPacket())));
            }

            // Take stuff from the fake collection
            while(mockCollection.Count != 0)
            {
                DataConsumer.Instance.ReceiveGreenhouseData(mockCollection);
            }

            // Make sure stuff was taken from the collection
            Assert.IsTrue(mockCollection.Count == 0);
        }
    }
}
