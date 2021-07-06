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

        //private async Task InsertDatatoDynamoDB(TenureInformation entity)
        //{
        //    await DynamoDb.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        //    _cleanup.Add(async () => await DynamoDb.DeleteAsync<TenureInformationDb>(entity.Id).ConfigureAwait(false));
        //}

        //private TenureInformation ConstructTenureInformation(bool nullTenuredAssetType = false)
        //{
        //    var entity = _fixture.Build<TenureInformation>()
        //                         .With(x => x.EndOfTenureDate, DateTime.UtcNow)
        //                         .With(x => x.StartOfTenureDate, DateTime.UtcNow)
        //                         .With(x => x.SuccessionDate, DateTime.UtcNow)
        //                         .With(x => x.PotentialEndDate, DateTime.UtcNow)
        //                         .With(x => x.SubletEndDate, DateTime.UtcNow)
        //                         .With(x => x.EvictionDate, DateTime.UtcNow)
        //                         .With(x => x.VersionNumber, (int?) null)
        //                         .Create();

        //    if (nullTenuredAssetType)
        //        entity.TenuredAsset.Type = null;

        //    return entity;
        //}

        //[Fact]
        //public async Task GetTenureInfoByIdAsyncReturnsNullIfEntityDoesntExist()
        //{
        //    var id = Guid.NewGuid();
        //    var response = await _classUnderTest.GetTenureInfoByIdAsync(id).ConfigureAwait(false);

        //    response.Should().BeNull();
        //    _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {id}", Times.Once());
        //}

        //[Theory]
        //[InlineData(false)]
        //[InlineData(true)]
        //public async Task GetTenureInfoByIdAsyncReturnsTheEntityIfItExists(bool nullTenuredAssetType)
        //{
        //    var tenure = ConstructTenureInformation(nullTenuredAssetType);
        //    await InsertDatatoDynamoDB(tenure).ConfigureAwait(false);

        //    var response = await _classUnderTest.GetTenureInfoByIdAsync(tenure.Id).ConfigureAwait(false);

        //    response.Should().BeEquivalentTo(tenure, (e) => e.Excluding(y => y.VersionNumber));
        //    _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {tenure.Id}", Times.Once());
        //}

        //[Theory]
        //[InlineData(false)]
        //[InlineData(true)]
        //public async Task UpdateTenureInfoAsyncUpdatesDatabase(bool nullTenuredAssetType)
        //{
        //    var tenure = ConstructTenureInformation(nullTenuredAssetType);
        //    await InsertDatatoDynamoDB(tenure).ConfigureAwait(false);

        //    tenure.HouseholdMembers = _fixture.CreateMany<HouseholdMembers>(5);
        //    tenure.AccountType = _fixture.Create<AccountType>();
        //    tenure.IsMutualExchange = !tenure.IsMutualExchange;
        //    tenure.IsSublet = !tenure.IsSublet;
        //    tenure.RentCostCentre = "Some new cost centre";
        //    tenure.VersionNumber = 0; // This will have been set when injecting the inital record.

        //    await _classUnderTest.UpdateTenureInfoAsync(tenure).ConfigureAwait(false);

        //    var updatedInDB = await DynamoDb.LoadAsync<TenureInformationDb>(tenure.Id).ConfigureAwait(false);
        //    updatedInDB.ToDomain().Should().BeEquivalentTo(tenure, (e) => e.Excluding(y => y.VersionNumber));
        //    updatedInDB.VersionNumber.Should().Be(tenure.VersionNumber + 1);

        //    _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync for id {tenure.Id}", Times.Once());
        //}
    }
}