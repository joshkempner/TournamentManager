using System;
using System.Reactive;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Testing;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public class when_adding_a_referee :
        IDisposable,
        IHandleCommand<RefereeMsgs.AddReferee>,
        IHandleCommand<RefereeMsgs.AddOrUpdateEmailAddress>,
        IHandleCommand<RefereeMsgs.AddOrUpdateAge>,
        IHandleCommand<RefereeMsgs.AddOrUpdateBirthdate>,
        IHandleCommand<RefereeMsgs.AddOrUpdateMaxAgeBracket>
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _mockHostScreen = new MockHostScreen();

        public when_adding_a_referee()
        {
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddReferee>(this);
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateEmailAddress>(this);
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateAge>(this);
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateBirthdate>(this);
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
        }

        public void Dispose()
        {
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddReferee>(this);
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateEmailAddress>(this);
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateAge>(this);
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateBirthdate>(this);
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
        }

        [Fact]
        public void can_add_intramural_referee()
        {
            var vm = new NewRefereeVM(_fixture.Dispatcher, _mockHostScreen)
            {
                GivenName = "John",
                Surname = "Smith",
                EmailAddress = "John.Smith@aol.com",
                RefereeGrade = RefereeMsgs.Grade.Intramural,
                Age = 12
            };
            AssertEx.IsOrBecomesTrue(() => vm.CanAddReferee);
            vm.AddReferee.Execute(Unit.Default).Subscribe();
            _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(100));
            var cmd = _fixture.TestQueue.DequeueNext<RefereeMsgs.AddReferee>();
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateEmailAddress>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateAge>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd.CorrelationId, out var maxAgeCmd)
                .AssertEmpty();
            Assert.Equal(RefereeMsgs.Grade.Intramural, cmd.RefereeGrade);
            Assert.Equal(TeamMsgs.AgeBracket.U8, maxAgeCmd.MaxAgeBracket);
        }

        [Fact]
        public void can_add_certified_referee()
        {
            var birthdate = new DateTime(1990, 7, 1);
            var vm = new NewRefereeVM(_fixture.Dispatcher, _mockHostScreen)
            {
                GivenName = "John",
                Surname = "Smith",
                EmailAddress = "John.Smith@aol.com",
                RefereeGrade = RefereeMsgs.Grade.Grassroots,
                Birthdate = birthdate,
                MaxAgeBracket = TeamMsgs.AgeBracket.U14
            };
            AssertEx.IsOrBecomesTrue(() => vm.CanAddReferee);
            vm.AddReferee.Execute(Unit.Default).Subscribe();
            _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(100));
            var cmd = _fixture.TestQueue.DequeueNext<RefereeMsgs.AddReferee>();
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateEmailAddress>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateBirthdate>(cmd.CorrelationId, out var birthdateCmd)
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd.CorrelationId, out var maxAgeCmd)
                .AssertEmpty();
            Assert.Equal(RefereeMsgs.Grade.Grassroots, cmd.RefereeGrade);
            Assert.Equal(birthdate, birthdateCmd.Birthdate);
            Assert.Equal(TeamMsgs.AgeBracket.U14, maxAgeCmd.MaxAgeBracket);
        }

        [Fact]
        public void cannot_add_referee_with_invalid_email_address()
        {
            var vm = new NewRefereeVM(_fixture.Dispatcher, _mockHostScreen)
            {
                GivenName = "John",
                Surname = "Smith",
                EmailAddress = "John.Smith",
                RefereeGrade = RefereeMsgs.Grade.Intramural,
                Age = 12
            };
            AssertEx.IsOrBecomesFalse(() => vm.CanAddReferee);
        }

        [Fact]
        public void can_cancel_adding_a_referee()
        {
            var vm = new NewRefereeVM(_fixture.Dispatcher, _mockHostScreen);
            vm.Cancel.Execute(Unit.Default).Subscribe();
            Assert.Throws<TimeoutException>(
                () => _fixture.TestQueue.WaitFor<RefereeMsgs.RefereeAdded>(TimeSpan.FromMilliseconds(100)));
            _fixture.TestQueue.AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        public CommandResponse Handle(RefereeMsgs.AddReferee command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateEmailAddress command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateAge command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateBirthdate command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateMaxAgeBracket command)
        {
            return command.Succeed();
        }
    }
}
