using System;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SimpleBrokerMessage.Receiver
{
    class ReceiverConsole
    {
        static string ConnectionString =
            "Endpoint=sb://sansdemos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=4Im6V2/ka4hKyG8rQ7P+QfcNzPZ3WTOdbhlDWSq1QjM=";

        static string QueuePath = "demoqueue";

        static void Main(string[] args)
        {
            //Create a queue client
            var queueClient = new QueueClient(ConnectionString, QueuePath);

            //Create a message handler to receive messages
            //The messages will be received on a different thread.
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, HandleExceptionAsync);

            Console.Write("Press enter to exit.");
            Console.ReadLine();

            //Close the client
            queueClient.CloseAsync().Wait();
        }

        private static Task HandleExceptionAsync(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var content = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received: {content}");

        }
    }
}
