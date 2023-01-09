using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Math = RPCServer.Math;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var rpcQueue = "rpc_queue";
channel.QueueDeclare(rpcQueue, false, false, false, null);
channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(rpcQueue, false, consumer);

Console.WriteLine(" [x] Awaiting RPC requests");

consumer.Received += (model, ea) =>
{
    string response = null;
    var body = ea.Body.ToArray();
    var replyQueue = ea.BasicProperties.ReplyTo;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;
    
    try
    {
        var message = Encoding.UTF8.GetString(body);
        int number = int.Parse(message);
        Console.WriteLine($" [.] fib{message}");
        response = Math.Fib(number).ToString();
    }
    catch (Exception e)
    {
        Console.WriteLine($" [.] {e.Message}");
        response = "";
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish("", replyQueue, false, replyProps,responseBytes);
        channel.BasicAck(ea.DeliveryTag, multiple: false);
    }
};

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
