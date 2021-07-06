using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Hackney.Core.DynamoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace ActivityListener.Tests
{
    public class AwsMockApplicationFactory
    {
        private readonly List<TableDef> _tables;

        public IAmazonDynamoDB DynamoDb { get; private set; }
        public IDynamoDBContext DynamoDbContext { get; private set; }

        public AwsMockApplicationFactory(List<TableDef> tables)
        {
            _tables = tables;
        }

        public IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
           .ConfigureServices((hostContext, services) =>
           {
               services.ConfigureDynamoDB();

               var serviceProvider = services.BuildServiceProvider();
               DynamoDb = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
               DynamoDbContext = serviceProvider.GetRequiredService<IDynamoDBContext>();

               EnsureTablesExist(DynamoDb, _tables);
           });

        private static void EnsureTablesExist(IAmazonDynamoDB dynamoDb, List<TableDef> tables)
        {
            foreach (var table in tables)
            {
                try
                {
                    var keySchema = new List<KeySchemaElement> { new KeySchemaElement(table.KeyName, KeyType.HASH) };
                    var attributes = new List<AttributeDefinition> { new AttributeDefinition(table.KeyName, table.KeyType) };
                    if (!string.IsNullOrEmpty(table.RangeKeyName))
                    {
                        keySchema.Add(new KeySchemaElement(table.RangeKeyName, KeyType.RANGE));
                        attributes.Add(new AttributeDefinition(table.RangeKeyName, table.RangeKeyType));
                    }

                    var request = new CreateTableRequest(table.Name,
                        keySchema,
                        attributes,
                        new ProvisionedThroughput(3, 3));
                    _ = dynamoDb.CreateTableAsync(request).GetAwaiter().GetResult();
                }
                catch (ResourceInUseException)
                {
                    // It already exists :-)
                }
            }
        }
    }
}
