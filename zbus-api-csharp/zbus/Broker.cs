using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus.Remoting;
namespace zbus
{
    public class ClientHint
    {
        public String Mq = "";
	    public String Broker = "";
    }

    public interface Broker : IDisposable
    {
        RemotingClient GetClient(ClientHint hint);
        void CloseClient(RemotingClient client);
        Message InvokeSync(Message req, int timeout);
    }


    public class SingleBrokerConfig : RemotingClientPoolConfig
    {
    }

    public class SingleBroker : Broker
    {
        private RemotingClientPool pool;
        public SingleBroker(SingleBrokerConfig config)
        {
            pool = new RemotingClientPool(config);
        }
        public RemotingClient GetClient(ClientHint hint)
        {
            return pool.BorrowClient() ;
        }

        public void CloseClient(RemotingClient client)
        {
            pool.ReturnClient(client);
        }

        public Message InvokeSync(Message req, int timeout)
        {
            RemotingClient client = null;
            try
            {
                client = pool.BorrowClient();
                return client.Invoke(req, timeout);
            }
            finally
            {
                if (client != null)
                {
                    pool.ReturnClient(client);
                }
            } 
        }

        public void Dispose()
        {
            if (pool != null)
            {
                pool.Dispose();
            }
        }
    }



}
