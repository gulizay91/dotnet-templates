
## sample RMQSettings
```
"RabbitMQSettings": {
    "Hostname": "${RABBITMQ_HOSTNAME}",
    "Username": "${RABBITMQ_USERNAME}",
    "Password": "${RABBITMQ_PASSWORD}",
    "Consumers": {
      "XConsumer": {
        "ExchangeType": "fanout",
        "ExchangeName": "a-service.xContract-exchange",
        "QueueName": "a-service.xContract",
        "HasDlq": true,
        "Durable": true,
        "RetryMaxCount": 3,
        "RetryDelaySecond": 5
      }
    }
  },
```
