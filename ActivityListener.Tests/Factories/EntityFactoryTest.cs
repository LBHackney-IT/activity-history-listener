//using AutoFixture;
//using ActivityListener.Domain;
//using ActivityListener.Factories;
//using ActivityListener.Infrastructure;
//using FluentAssertions;
//using Xunit;

//namespace ActivityListener.Tests.Factories
//{
//    public class EntityFactoryTest
//    {
//        private readonly Fixture _fixture = new Fixture();

//        [Fact]
//        public void CanMapADatabaseEntityToADomainObject()
//        {
//            var databaseEntity = _fixture.Create<TenureInformationDb>();
//            var entity = databaseEntity.ToDomain();

//            databaseEntity.Should().BeEquivalentTo(entity);
//        }

//        [Fact]
//        public void CanMapADomainEntityToADatabaseObject()
//        {
//            var entity = _fixture.Create<TenureInformation>();
//            var databaseEntity = entity.ToDatabase();

//            entity.Should().BeEquivalentTo(databaseEntity);
//        }
//    }
//}
