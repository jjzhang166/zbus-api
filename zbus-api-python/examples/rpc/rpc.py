#encoding=utf8
import sys 
sys.path.append('../../') 

from zbus import SingleBroker, Rpc 

broker = SingleBroker(host='127.0.0.1', port=15555)

rpc = Rpc(broker=broker, 
          mq='MyRpc2', 
          module='Interface',
          encoding='utf8',
          timeout=10)


print rpc.testEncoding()

broker.destroy()






