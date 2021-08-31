namespace Liberis.OrchestrationHub.Core.Options
{
    public class MongoDBOptions
    {
        public const string Options = "MongoDB";
        public string DatabaseName { get; set; }
        public string Url { get; set; }
    }
}
