namespace Liberis.OrchestrationHub.Core.Options
{
    public class MessageBrokerOptions
    {
        public const string Options = "MessageBroker";
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
