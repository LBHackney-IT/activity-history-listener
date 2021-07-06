using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;

namespace ActivityListener.Infrastructure
{

    [DynamoDBTable("ActivityHistory", LowerCamelCaseProperties = true)]
    public class ActivityHistoryDB
    {
        [DynamoDBHashKey]
        public Guid TargetId { get; set; }

        [DynamoDBRangeKey]
        public Guid Id { get; set; }


        [DynamoDBVersion]
        public int? VersionNumber { get; set; }
    }
}
