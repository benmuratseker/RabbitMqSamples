using RabbitMQ.Client;
using System;
using System.Text;

namespace TopicSample
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

            channel.ExchangeDeclare("ex.topic", "topic", true, false, null);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            channel.QueueDeclare("my.queue3", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.topic", "*.image.*");
            channel.QueueBind("my.queue2", "ex.topic", "#.image");
            channel.QueueBind("my.queue3", "ex.topic", "image.#");//* bir kelime, # bir veya birden fazla kelime

            //topic: routekey uyumluluğuna gre queue ya aktarım yapar
            channel.BasicPublish("ex.topic", "convert.image.jpeg", null, Encoding.UTF8.GetBytes("Routing key is convert.image.jpeg"));
            channel.BasicPublish("ex.topic", "convert.bitmap.image", null, Encoding.UTF8.GetBytes("Routing key is convert.bitmap.image"));
            channel.BasicPublish("ex.topic", "convert.bitmap.32bit", null, Encoding.UTF8.GetBytes("Routing key is convert.bitmap.bmp"));
        }
    }
}
