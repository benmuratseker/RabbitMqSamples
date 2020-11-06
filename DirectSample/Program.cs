using RabbitMQ.Client;
using System;
using System.Text;

namespace DirectSample
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

            channel.ExchangeDeclare("ex.direct", "direct", true, false, null);

            channel.QueueDeclare("my.info", true, false, false, null);
            channel.QueueDeclare("my.errors", true, false, false, null);
            channel.QueueDeclare("my.warnings", true, false, false, null);

            channel.QueueBind("my.info", "ex.direct", "info", null);
            channel.QueueBind("my.errors", "ex.direct", "error", null);
            channel.QueueBind("my.warnings", "ex.direct", "warning", null);

            channel.BasicPublish("ex.direct", "info", null, Encoding.UTF8.GetBytes("info message 1"));
            channel.BasicPublish("ex.direct", "error", null, Encoding.UTF8.GetBytes("error message 1"));
            channel.BasicPublish("ex.direct", "warning", null, Encoding.UTF8.GetBytes("warning message 1"));
        }
    }
}
