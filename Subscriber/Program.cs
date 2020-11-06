using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the queue name:");
            string queueName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.UserName = "guest";
            factory.Password = "guest";

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Subscriber [{queueName}] Message: {message}");
            };

            var consumerTag = channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine("Subscribed to the queue '{queueName}'. Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            conn.Close();
        }
    }
}
