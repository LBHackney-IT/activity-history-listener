using ActivityListener.Infrastructure;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;

namespace ActivityListener.Tests.E2ETests.Fixtures
{
    public class ActivityHistoryFixture : IDisposable
    {
        private readonly IDynamoDBContext _dbContext;
        public List<ActivityHistoryDB> ToDelete { get; } = new List<ActivityHistoryDB>();


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
                foreach (var obj in ToDelete)
                    _dbContext.DeleteAsync(obj).GetAwaiter().GetResult();

                _disposed = true;
            }
        }
    }
}
