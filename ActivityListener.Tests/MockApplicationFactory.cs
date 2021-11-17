using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Hackney.Core.DynamoDb;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace ActivityListener.Tests
{
    public class MockApplicationFactory
    {
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef
            {
                Name = "ActivityHistory",
                KeyName = "targetId",
                KeyType = ScalarAttributeType.S,
                RangeKeyName = "id",
                RangeKeyType = ScalarAttributeType.S,
                LocalSecondaryIndexes = new List<LocalSecondaryIndex>(new[]
                {
                    new LocalSecondaryIndex
                    {
                        IndexName = "ActivityHistoryByCreatedAt",
                        KeySchema = new List<KeySchemaElement>(new[]
                        {
                            new KeySchemaElement("targetId", KeyType.HASH),
                            new KeySchemaElement("createdAt", KeyType.RANGE)
                        }),
                        Projection = new Projection { ProjectionType = ProjectionType.ALL }
                    }
                })
            }
        };

        public IDynamoDbFixture DynamoDbFixture { get; private set; }

        public MockApplicationFactory()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

            CreateHostBuilder().Build();
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        public IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder(null)
           .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
           .ConfigureServices((hostContext, services) =>
           {
               services.ConfigureDynamoDB();
               services.ConfigureDynamoDbFixture();

               var serviceProvider = services.BuildServiceProvider();

               LogCallAspectFixture.SetupLogCallAspect();

               DynamoDbFixture = serviceProvider.GetRequiredService<IDynamoDbFixture>();
               DynamoDbFixture.EnsureTablesExist(_tables);
           });
    }
}
