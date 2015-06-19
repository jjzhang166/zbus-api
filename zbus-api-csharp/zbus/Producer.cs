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
    }
}
