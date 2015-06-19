using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus;


namespace zbus
{ 
    class RpcTest
    { 
        public static void Main(string[] args)
        {

            SingleBrokerConfig brokerConfig = new SingleBrokerConfig();
            brokerConfig.brokerAddress = "127.0.0.1:15555";
            Broker broker = new SingleBroker(brokerConfig);

            RpcConfig config = new RpcConfig();
            config.mq = "MyRpc";
            config.broker = broker;

            Rpc rpc = new Rpc(config);
            for (int i = 0; i < 100; i++)
            {
                object res = rpc.Invoke("stringArray");
                System.Console.WriteLine(res);
            }
            broker.Dispose();

            System.Console.ReadKey();
        }
    }
}
