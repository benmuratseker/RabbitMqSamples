using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlternateExchangeSample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
            channel.ExchangeDeclare(
                "ex.direct",
                "direct",
                true,
                false,
                new Dictionary<string, object>()
                {
                    {"alternate-exchange","ex.fanout" }
                });

            channel.QueueDeclare("queue1", true, false,false, null);
            channel.QueueDeclare("queue2", true, false, false, null);
            channel.QueueDeclare("unrouted.queue", true, false, false, null);

            channel.QueueBind("queue1", "ex.direct", "video");
            channel.QueueBind("queue2", "ex.direct", "image");
            channel.QueueBind("unrouted.queue", "ex.fanout", "");

            channel.BasicPublish("ex.direct", "video", null, Encoding.UTF8.GetBytes("Message with routing key video"));

            channel.BasicPublish("ex.direct", "text", null, Encoding.UTF8.GetBytes("Message with routing key text"));//var olmayan routekey nedeniyle exfanout bunu unrouted queue ya yönlendirir
        }
    }
}
