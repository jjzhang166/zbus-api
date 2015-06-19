using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus;


namespace zbus
{
    class User
    {
        public string name;
        public string addr;
    }
    class MyService
    {
        [Remote]
        public string echo(string msg)
        {
            return msg;
        }

        [Remote]
        public int plus(int a, int b)
        {
            return a + b;
        }

        [Remote]
        public User user(string name)
        { 
            User user = new User();
            user.name = name;
            user.addr = "深圳";
            return user;
        }

    }
    class RpcService
    {
        public static void Main(string[] args)
        {
            SingleBrokerConfig config = new SingleBrokerConfig();
            config.brokerAddress = "127.0.0.1:15555";
            Broker broker = new SingleBroker(config);

            MyService mySerive = new MyService();
            RpcServiceHandler handler = new RpcServiceHandler(mySerive);
           

            ServiceConfig serviceConfig = new ServiceConfig(broker);
            serviceConfig.mq = "MyRpc";
            serviceConfig.serviceHandler = handler;

            Service service = new Service(serviceConfig);
            service.Start();
        } 
    }
}
