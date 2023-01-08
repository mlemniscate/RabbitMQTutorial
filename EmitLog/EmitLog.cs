// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Text;
using NewTask;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "logs",
    type: ExchangeType.Fanout);

string message = ConsoleMessage.GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(
    exchange: "logs",
    routingKey: "",
    basicProperties: null,
    body: body);

Console.WriteLine($" [x] Sent {message}");
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


