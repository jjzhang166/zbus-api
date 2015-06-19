using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using fastJSON;
using System.Reflection;
using System.Collections;
using zbus.Logging;
using zbus.Remoting;
namespace zbus
{
    public interface ServiceHandler
    {
        Message HandleRequest(Message request);
    }

    public class Remote : Attribute
    {
        private string id;

        public Remote(string id)
        {
            this.id = id;
        }

        public Remote()
        {
            this.id = null;
        }

        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
    }


    class MethodInstance
    {
        private MethodInfo method;
        private object instance;

        public MethodInstance(MethodInfo method, object instance)
        {
            this.method = method;
            this.instance = instance;
        }

        public MethodInfo Method
        {
            get { return this.method; }
            set { this.method = value; }
        }

        public object Instance
        {
            get { return this.instance; }
            set { this.instance = value; }
        }
    }


    public class RpcServiceHandler : ServiceHandler
    {

        private Encoding encoding;
        private Dictionary<string, MethodInstance> methods = new Dictionary<string, MethodInstance>();

        public RpcServiceHandler(params object[] services)
        {
            this.Init(Encoding.UTF8, services);
        }

        public RpcServiceHandler(Encoding encoding, params object[] services)
        {
            this.Init(encoding, services);
        }

        private void Init(Encoding encoding, params object[] services)
        {
            this.encoding = encoding;
            foreach (object service in services)
            {
                this.InitCommandTable(service);
            }
        }

        private void InitCommandTable(object service)
        {
            List<Type> types = new List<Type>();
            types.Add(service.GetType());
            foreach (Type type in service.GetType().GetInterfaces())
            {
                types.Add(type);
            }
            foreach (Type type in types)
            {
                foreach (MethodInfo info in type.GetMethods())
                {
                    foreach (Attribute attr in Attribute.GetCustomAttributes(info))
                    {
                        if (attr.GetType() == typeof(Remote))
                        {
                            Remote r = (Remote)attr;
                            string id = r.Id;
                            if (id == null)
                            {
                                id = info.Name;
                            }
                            if (this.methods.ContainsKey(id))
                            {
                                Console.WriteLine("{0} overridden", id);
                                break;
                            }

                            MethodInstance instance = new MethodInstance(info, service);
                            this.methods[id] = instance;
                            break;
                        }
                    }
                }
            }
        }

        public Message HandleRequest(Message request)
        {
            string json = request.GetBody();

            System.Exception error = null;
            object result = null;
             
            string method = null;
            ArrayList args = null;

            MethodInstance target = null;

            Dictionary<string, object> parsed = null;
            try
            {
                parsed = (Dictionary<string, object>)JSON.Instance.Parse(json);
            }
            catch (System.Exception ex)
            {
                error = ex;
            }
            if (error == null)
            {
                try
                { 
                    method = (string)parsed["method"];
                    args = (ArrayList)parsed["params"];
                }
                catch (System.Exception ex)
                {
                    error = ex;
                } 
                if (method == null)
                {
                    error = new ZbusException("missing method name");
                }
            }

            if (error == null)
            {
                if (this.methods.ContainsKey(method))
                {
                    target = this.methods[method];
                }
                else
                {
                    error = new ZbusException(method + " not found");
                }
            }

            if (error == null)
            {
                try
                {
                    ParameterInfo[] pinfo = target.Method.GetParameters();
                    if (pinfo.Length == args.Count)
                    {
                        object[] paras = new object[args.Count];
                        for (int i = 0; i < pinfo.Length; i++)
                        {
                            paras[i] = System.Convert.ChangeType(args[i], pinfo[i].ParameterType);
                        }
                        result = target.Method.Invoke(target.Instance, paras);
                    }
                    else
                    {
                        error = new ZbusException("number of argument not match");
                    }
                }
                catch (System.Exception ex)
                {
                    error = ex;
                }
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (error == null)
            {
                data["error"] = null;
                data["result"] = result;
            }
            else
            {
                data["error"] = error.Message;
                data["result"] = null;
            }

            string resJson = JSON.Instance.ToJSON(data);
            Message res = new Message();
            res.SetBody(resJson);

            return res;
        }
    }



