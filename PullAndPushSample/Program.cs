using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace PullAndPushSample
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            //ReadMessagesWithPushModel();
            ReadMessagesWithPullModel();

            channel.Close();
            conn.Close();
        }

        private static void ReadMessagesWithPushModel()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Message:" + message);
            };

            string consuerTag = channel.BasicConsume("my.queue1", true, consumer);

            Console.WriteLine("Subscribed. Pşress a key to unsubscribe and exit");
            Console.ReadKey();

            channel.BasicCancel(consuerTag);
        }

        private static void ReadMessagesWithPullModel()
        {
            Console.WriteLine("Reading messages from queue. Press 'e' to exit");

            while (true)
            {
                Console.WriteLine("Trying to get a message from the queue...");

                BasicGetResult result = channel.BasicGet("my.queue1", true);
                if (result != null)
                {
                    string message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine("Message:" + message);
                }

                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'e' || keyInfo.KeyChar == 'E')
                        return;
                }

                Thread.Sleep(2000);
            }
        }
    }
}