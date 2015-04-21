using System;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Common;
using StandUpTimer.Services;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.UnitTests.Services
{
    [TestFixture]
    public class StatusPublisherTests
    {
        [Test]
        public void When_creating_an_instance_publish_a_sitting_state_to_the_server()
        {
            var server = A.Fake<IServer>();

            new StatusPublisher(server);

            A.CallTo(() => server.SendDeskState(A<Status>.That.Matches(x => x.DeskState == DeskState.Sitting)))
             .MustHaveHappened();
        }

        [Test]
        public void You_can_publish_a_new_state()
        {
            var server = A.Fake<IServer>();

            var target = new StatusPublisher(server);

            target.PublishChangedDeskState(StandUpTimer.Models.DeskState.Standing);

            A.CallTo(() => server.SendDeskState(A<Status>.That.Matches(x => x.DeskState == DeskState.Standing)))
             .MustHaveHappened();
        }

        [Test]
        public void When_publishing_use_the_current_time()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 21, 12, 19, 0));

            var server = A.Fake<IServer>();

            var target = new StatusPublisher(server);

            target.PublishChangedDeskState(StandUpTimer.Models.DeskState.Standing);

            A.CallTo(() => server.SendDeskState(A<Status>.That.Matches(x => x.DateTime.Equals("2015, 4, 21, 12, 19, 0, 0"))))
             .MustHaveHappened();
        }

        [Test]
        public void You_can_publish_the_end_of_a_session()
        {
            var server = A.Fake<IServer>();

            var target = new StatusPublisher(server);

            target.PublishEndOfSession();

            A.CallTo(() => server.SendDeskState(A<Status>.That.Matches(x => x.DeskState == DeskState.Inactive)))
             .MustHaveHappened();
        }
    }
}