    public class ServiceConfig : MqConfig
    {
        public ServiceHandler serviceHandler;
        public int consumerCount = 1; 
        public int readTimeout = 30000; //30ms
        private Broker[] brokers;
        

        public ServiceConfig(params Broker[] brokers)
        {
            this.brokers = brokers;
            if (this.brokers.Length > 0)
            {
                this.broker = this.brokers[0];
            }
        }

        public Broker[] GetBrokers()
        {
            if (this.brokers == null || this.brokers.Length == 0)
            {
                if (this.broker != null)
                {
                    this.brokers = new Broker[] { this.broker };
                }
            }
            return this.brokers;
        }
    }

    class ConsumerThread : IDisposable
    {
        private Thread thread = null;
        private ConsumerThreadStart threadStart = null;


        public ConsumerThread(ConsumerThreadStart threadStart)
        {
            this.threadStart = threadStart;
            this.thread = new Thread(this.threadStart.Run);
        }

        public void Start()
        {
            this.thread.Start();
        }

        public void Dispose()
        {
            try
            {
                this.thread.Interrupt();
            }
            finally
            {
                this.threadStart.Dispose();
            }
            
        }
    }

    class ConsumerThreadStart : IDisposable
    {
        private static readonly ILogger log = LoggerFactory.GetLogger(typeof(ConsumerThreadStart));
        private ServiceConfig config;
        private Consumer consumer;
        public ConsumerThreadStart(ServiceConfig config)
        {
            this.config = config; 
        }

        public void Run()
        {
            this.consumer = new Consumer(config);
            int readTimeout = config.readTimeout;
            ServiceHandler handler = config.serviceHandler;
            while (true) 
            {
                try
                {
                    Message req = consumer.Recv(readTimeout);
                    if (req == null) continue;

                    string mqReply = req.MqReply;
                    string msgId = req.MsgIdRaw;

                    Message res = handler.HandleRequest(req);
                    if (res != null)
                    {
                        res.Mq = mqReply;
                        res.MsgId = msgId;
                        consumer.Reply(res, readTimeout);
                    }
                } 
                catch(System.Exception e)
                {
                    log.Error(e.Message, e);
                }
            }
        }

        public void Dispose()
        {
            if (this.consumer != null)
            {
                this.consumer.Dispose();
            }
        }
    }


    public class Service : IDisposable
    {
        private static readonly ILogger log = LoggerFactory.GetLogger(typeof(Service));
        private ServiceConfig config;
        private ConsumerThread[][] brokerConsumerThreads;
        public Service(ServiceConfig config)
        {
            this.config = config; 
        }

        public void Start()
        {
            Broker[] brokers = config.GetBrokers();
            int consumerCount = config.consumerCount;
            if (brokers.Length < 1 || consumerCount < 1) return;

            this.brokerConsumerThreads = new ConsumerThread[brokers.Length][];
            for (int i = 0; i < brokers.Length; i++)
            {
                ConsumerThread[] threads = new ConsumerThread[consumerCount];
                this.brokerConsumerThreads[i] = threads;
                for (int j = 0; j < consumerCount; j++)
                {
                    ServiceConfig myConfig = (ServiceConfig)this.config.Clone();
                    myConfig.broker = brokers[i];
                    
                    ConsumerThreadStart start = new ConsumerThreadStart(myConfig);
                    ConsumerThread thread = new ConsumerThread(start);
                    thread.Start();
                    threads[j] = thread;
                }
            }

            log.InfoFormat("Service({0}) started", config.mq);
        }

        public void Dispose()
        {
            if (this.brokerConsumerThreads != null)
            {
                for (int i = 0; i < brokerConsumerThreads.Length; i++ )
                {
                    ConsumerThread[] threads = brokerConsumerThreads[i];
                    for (int j = 0; j < threads.Length; j++)
                    {
                        ConsumerThread thread = threads[j];
                        thread.Dispose();
                    }
                }
            }
        }  
    } 
        

}
