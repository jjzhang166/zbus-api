using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus.Remoting;


namespace zbus
{
    public class Producer: MqAdmin
    {
        public Producer(Broker broker, String mq, params MessageMode[] modes)
            :base(broker, mq, modes)
        {
        }

        public Producer(MqConfig config)
            :base(config)
        {
        }
      

        public Message Send(Message msg, int timeout)
        {
            msg.Command = Proto.Produce;
            msg.Mq = this.mq;
            msg.Token = this.accessToken;

            return this.broker.InvokeSync(msg, timeout);
        }

        public static void Main_Producer(string[] args)
        {
            SingleBrokerConfig config = new SingleBrokerConfig();
            config.brokerAddress = "127.0.0.1:15555";
            Broker broker = new SingleBroker(config);

            Producer producer = new Producer(broker, "MyMQ", MessageMode.MQ);
            producer.CreateMQ();

            Message msg = new Message();
            msg.Topic = "qhee";
            msg.SetBody("hello world from C# {0}", DateTime.Now);
            msg = producer.Send(msg, 10000); 

            Console.ReadKey();
        }
    }
}
