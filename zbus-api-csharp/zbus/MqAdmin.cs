using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using zbus.Remoting;
namespace zbus
{
    public class MqAdmin{     
	    protected readonly Broker broker;      
	    protected String mq;                  //队列唯一性标识
	    protected String accessToken = "";    //访问控制码
	    protected String registerToken = "";  //注册认证码  
	    protected int mode; 
	    protected int invokeTimeout = 2500;


        public MqAdmin(Broker broker, String mq,  params MessageMode[] modes){
            this.broker = broker;
            this.mq = mq;
            if (modes.Length == 0)
            {
                this.mode = (int)MessageMode.MQ;
            }
            else
            {
                foreach (MessageMode m in modes)
                {
                    this.mode |= (int)m;
                }
            }
        }

        public MqAdmin(MqConfig config)
        {
            this.broker = config.broker;
            this.mq = config.mq;
            this.accessToken = config.accessToken;
            this.registerToken = config.registerToken;
            this.mode = config.mode;
        } 

        public bool CreateMQ()
        {
            Dictionary<string, string> args = new Dictionary<string,string>();
            args["mqName"] = mq;
            args["accessToken"] = accessToken;
            args["mqMode"] = ""+mode;

            Message req = Proto.BuildSubCommandMessage(Proto.Admin, Proto.AdminCreateMQ,args);
            req.Token = registerToken;
            req.Mq = mq;
            
            Message res = InvokeCreateMQ(req);
            if (res == null)
            {
                return false;
            }
            return res.IsStatus200();
        }

        protected Message InvokeCreateMQ(Message req)
        {
            return broker.InvokeSync(req, invokeTimeout);
        }

        protected ClientHint GetClientHint()
        {
            ClientHint hint = new ClientHint();
            hint.Mq = mq;
            return hint;
        }
    }
	
}
