using RabbitMQ.Client;
using System;
using System.Text;

namespace ExchangeToExchangeSample
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

            channel.ExchangeDeclare("exchange1", "direct", true, false, null);
            channel.ExchangeDeclare("exchange2", "direct", true, false, null);

            channel.QueueDeclare("queue1", true, false, false, null);
            channel.QueueDeclare("queue2", true, false, false, null);

            channel.QueueBind("queue1", "exchange1", "key1");
            channel.QueueBind("queue2", "exchange2", "key2");

            channel.ExchangeBind("exchange2", "exchange1", "key2");

            channel.BasicPublish("exchange1", "key1", null, Encoding.UTF8.GetBytes("Message with key key1"));
            channel.BasicPublish("exchange1", "key2", null, Encoding.UTF8.GetBytes("Message with key key2"));
            //key2 kuyruğu exchange1 e bağlı olmasa bile exchange2 üzerinden 2 nolu kuyruğa ulaşmış olur
        }
    }
}
