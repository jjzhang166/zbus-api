using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus.Remoting;

namespace zbus
{
    public class RemotingClientPoolConfig
    {
        public string brokerAddress = "127.0.0.1:15555";
        public int minSize = 5;
        public int maxSize = 20;
        public int clientLifeTimeInMinutes = 10;
    }

    public class RemotingClientPool : IDisposable
    {
        private string brokerAddress;
        private int minSize = 5;
        private int maxSize = 20;
        private int clientLifeTimeInMinutes = 10;
        
        private Queue<RemotingClient> clients = new Queue<RemotingClient>();

        public RemotingClientPool(RemotingClientPoolConfig config)
        {
            brokerAddress = config.brokerAddress;
            minSize = config.minSize;
            maxSize = config.maxSize;
            clientLifeTimeInMinutes = config.clientLifeTimeInMinutes;
        }

        public RemotingClient BorrowClient()
        {
            if (clients.Count > 0)
            {
                lock (clients)
                {
                    RemotingClient client = null;
                    while (clients.Count > 0)
                    {
                        client = clients.Dequeue();
                        if (client.IsConnected())
                        {
                            return client;
                        }
                        client.Close();
                    }
                }
            }
            return OpenClient();
        }


        public void ReturnClient(RemotingClient client)
        {
            if(client == null) return;
            lock (clients)
            {
                TimeSpan lifeTime = DateTime.Now.Subtract(client.TimeCreated);
                if (clients.Count < maxSize && lifeTime.Minutes < clientLifeTimeInMinutes)
                {
                    if (client.IsConnected())
                    {
                        clients.Enqueue(client);
                    }
                    else
                    {
                        client.Close();
                    }
                }
                else
                {
                    client.Close();
                }
            }

        }

        private RemotingClient OpenClient()
        {
            if (clients.Count > maxSize)
            {
                throw new Exception("RemotingClientPool reached its limit");
            }
            RemotingClient client = new RemotingClient(brokerAddress);
            return client;
        }

        public void Dispose()
        {
            while (clients.Count > 0)
            {
                RemotingClient client = clients.Dequeue();
                client.Close();
            }

        }
    }


}
