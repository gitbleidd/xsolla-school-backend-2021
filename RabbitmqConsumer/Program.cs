using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;

namespace RabbitmqConsumer
{
    class Program
    {
        static private readonly int _messageDelayTimeInMs = 1000 * 60; // Время задержки отправки сообщения в миллисекундах

        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "common-exchange",
                    type: "direct",
                    durable: true,
                    autoDelete: false);

                channel.QueueDeclare(queue: "common-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                channel.QueueBind(queue: "common-queue",
                  exchange: "common-exchange",
                  routingKey: "");

                InitDelayMessageRepeat();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += ConsumerReceivedCommonMessage;

                channel.BasicConsume(queue: "common-queue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static async void ConsumerReceivedCommonMessage(object sender, BasicDeliverEventArgs e)
        {
            // Проверяет на доступность endpoint, доставленный producer'ом

            var body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine($"[x] Message: {message}");

            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(message);
                Console.WriteLine($"[Debug] Status code: {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // Переотправка (повторная обработка данного сообщения) запроса в случае,
            // если полученная страница вернула статус код, отличный от 20*.
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                properties.Expiration = _messageDelayTimeInMs.ToString();

                channel.BasicPublish(exchange: "delay-exchange",
                                 routingKey: "",
                                 body: body, 
                                 basicProperties: properties);
            }
            Console.WriteLine($"Message will be repeated in {_messageDelayTimeInMs} ms.");
        }

        private static void InitDelayMessageRepeat()
        {
            // Инициализирует queue, exchange для отправки сообщений с задержкой

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "delay-exchange",
                    type: "fanout",
                    durable: true,
                    autoDelete: false);

                Dictionary<string, object> args = new Dictionary<string, object>()
                {
                    { "x-dead-letter-exchange", "common-exchange" }
                };

                channel.QueueDeclare(queue: $"delay-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: args);

                channel.QueueBind(queue: "delay-queue",
                  exchange: "delay-exchange",
                  routingKey: "");
            }
        }
    }
}
