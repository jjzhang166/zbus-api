#encoding=utf8 

from zbus import Kcxp, gen_kcxp_param_map, gen_kcxp_param
from zbus import SingleBroker
 

#创建一个Broker（管理链接，抽象zbus节点。 高可用版本可直接换HaBroker即可）
broker = SingleBroker(host='172.24.180.45', port=15555)
#broker = SingleBroker(host='172.24.178.175', port=15555)

kcxp = Kcxp(broker=broker, mq='KCXP') 

for i in range(10):
    params = gen_kcxp_param_map(func_no='L0063001', ip_address = '172.24.174.52')
    params["ORG_TYPE"] = "0"
    params["ORG_CODE"] = "1100" 
    res = kcxp.request_map(params) 
    print res


for i in range(10):
    params = gen_kcxp_param(func_no='L0063001', ip_address = '172.24.174.52')
    params.append("ORG_TYPE")
    params.append("0")
    params.append("ORG_CODE")
    params.append("1100")
    res = kcxp.request(params) 
    print res


