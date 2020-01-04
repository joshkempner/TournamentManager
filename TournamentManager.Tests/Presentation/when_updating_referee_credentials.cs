using System;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Testing;
using TournamentManager.Helpers;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public class when_updating_referee_credentials :
        with_vm_fixtures,
        IDisposable,
        IHandleCommand<RefereeMsgs.UpdateGrade>,
        IHandleCommand<RefereeMsgs.AddOrUpdateAge>,
        IHandleCommand<RefereeMsgs.AddOrUpdateMaxAgeBracket>
    {
        public when_updating_referee_credentials()
        {
            Fixture.Dispatcher.Subscribe<RefereeMsgs.UpdateGrade>(this);
            Fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateAge>(this);
            Fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
        }

        [Fact]
        public void can_update_referee_grade()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);

            vm.RefereeGrade = RefereeMsgs.Grade.Regional;
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.UpdateGrade>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateGrade>(cmd => cmd.RefereeId == refereeId &&
                                                            cmd.RefereeGrade == RefereeMsgs.Grade.Regional)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_grade_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);

            vm.RefereeGrade = RefereeMsgs.Grade.Intramural;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.UpdateGrade>(TimeSpan.FromMilliseconds(200)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_intramural_referee_age()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.IntramuralAge == vm.CurrentAge);

            const ushort newAge = RefereeTestHelper.IntramuralAge + 1;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateAge>(cmd => cmd.RefereeId == refereeId &&
                                                               cmd.Age == newAge)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_age_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.IntramuralAge == vm.CurrentAge);

            const ushort newAge = RefereeTestHelper.IntramuralAge;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(200)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void cannot_update_travel_referee_age()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);

            var newAge = (ushort)RefereeTestHelper.TravelBirthdate.YearsAgo();
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(200)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_travel_referee_birthdate()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelBirthdate == vm.Birthdate);

            vm.Birthdate -= TimeSpan.FromDays(1);
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateBirthdate>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateBirthdate>(cmd => cmd.RefereeId == refereeId &&
                                                                     cmd.Birthdate == vm.Birthdate)
                .AssertEmpty();
            // ReSharper restore AccessToDisposedClosure
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_birthdate_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelBirthdate == vm.Birthdate);
            // ReSharper restore AccessToDisposedClosure

            vm.Birthdate = RefereeTestHelper.TravelBirthdate;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateBirthdate>(TimeSpan.FromMilliseconds(200)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_max_age_bracket()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelMaxAgeBracket == vm.MaxAgeBracket);
            // ReSharper restore AccessToDisposedClosure

            const TeamMsgs.AgeBracket newAgeBracket = TeamMsgs.AgeBracket.Adult;
            vm.MaxAgeBracket = newAgeBracket;
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd => cmd.RefereeId == refereeId &&
                                                                         cmd.MaxAgeBracket == newAgeBracket)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void setting_max_age_bracket_to_existing_value_does_not_trigger_update()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelMaxAgeBracket == vm.MaxAgeBracket);
            // ReSharper restore AccessToDisposedClosure

            const TeamMsgs.AgeBracket newAgeBracket = RefereeTestHelper.TravelMaxAgeBracket;
            vm.MaxAgeBracket = newAgeBracket;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(200)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_cancel_updating_credentials()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, Fixture.Dispatcher, Screen);
            vm.Cancel.Execute().Subscribe();
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        public void Dispose()
        {
            Fixture.Dispatcher.Unsubscribe<RefereeMsgs.UpdateGrade>(this);
            Fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateAge>(this);
            Fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
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
