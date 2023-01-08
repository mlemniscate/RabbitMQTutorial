// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare(
    queue: "hello",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    int dots = message.Split('.').Length - 1;
    Thread.Sleep( dots * 1000);
    Console.WriteLine(" [x] Done");

    channel.BasicAck(
        deliveryTag: ea.DeliveryTag,
        multiple: false);
};

channel.BasicConsume(
    queue: "hello",
    autoAck: false,
    consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
