using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

Console.WriteLine("Azure Queue Storage client library - .NET quickstart sample");

string queueName = "quickstartqueues-" + Guid.NewGuid().ToString();
string storageAccountName = "stdevidentitymigration";

Console.Write("Enter 1 (Passwordless) or 2 (Connection String): ");
string? input = Console.ReadLine();

QueueClient? queueClient = null;

if (input == "1")
{
    // Instantiate a QueueClient to create and interact with the queue
    queueClient = new QueueClient(
        new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
        new DefaultAzureCredential());
}
else if (input == "2")
{
    // Retrieve the connection string for use with the application. The storage
    // connection string is stored in an environment variable called
    // AZURE_STORAGE_CONNECTION_STRING on the machine running the application.
    // If the environment variable is created after the application is launched
    // in a console or with Visual Studio, the shell or application needs to be
    // closed and reloaded to take the environment variable into account.
    string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("AZURE_STORAGE_CONNECTION_STRING environment variable is not set.");
        return;
    }
    queueClient = new QueueClient(connectionString, queueName);
}
else
{
    Console.WriteLine("Invalid input. Please enter 1 or 2.");
    return;
}

Console.WriteLine($"Creating queue: {queueName}");

// Create the queue
await queueClient.CreateAsync();

Console.WriteLine("\nAdding messages to the queue...");

// Send several messages to the queue
await queueClient.SendMessageAsync("First message");
await queueClient.SendMessageAsync("Second message");

// Save the receipt so we can update this message later
SendReceipt receipt = await queueClient.SendMessageAsync("Third message");

Console.WriteLine("\nPeek at the messages in the queue...");

// Peek at messages in the queue
PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(maxMessages: 10);

foreach (PeekedMessage peekedMessage in peekedMessages)
{
    // Display the message
    Console.WriteLine($"Message: {peekedMessage.MessageText}");
}

Console.WriteLine("\nUpdating the third message in the queue...");

// Update a message using the saved receipt from sending the message
await queueClient.UpdateMessageAsync(receipt.MessageId, receipt.PopReceipt, "Third message has been updated");

QueueProperties properties = queueClient.GetProperties();

// Retrieve the cached approximate message count
int cachedMessagesCount = properties.ApproximateMessagesCount;

// Display number of messages
Console.WriteLine($"Number of messages in queue: {cachedMessagesCount}");

Console.WriteLine("\nReceiving messages from the queue...");

// Get messages from the queue
QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 10);

Console.WriteLine("\nPress Enter key to 'process' messages and delete them from the queue...");
Console.ReadLine();

// Process and delete messages from the queue
foreach (QueueMessage message in messages)
{
    // "Process" the message
    Console.WriteLine($"Message: {message.MessageText}");

    // Let the service know we're finished with
    // the message and it can be safely deleted.
    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
}

Console.WriteLine("\nPress Enter key to delete the queue...");
Console.ReadLine();

// Clean up
Console.WriteLine($"Deleting queue: {queueClient.Name}");
await queueClient.DeleteAsync();

Console.WriteLine("Done");