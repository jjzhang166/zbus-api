#ifndef __ZBOX_MSG_H_
#define __ZBOX_MSG_H_

#include "platform.h" 
#include "buffer.h"
#ifdef __cplusplus
extern "C" {
#endif

//����ĳ������嶼����Э�����Ҫ��ģ�ֻ�ǳ���Ӧ�ö���ģ�ZBUS�д������÷�������

#define HEARTBEAT          "heartbeat" //������Ϣ

//��׼HTTPͷ������
#define HEADER_CLIENT			"remote-addr"
#define HEADER_ENCODING			"content-encoding"
#define HEADER_CONTENT_TYPE		"content-type"

#define HEADER_CMD         "cmd"
#define HEADER_SUBCMD      "sub_cmd"
#define HEADER_BROKER      "broker"
#define HEADER_TOPIC       "topic"   //ʹ��,�ָ� 
#define HEADER_MQ_REPLY    "mq_reply"
#define HEADER_MQ          "mq"
#define HEADER_TOKEN       "token"
#define HEADER_MSGID       "msgid"	
#define HEADER_MSGID_RAW   "msgid_raw"	
#define HEADER_ACK         "ack"
#define HEADER_REPLY_CODE  "reply_code"	


typedef struct meta meta_t;
typedef struct msg msg_t;
typedef struct iobuf iobuf_t; 

 
ZBOX_EXPORT msg_t*msg_new (void); 
ZBOX_EXPORT void  msg_destroy (msg_t **self_p);
ZBOX_EXPORT char* msg_get_uri(msg_t* self);
ZBOX_EXPORT char* msg_get_path(msg_t* self); 
ZBOX_EXPORT char* msg_get_head (msg_t *self, char* key);
ZBOX_EXPORT void  msg_set_head (msg_t *self, char* key, char* value);
ZBOX_EXPORT char* msg_get_head_or_param(msg_t* self, char* key);
ZBOX_EXPORT void  msg_set_body_nocopy(msg_t* self, void* body, int len);
ZBOX_EXPORT void  msg_set_body_copy(msg_t* self, void* body, int len);
ZBOX_EXPORT void  msg_set_bodyfmt(msg_t* self, const char* format, ...);
ZBOX_EXPORT void  msg_set_body(msg_t* self, char* body);
ZBOX_EXPORT void  msg_set_json_body(msg_t* self, char* body);
ZBOX_EXPORT void* msg_get_body(msg_t* self);
ZBOX_EXPORT int   msg_get_body_len(msg_t* self);
ZBOX_EXPORT char* msg_get_status(msg_t* self);
ZBOX_EXPORT void  msg_set_status(msg_t* self, char* value); 
ZBOX_EXPORT int   msg_is_status200(msg_t* self);
ZBOX_EXPORT int   msg_is_status404(msg_t* self);
ZBOX_EXPORT int   msg_is_status500(msg_t* self);

//ZBUS�п����õ�һЩͷ����չ�����������չ������GET_HEAD�ı�ݷ�����װ
ZBOX_EXPORT char* msg_get_command(msg_t* self);
ZBOX_EXPORT void  msg_set_command(msg_t* self, char* value); 
ZBOX_EXPORT char* msg_get_subcmd(msg_t* self);
ZBOX_EXPORT void  msg_set_subcmd(msg_t* self, char* value); 
ZBOX_EXPORT char* msg_get_mq_reply(msg_t* self);
ZBOX_EXPORT void  msg_set_mq_reply(msg_t* self, char* value);
ZBOX_EXPORT char* msg_get_msgid(msg_t* self);
ZBOX_EXPORT void  msg_set_msgid(msg_t* self, char* value);
ZBOX_EXPORT char* msg_get_msgid_raw(msg_t* self);
ZBOX_EXPORT void  msg_set_msgid_raw(msg_t* self, char* value);
ZBOX_EXPORT bool  msg_is_ack(msg_t* self);
ZBOX_EXPORT void  msg_set_ack(msg_t* self, bool value);
ZBOX_EXPORT char* msg_get_mq(msg_t* self);
ZBOX_EXPORT void  msg_set_mq(msg_t* self, char* value);
ZBOX_EXPORT char* msg_get_token(msg_t* self);
ZBOX_EXPORT void  msg_set_token(msg_t* self, char* value);
ZBOX_EXPORT char* msg_get_topic(msg_t* self);
ZBOX_EXPORT void  msg_set_topic(msg_t* self, char* value);
ZBOX_EXPORT char* msg_get_encoding(msg_t* self);
ZBOX_EXPORT void  msg_set_encoding(msg_t* self, char* value);

ZBOX_EXPORT void   msg_encode(msg_t* self, buf_t* buf);
ZBOX_EXPORT msg_t* msg_decode(buf_t* buf);
ZBOX_EXPORT void   msg_print(msg_t* self);
ZBOX_EXPORT char*  msg_copy_body(msg_t* self);

#ifdef __cplusplus
}
#endif

#endif
