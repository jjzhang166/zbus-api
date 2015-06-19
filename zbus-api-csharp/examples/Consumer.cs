using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

using zbus.Remoting;
using zbus.Logging;
namespace zbus
{ 
    class ConsumerTest
    {
        public static void Main(string[] args)
        {

            SingleBrokerConfig config = new SingleBrokerConfig();
            config.brokerAddress = "127.0.0.1:15555";
            Broker broker = new SingleBroker(config);


            Consumer c = new Consumer(broker, "MyMQ");

            while (true)
            {
                Message msg = c.Recv(30000);
                if (msg == null) continue;

                System.Console.WriteLine(msg);
            }

            c.Dispose();
            broker.Dispose();
            Console.ReadKey();
        }
    }
}
