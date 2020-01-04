using System;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Testing;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public class when_updating_referee_credentials :
        IDisposable,
        IHandleCommand<RefereeMsgs.UpdateGrade>,
        IHandleCommand<RefereeMsgs.AddOrUpdateAge>,
        IHandleCommand<RefereeMsgs.AddOrUpdateMaxAgeBracket>
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _screen = new MockHostScreen();
        private readonly CorrelatedStreamStoreRepository _repo;

        public when_updating_referee_credentials()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => _fixture.StreamStoreConnection, typeof(IStreamStoreConnection));
            _fixture.Dispatcher.Subscribe<RefereeMsgs.UpdateGrade>(this);
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateAge>(this);
            _fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
            _repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
        }

        [Fact]
        public void can_update_referee_grade()
        {
            var refereeId = Guid.NewGuid();
            AddIntramuralReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);

            vm.RefereeGrade = RefereeMsgs.Grade.Regional;
            vm.Save.Execute().Subscribe();
            _fixture.TestQueue.WaitFor<RefereeMsgs.UpdateGrade>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateGrade>(cmd => cmd.RefereeId == refereeId &&
                                                            cmd.RefereeGrade == RefereeMsgs.Grade.Regional)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_grade_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            AddIntramuralReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);

            vm.RefereeGrade = RefereeMsgs.Grade.Intramural;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => _fixture.TestQueue.WaitFor<RefereeMsgs.UpdateGrade>(TimeSpan.FromMilliseconds(100)));
            _fixture.TestQueue.AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_intramural_referee_age()
        {
            var refereeId = Guid.NewGuid();
            AddIntramuralReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => 12 == vm.CurrentAge);

            const ushort newAge = 13;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateAge>(cmd => cmd.RefereeId == refereeId &&
                                                               cmd.Age == newAge)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_age_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            AddIntramuralReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => 12 == vm.CurrentAge);

            const ushort newAge = 12;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(100)));
            _fixture.TestQueue.AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void cannot_update_travel_referee_age()
        {
            var refereeId = Guid.NewGuid();
            AddTravelReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);

            const ushort newAge = 20;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(100)));
            _fixture.TestQueue.AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_max_age_bracket()
        {
            var refereeId = Guid.NewGuid();
            AddTravelReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => TeamMsgs.AgeBracket.U16 == vm.MaxAgeBracket);
            // ReSharper restore AccessToDisposedClosure

            const TeamMsgs.AgeBracket newAgeBracket = TeamMsgs.AgeBracket.Adult;
            vm.MaxAgeBracket = newAgeBracket;
            vm.Save.Execute().Subscribe();
            _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd => cmd.RefereeId == refereeId &&
                                                                         cmd.MaxAgeBracket == newAgeBracket)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_max_age_bracket_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            AddTravelReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => TeamMsgs.AgeBracket.U16 == vm.MaxAgeBracket);
            // ReSharper restore AccessToDisposedClosure

            const TeamMsgs.AgeBracket newAgeBracket = TeamMsgs.AgeBracket.U16;
            vm.MaxAgeBracket = newAgeBracket;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => _fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(100)));
            _fixture.TestQueue.AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_cancel_updating_credentials()
        {
            var refereeId = Guid.NewGuid();
            AddTravelReferee(refereeId);
            using var vm = new RefereeCredentialsVM(refereeId, _fixture.Dispatcher, _screen);
            vm.Cancel.Execute().Subscribe();
            _fixture.TestQueue.AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        private void AddIntramuralReferee(Guid refereeId)
        {
            var referee = new Referee(
                                refereeId,
                                "John",
                                "Smith",
                                RefereeMsgs.Grade.Intramural,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            referee.AddOrUpdateAge(12);
            referee.AddOrUpdateMaxAgeBracket(TeamMsgs.AgeBracket.U8);
            _repo.Save(referee);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.MaxAgeBracketChanged>(TimeSpan.FromMilliseconds(100));
            _fixture.ClearQueues();
        }

        private void AddTravelReferee(Guid refereeId)
        {
            var referee = new Referee(
                                refereeId,
                                "John",
                                "Smith",
                                RefereeMsgs.Grade.Grassroots,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            referee.AddOrUpdateBirthdate(new DateTime(1990, 1, 1));
            referee.AddOrUpdateMaxAgeBracket(TeamMsgs.AgeBracket.U16);
            _repo.Save(referee);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.MaxAgeBracketChanged>(TimeSpan.FromMilliseconds(100));
            _fixture.ClearQueues();
        }

        public void Dispose()
        {
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.UpdateGrade>(this);
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateAge>(this);
            _fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
        }

        public CommandResponse Handle(RefereeMsgs.UpdateGrade command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateAge command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateMaxAgeBracket command)
        {
            return command.Succeed();
        }
    }
}
