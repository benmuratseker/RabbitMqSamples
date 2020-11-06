using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace HeadersSample
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    IConnection conn;
        //    IModel channel;

        //    ConnectionFactory factory = new ConnectionFactory();
        //    factory.HostName = "localhost";
        //    factory.VirtualHost = "/";
        //    factory.Port = 5672;
        //    factory.UserName = "guest";
        //    factory.Password = "guest";

        //    conn = factory.CreateConnection();
        //    channel = conn.CreateModel();

        //    channel.ExchangeDeclare("ex.headers", "headers", true, false, null);

        //    channel.QueueDeclare("my.queue1", true, false, false, null);
        //    channel.QueueDeclare("my.queue2", true, false, false, null);

        //    channel.QueueBind(
        //        "my.queue1",
        //        "ex.headers",
        //        "",
        //        new Dictionary<string, object>()
        //        {
        //            {"x-match","all" },
        //            {"job", "convert" },
        //            {"format","jpeg" }
        //        });//job ve format uyumlu olmalı

        //    channel.QueueBind(
        //        "my.queue2",
        //        "ex.headers",
        //        "",
        //        new Dictionary<string, object>()
        //        {
        //            {"x-match","any" },
        //            {"job", "convert" },
        //            {"format","jpeg" }
        //        });//job veya formattan birisi uysa yeterli

        //    IBasicProperties props = channel.CreateBasicProperties();
        //    var headers = new Dictionary<string, object>();
        //    headers.Add("job", "convert");
        //    headers.Add("format", "jpeg");

        //    props.Headers = headers;


        //    channel.BasicPublish(
        //        "ex.headers",
        //        "",
        //        props,
        //        Encoding.UTF8.GetBytes("Message 1"));


        //    props = channel.CreateBasicProperties();
        //    headers = new Dictionary<string, object>();
        //    headers.Add("job", "convert");
        //    headers.Add("format", "bmp");

        //    props.Headers = headers;

        //    channel.BasicPublish(
        //        "ex.headers",
        //        "",
        //        props,
        //        Encoding.UTF8.GetBytes("Message 2"));
        //}





        const string ExchangeName = "header-exchange-example";

        static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Headers, false, true, null);
            byte[] message = Encoding.UTF8.GetBytes("Hello, World!");

            var properties =  channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>();
            properties.Headers.Add("job", "convert");
            properties.Headers.Add("format", "jpeg");

            channel.QueueDeclare("my.queue1", true, false, false, null);

            channel.QueueBind(
                "my.queue1",
                "header-exchange-example",
                "",
                new Dictionary<string, object>()
                {
                        {"x-match","all" },
                        {"job", "convert" },
                        {"format","jpeg" }
                });//job ve format uyumlu olmalı

            channel.QueueDeclare("my.queue2", true, false, false, null);

            channel.QueueBind(
                "my.queue2",
                "header-exchange-example",
                "",
                new Dictionary<string, object>()
                {
                        {"x-match","any" },
                        {"job", "convert" },
                        {"format","jpeg" }
                });//job ve format uyumlu olmalı

            TimeSpan time = TimeSpan.FromSeconds(10);
                var stopwatch = new Stopwatch();
            Console.WriteLine("Running for {0} seconds", time.ToString("ss"));
            stopwatch.Start();
            var messageCount = 0;

            while (stopwatch.Elapsed < time)
            {
                channel.BasicPublish(ExchangeName, "", properties, message);
                messageCount++;
                Console.Write("Time to complete: {0} seconds - Messages published: {1}\r", (time - stopwatch.Elapsed).ToString("ss"), messageCount);
                Thread.Sleep(1000);
                if(messageCount == 5)
                {
                    properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("job", "convert");
                    properties.Headers.Add("format", "png");
                }
            }

            Console.Write(new string(' ', 70) + "\r");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            message = Encoding.UTF8.GetBytes("quit");
            channel.BasicPublish(ExchangeName, "", properties, message);
            connection.Close();
        }
    }
}
