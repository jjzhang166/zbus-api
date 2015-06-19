using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus.Remoting;


namespace zbus
{
    public class ProducerTest
    {
        public static void Main(string[] args)
        {
            SingleBrokerConfig config = new SingleBrokerConfig();
            config.brokerAddress = "127.0.0.1:15555";
            Broker broker = new SingleBroker(config);

            Producer producer = new Producer(broker, "MyMQ", MessageMode.MQ);
            producer.CreateMQ();

            Message msg = new Message(); 
            msg.SetBody("hello world from C# {0}", DateTime.Now);
            msg = producer.Send(msg, 10000);

            broker.Dispose();
            Console.ReadKey();
        }
    }
}
