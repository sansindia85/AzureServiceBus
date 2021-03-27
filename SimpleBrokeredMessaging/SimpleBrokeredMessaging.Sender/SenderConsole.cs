using System;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace SimpleBrokeredMessaging.Sender
{
    class SenderConsole
    {
        static string ConnectionString =
            "Endpoint=sb://sansdemos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=4Im6V2/ka4hKyG8rQ7P+QfcNzPZ3WTOdbhlDWSq1QjM=";

        static string QueuePath = "demoqueue";

        static void Main(string[] args)
        {
            //Create a queue client
            var queueClient = new QueueClient(ConnectionString, QueuePath);
            
            //Send some messages
            for (int index = 0; index < 10; ++index)
            {
                var content = $"Message: {index}";
                var message = new Message(Encoding.UTF8.GetBytes(content));
                queueClient.SendAsync(message).Wait();

                Console.WriteLine("Sent: " +index);
            }

            //Close the client
            queueClient.CloseAsync().Wait();

            Console.WriteLine("Sent messages...");
            Console.ReadLine();
        }
    }
}
