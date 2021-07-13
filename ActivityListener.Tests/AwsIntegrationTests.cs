using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Xunit;

namespace ActivityListener.Tests
{
    public class AwsIntegrationTests
    {
        public IDynamoDBContext DynamoDbContext => _factory?.DynamoDbContext;

        private readonly AwsMockApplicationFactory _factory;
        private readonly IHost _host;

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

        public AwsIntegrationTests()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

            _factory = new AwsMockApplicationFactory(_tables);
            _host = _factory.CreateHostBuilder(null).Build();
            _host.Start();

            LogCallAspectFixture.SetupLogCallAspect();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != _host)
                {
                    _host.StopAsync().GetAwaiter().GetResult();
                    _host.Dispose();
                }
                _disposed = true;
            }
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }
    }

    public class TableDef
    {
        public string Name { get; set; }
        public string KeyName { get; set; }
        public ScalarAttributeType KeyType { get; set; }
        public string RangeKeyName { get; set; }
        public ScalarAttributeType RangeKeyType { get; set; }
        public List<LocalSecondaryIndex> LocalSecondaryIndexes { get; set; } = new List<LocalSecondaryIndex>();
    }

    [CollectionDefinition("Aws collection", DisableParallelization = true)]
    public class AwsCollection : ICollectionFixture<AwsIntegrationTests>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
