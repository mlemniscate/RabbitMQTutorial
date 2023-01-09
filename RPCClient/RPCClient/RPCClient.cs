using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var correlationId = Guid.NewGuid().ToString();
var replyQueueName = "rpc_queue_" + correlationId;
var consumer = new EventingBasicConsumer(channel);

var number = args.Length == 1 ? int.Parse(args[0]) : 2;
channel.QueueDeclare("rpc_queue", false, false, false, null);
channel.QueueDeclare(replyQueueName, false, false, false, null);
channel.BasicQos(0, 1, false);

var props = channel.CreateBasicProperties();
props.CorrelationId = correlationId;
props.ReplyTo = replyQueueName;

var body = Encoding.UTF8.GetBytes(number.ToString());
channel.BasicPublish("", "rpc_queue", false, props, body);

consumer.Received += (model, ea) =>
{
    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
    if (ea.BasicProperties.CorrelationId == correlationId)
    {
        Console.WriteLine(response);
    }
};

channel.BasicConsume(consumer, replyQueueName, true);

Console.WriteLine("Wait");
Console.ReadLine();