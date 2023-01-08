using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReceiveLogDirect;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

var queueName = channel.QueueDeclare().QueueName;

ArgsManagement.CheckArgsCount(args);

foreach (var severity in args)
{
    channel.QueueBind(
        queue: queueName,
        exchange: "direct_logs",
        routingKey: severity);
}

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = args.RoutingKey;
    Console.WriteLine($" [x] {message}:{routingKey}");
};

channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

