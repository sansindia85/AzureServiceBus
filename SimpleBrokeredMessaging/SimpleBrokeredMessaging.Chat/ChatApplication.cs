using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SimpleBrokeredMessaging.Chat
{
    class ChatApplication
    {
        static string ConnectionString =
            "Endpoint=sb://sansdemos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=4Im6V2/ka4hKyG8rQ7P+QfcNzPZ3WTOdbhlDWSq1QjM=";

        static string TopicPath = "chattopic";
        static void Main(string[] args)
        {
            Console.WriteLine("Enter name:");
            var userName = Console.ReadLine();

            //Create a management client to manage artifacts
            var manager = new ManagementClient(ConnectionString);

            //Create a topic if it does not exist
            if (!manager.TopicExistsAsync(TopicPath).Result)
                manager.CreateTopicAsync(TopicPath).Wait();

            //Create a subscription for the user
            var description = new SubscriptionDescription(TopicPath, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };

            manager.CreateSubscriptionAsync(description).Wait();

            //Create Clients
            var topicClient = new TopicClient(ConnectionString, TopicPath);
            var subscriptionClient = new SubscriptionClient(ConnectionString, TopicPath,userName);

            //Create a message pump for receiving messages
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            //Send a message to say you are here
            var helloMessage = new Message(Encoding.UTF8.GetBytes("Has entered the room..."))
            {
                Label = userName
            };
            topicClient.SendAsync(helloMessage).Wait();

            while (true)
            {
                string text = Console.ReadLine();
                if (text != null && text.Equals("exit")) break;

                //send a chat message
                var chatMessage = new Message(Encoding.UTF8.GetBytes(text)) {Label = userName};
                topicClient.SendAsync(chatMessage).Wait();
            }

            //Send a message to say your leaving
            var goodbyMessage = new Message(Encoding.UTF8.GetBytes("Has left the building..."));
            goodbyMessage.Label = userName;
            topicClient.SendAsync(goodbyMessage).Wait();

            //close the clients
            topicClient.CloseAsync().Wait();
            subscriptionClient.CloseAsync().Wait();
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private static Task ProcessMessagesAsync(Message message, CancellationToken token)
        { 
            //Deserialize the message body.
            var text = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"{message.Label}> { text }");
            return Task.CompletedTask;
        }
    }
}
