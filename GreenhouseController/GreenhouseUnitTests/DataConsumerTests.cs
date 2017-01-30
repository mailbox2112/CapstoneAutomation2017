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
            BlockingCollection<byte[]> mockCollection = new BlockingCollection<byte[]>();

            for (int i = 0; i < 6; i++)
            {
                mockCollection.Add(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new DataPacket())));
            }
        }
    }
}
