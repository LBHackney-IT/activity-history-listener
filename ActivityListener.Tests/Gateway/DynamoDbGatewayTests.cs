using ActivityListener.Gateway;
using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Shared;
using Hackney.Shared.ActivityHistory.Domain;
using Hackney.Shared.ActivityHistory.Factories;
using Hackney.Shared.ActivityHistory.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ActivityListener.Tests.Gateway
{
    [Collection("AppTest collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly DynamoDbGateway _classUnderTest;
        private readonly IDynamoDbFixture _dbFixture;
        private IDynamoDBContext DynamoDb => _dbFixture.DynamoDbContext;

        public DynamoDbGatewayTests(MockApplicationFactory appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _classUnderTest = new DynamoDbGateway(DynamoDb, _logger.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        private object _dbTestFixture;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_dbTestFixture != null)
                    _dbTestFixture = null;

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
