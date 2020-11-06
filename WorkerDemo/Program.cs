using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace WorkerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the name for this worker:");
            string workerName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
             {
                 string message = Encoding.UTF8.GetString(e.Body.ToArray());
                 Console.WriteLine($"[{workerName}] Message:{message}");
             };

            var consumerTag = channel.BasicConsume("my.queue1", true, consumer);

            Console.WriteLine("Waiting for messages. Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            conn.Close();
        }
    }
}
