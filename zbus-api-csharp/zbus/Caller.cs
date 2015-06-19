using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zbus.Remoting;

using fastJSON;
namespace zbus
{
    public class Caller : MqAdmin
    {
        public Caller(Broker broker, String mq, params MessageMode[] modes)
            :base(broker, mq, modes)
        {
        }

        public Caller(MqConfig config)
            :base(config)
        {
        }

        public Message Invoke(Message msg, int timeout)
        {
            msg.Command = Proto.Request;
            msg.Mq = this.mq;
            msg.Token = this.accessToken; 

            return this.broker.InvokeSync(msg, timeout);
        }

        public static void Main_Caller(string[] args)
        {
            SingleBrokerConfig config = new SingleBrokerConfig();
            config.brokerAddress = "127.0.0.1:15555";
            Broker broker = new SingleBroker(config);


            Caller c = new Caller(broker, "MyService");
            for (int i = 0; i < 1; i++)
            {
                Message msg = new Message();
                msg.SetBody("hello from C#");
                Message res = c.Invoke(msg, 2500);
                Console.WriteLine(res);
            }

            broker.Dispose();
            Console.ReadKey();
        }
    }

    public class RpcConfig : MqConfig
    {
        public static readonly string DEFAULT_ENCODING = "UTF-8";
        public string module = "";
        public int timeout = 10000;
        public string encoding = DEFAULT_ENCODING;
    }


    public class Rpc : Caller
    {
        private String module = "";
        private String encoding = RpcConfig.DEFAULT_ENCODING;
        private int timeout = 10000;  

        public Rpc(Broker broker, String mq, params MessageMode[] modes)
            :base(broker, mq, modes)
        {
        }

        public Rpc(RpcConfig config)
            :base(config)
        {
            this.module = config.module;
            this.encoding = config.encoding;
            this.timeout = config.timeout;
        }

        public object Invoke(string method, params object[] args)
        {
            IDictionary<string, object> req = new Dictionary<string, object>();
            req["module"] = this.module;
            req["method"] = method;
            req["params"] = args;
            req["encoding"] = this.encoding;

            Message msgReq = new Message();
            string json = JSON.Instance.ToJSON(req);
            msgReq.SetJsonBody(json);

            Message msgRes = base.Invoke(msgReq, this.timeout);
            string encodingName = msgRes.Encoding;
            Encoding encoding = Encoding.Default;
            if(encodingName != null){
                encoding = Encoding.GetEncoding(encodingName);
            }
            string jsonString = msgRes.GetBody(encoding);
            Dictionary<string, object> jsonRes = (Dictionary<string, object>)JSON.Instance.Parse(jsonString);
            if (jsonRes.Keys.Contains("result"))
            {
                return jsonRes["result"];
            }

            if (jsonRes.Keys.Contains("error"))
            {
                throw new ZbusException((string)jsonRes["error"]);
            }
            throw new ZbusException("return format error");

        }

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
