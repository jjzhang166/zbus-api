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
    public class Consumer : MqAdmin, IDisposable
    {
        private static readonly ILogger log = LoggerFactory.GetLogger(typeof(Consumer));
        private RemotingClient client = null;
        private string topic = null;
        
        public Consumer(Broker broker, String mq, params MessageMode[] modes)
            :base(broker, mq, modes)
        {
        }

        public Consumer(MqConfig config)
            :base(config)
        {
            this.topic = config.topic;
        }

        public Message Recv(int timeout)
        {
            if (this.client == null)
            {
                this.client = broker.GetClient(GetClientHint());
            }
            Message req = new Message();
            req.Command = Proto.Consume;
            req.Mq = this.mq;
            req.Token = this.accessToken;
            if ((this.mode & (int)MessageMode.PubSub) != 0)
            {
                if (this.topic != null)
                {
                    req.Topic = this.topic;
                }
            }
            try { 

                Message res = this.broker.InvokeSync(req, timeout);
                if (res != null && res.IsStatus404())
                {
                    if (!this.CreateMQ())
                    {
                        throw new ZbusException("register error");
                    }
                    return Recv(timeout);
                }

                return res;
            }
            catch (IOException ex)
            {
                if (Environment.Version.Major < 4) //.net 3.5 socket sucks!!!!
                {
                    this.HandleFailover();
                }
                else 
                {
                    if (!ex.Message.Contains("period of time")) //timeout just ignore
                    {
                        this.HandleFailover();
                    } 
                }
                
            } 

            return null;

        }

        public void Reply(Message msg, int timeout)
        {
            msg.SetHead(Message.HEADER_REPLY_CODE, msg.Status);
            msg.Command = Proto.Produce;
            msg.Ack = false;
            this.client.Send(msg, timeout);
        }


        private void HandleFailover()
        {
            try
            {
                broker.CloseClient(this.client);
                this.client = broker.GetClient(GetClientHint());
            }
            catch (IOException ex)
            {
                log.Error(ex.Message, ex);
            }
            
        }

        public void Dispose()
        {
            if (this.client != null)
            {
                broker.CloseClient(this.client);
            }
        }
    }
}
