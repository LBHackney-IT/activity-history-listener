using ActivityListener.Boundary;
using ActivityListener.Domain;
using ActivityListener.Factories;
using ActivityListener.Gateway.Interfaces;
using ActivityListener.UseCase;
using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ActivityListener.Tests.UseCase
{
    public class MessageProcessorTests
    {
        private readonly Mock<IDynamoDbGateway> _mockGateway;
        private readonly MessageProcessor _sut;

        private readonly EntityEventSns _eventSns;

        private readonly Fixture _fixture;

        public MessageProcessorTests()
        {
            _fixture = new Fixture();

            _mockGateway = new Mock<IDynamoDbGateway>();
            _sut = new MessageProcessor(_mockGateway.Object);

            _eventSns = CreateEventSns();
        }

        private EntityEventSns CreateEventSns(string eventType = EventTypes.PersonUpdatedEvent)
        {
            return _fixture.Build<EntityEventSns>()
                           .With(x => x.EventType, eventType)
                           .Create();
        }

        [Fact]
        public void ProcessMessageAsyncTestNullMessageThrows()
        {
            Func<Task> func = async () => await _sut.ProcessMessageAsync(null).ConfigureAwait(false);
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void ProcessMessageAsyncTestGatewayExceptionThrown()
        {
            var exMessage = "Some exception";
            _mockGateway.Setup(x => x.SaveAsync(It.IsAny<ActivityHistoryEntity>()))
                        .ThrowsAsync(new Exception(exMessage));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_eventSns).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMessage);
        }

        //[Fact]
        //public async Task ProcessMessageAsyncTestGatewayCalledSuccessfully()
        //{
        //    await _sut.ProcessMessageAsync(_eventSns).ConfigureAwait(false);
        //    Func<ActivityHistoryEntity, bool> VerifyDomainObject = (ahe) =>
        //    {
        //        ahe.Should().BeEquivalentTo(_eventSns.ToDomain());
        //        return true;
        //    };
        //    _mockGateway.Verify(x => x.SaveAsync(It.Is<ActivityHistoryEntity>(y => VerifyDomainObject(y))),
        //                        Times.Once);
        //}
    }
}
