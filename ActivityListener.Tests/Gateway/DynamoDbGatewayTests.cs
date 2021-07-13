using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using ActivityListener.Domain;
using ActivityListener.Factories;
using ActivityListener.Gateway;
using ActivityListener.Infrastructure;
using Xunit;
using ActivityListener.Domain;

namespace ActivityListener.Tests.Gateway
{
    [Collection("Aws collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly DynamoDbGateway _classUnderTest;
        private AwsIntegrationTests _dbTestFixture;
        private IDynamoDBContext DynamoDb => _dbTestFixture.DynamoDbContext;
        private readonly List<Action> _cleanup = new List<Action>();

        public DynamoDbGatewayTests(AwsIntegrationTests dbTestFixture)
        {
            _dbTestFixture = dbTestFixture;
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _classUnderTest = new DynamoDbGateway(DynamoDb, _logger.Object);
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
                foreach (var action in _cleanup)
                    action();

                if (_dbTestFixture != null)
                {
                    _dbTestFixture.Dispose();
                    _dbTestFixture = null;
                }

                _disposed = true;
            }
        }

        private ActivityHistoryEntity ConstructDomainObject(object oldData = null, object newData = null)
        {
            oldData = oldData ?? new Dictionary<string, object>();
            newData = newData ?? new Dictionary<string, object>();
            return _fixture.Build<ActivityHistoryEntity>()
                           .With(x => x.CreatedAt, DateTime.UtcNow)
                           .With(x => x.OldData, oldData)
                           .With(x => x.NewData, newData)
                           .Create();
        }

        [Fact]
        public async Task SaveAsyncUpdatesDatabase()
        {
            var domain = ConstructDomainObject();

            await _classUnderTest.SaveAsync(domain).ConfigureAwait(false);

            var savedInDB = await DynamoDb.LoadAsync<ActivityHistoryDB>(domain.TargetId, domain.Id).ConfigureAwait(false);
            savedInDB.Should().BeEquivalentTo(domain.ToDatabase());

            _logger.VerifyExact(LogLevel.Debug,
                                $"Calling IDynamoDBContext.SaveAsync for target id {domain.TargetId}, id {domain.Id}",
                                Times.Once());

            await DynamoDb.DeleteAsync(savedInDB).ConfigureAwait(false);
        }

        [Fact]
        public void SaveAsyncExceptionThrown()
        {
            var domain = ConstructDomainObject();
            var mockDynamoDb = new Mock<IDynamoDBContext>();
            var exMsg = "Some exception";
            mockDynamoDb.Setup(x => x.SaveAsync<ActivityHistoryDB>(It.IsAny<ActivityHistoryDB>(), default))
                        .ThrowsAsync(new Exception(exMsg));

            var classUnderTest = new DynamoDbGateway(mockDynamoDb.Object, _logger.Object);
            Func<Task> func = async () => await classUnderTest.SaveAsync(domain).ConfigureAwait(false);

            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);

            _logger.VerifyExact(LogLevel.Debug,
                                $"Calling IDynamoDBContext.SaveAsync for target id {domain.TargetId}, id {domain.Id}",
                                Times.Once());
        }
    }
}
