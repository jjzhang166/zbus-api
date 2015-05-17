#ifndef __ZBUSCONN_H__
#define __ZBUSCONN_H__

#include "remoting.h"  
#include "json.h"

#ifdef __cplusplus
extern "C" {
#endif
 
#define PRODUCE       "produce"     //������Ϣ
#define CONSUME       "consume"     //������Ϣ   
#define REQUEST       "request"     //����ȴ�Ӧ����Ϣ 
#define HEARTBEAT     "heartbeat"   //������Ϣ  
#define ADMIN         "admin"       //��������Ϣ
#define CREATE_MQ     "create_mq"   
#define TRACK_REPORT  "track_report" 
#define TRACK_SUB     "track_sub" 
#define TRACK_PUB     "track_pub"

#define MODE_MQ		  1
#define MODE_PUBSUB   2 
#define MODE_TEMP     4

typedef msg_t* (service_handler)(msg_t* req, void* privdata);
typedef void   (msg_callback)(msg_t* msg, void* privdata);
typedef struct service_cfg{
	char  broker[256];
	char  mq[256];
	char  acc_token[256];
	char  reg_token[256];
	int   thread_count;
	int   consume_timeout;
	int   reconnect_interval;
	service_handler* handler;
}service_cfg_t;



typedef struct producer producer_t;
typedef struct consumer consumer_t;
typedef struct caller caller_t;

typedef struct rpc rpc_t;

ZBOX_EXPORT producer_t* producer_new(rclient_t* client, char* mq, int mode);
ZBOX_EXPORT void producer_destroy(producer_t** self_p);
ZBOX_EXPORT void producer_set_token(producer_t* self, char* token);
ZBOX_EXPORT int  producer_send(producer_t* self, msg_t* msg, msg_t** result_p, int timeout);

ZBOX_EXPORT consumer_t* consumer_new(rclient_t* client, char* mq, int mode);
ZBOX_EXPORT void consumer_destroy(consumer_t** self_p);
ZBOX_EXPORT void consumer_set_acc_token(consumer_t* self, char* value);
ZBOX_EXPORT void consumer_set_reg_token(consumer_t* self, char* value);
ZBOX_EXPORT void consumer_set_topic(consumer_t* self, char* value);
ZBOX_EXPORT int  consumer_recv(consumer_t* self, msg_t** result_p, int timeout);
ZBOX_EXPORT int  consumer_reply(consumer_t* self, msg_t* msg);

ZBOX_EXPORT caller_t* caller_new(rclient_t* client, char* mq);
ZBOX_EXPORT void      caller_destroy(caller_t** self_p);
ZBOX_EXPORT void      caller_set_token(caller_t* self, char* value);
ZBOX_EXPORT void      caller_set_encoding(caller_t* self, char* value);
ZBOX_EXPORT int       caller_invoke(caller_t* self, msg_t* request, msg_t** result_p, int timeout);

ZBOX_EXPORT rpc_t* rpc_new(rclient_t* client, char* mq);
ZBOX_EXPORT void   rpc_destroy(rpc_t** self_p);
ZBOX_EXPORT rpc_t* rpc_set_token(rpc_t* self, char* value);
ZBOX_EXPORT int	   rpc_invoke(rpc_t* self, char* method, json_t* params, json_t** result_p, int timeout);
ZBOX_EXPORT int    rpc_call(rpc_t* self, json_t* request, json_t** result_p, int timeout);

ZBOX_EXPORT service_cfg_t* service_cfg_new();
ZBOX_EXPORT void		   service_cfg_destroy(service_cfg_t** self_p);
ZBOX_EXPORT void*          service_serve(void* args);//args: 


ZBOX_EXPORT msg_t* pack_json_request(json_t* request);
ZBOX_EXPORT json_t* unpack_json_object(msg_t* msg);

#ifdef __cplusplus
}
#endif

#endif
