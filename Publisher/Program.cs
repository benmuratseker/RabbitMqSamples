using RabbitMQ.Client;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.UserName = "guest";
            factory.Password = "guest";

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            while (true)
            {
                Console.Write("Enter message:");
                string message = Console.ReadLine();

                if (message == "exit")
                    break;

                channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes(message));
            }

            channel.Close();
            conn.Close();
        }
    }
}
