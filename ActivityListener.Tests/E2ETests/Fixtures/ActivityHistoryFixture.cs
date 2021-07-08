using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using ActivityListener.Infrastructure;

namespace ActivityListener.Tests.E2ETests.Fixtures
{
    public class ActivityHistoryFixture : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly IDynamoDBContext _dbContext;


        public ActivityHistoryFixture(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }
    }
}
