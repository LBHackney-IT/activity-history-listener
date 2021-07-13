using ActivityListener.Domain;
using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;

namespace ActivityListener.Infrastructure
{
    [DynamoDBTable("ActivityHistory", LowerCamelCaseProperties = true)]
    public class ActivityHistoryDB
    {
        [DynamoDBRangeKey]
        public Guid Id { get; set; }

        [DynamoDBHashKey]
        public Guid TargetId { get; set; }

        public string SourceDomain { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbEnumConverter<ActivityType>))]
        public ActivityType Type { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbEnumConverter<TargetType>))]
        public TargetType TargetType { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        public int TimetoLiveForRecord { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Dictionary<string, object>>))]
        public object OldData { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Dictionary<string, object>>))]
        public object NewData { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<AuthorDetails>))]
        public AuthorDetails AuthorDetails { get; set; }
    }
}
