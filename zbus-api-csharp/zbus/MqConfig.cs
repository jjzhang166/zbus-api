using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus.Remoting;
namespace zbus
{
    public class MqConfig : ICloneable
    {
        public Broker broker;
        public string mq;
        public string accessToken = "";
        public string registerToken = "";
        public int mode = (int)MessageMode.MQ;
        public string topic = null;

        public void SetMode(params MessageMode[] modes)
        {
            foreach (MessageMode m in modes)
            {
                this.mode |= (int)m;
            }
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }
      

        public static void Main_MqConfig(string[] args)
        {
            MqConfig config = new MqConfig();
            MqConfig config2 = (MqConfig)config.Clone();
            config2.accessToken = "access";

            System.Console.ReadKey();
        }
    }




}
