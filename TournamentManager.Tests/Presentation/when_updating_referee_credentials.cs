﻿using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Helpers;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_updating_referee_credentials :
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
        public void initial_values_are_correct()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.RefereeId == refereeId);
            AssertEx.IsOrBecomesTrue(() => vm.RefereeGrade == RefereeMsgs.Grade.Grassroots);
            AssertEx.IsOrBecomesTrue(() => vm.Birthdate == RefereeTestHelper.TravelBirthdate);
            AssertEx.IsOrBecomesTrue(() => vm.CurrentAge == (ushort)RefereeTestHelper.TravelBirthdate.YearsAgo());
            AssertEx.IsOrBecomesTrue(() => vm.MaxAgeBracket == RefereeTestHelper.TravelMaxAgeBracket);
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);
            // ReSharper restore AccessToDisposedClosure
        }

        [Fact]
        public void initial_values_are_correct_after_updates()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            var referee = repo.GetById<Referee>(refereeId, MessageBuilder.New(() => new TestCommands.Command1()));
            referee.UpdateRefereeGrade(RefereeMsgs.Grade.Regional);
            referee.AddOrUpdateBirthdate(RefereeTestHelper.TravelBirthdate.AddYears(-1));
            referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.Adult);
            repo.Save(referee);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.RefereeGrade == RefereeMsgs.Grade.Regional);
            AssertEx.IsOrBecomesTrue(() => vm.Birthdate == RefereeTestHelper.TravelBirthdate.AddYears(-1));
            AssertEx.IsOrBecomesTrue(() => vm.CurrentAge == (ushort)RefereeTestHelper.TravelBirthdate.AddYears(-1).YearsAgo());
            AssertEx.IsOrBecomesTrue(() => vm.MaxAgeBracket == TournamentMsgs.AgeBracket.Adult);
            // ReSharper restore AccessToDisposedClosure
        }

        [Fact]
        public void can_update_referee_grade()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);

            vm.RefereeGrade = RefereeMsgs.Grade.Regional;
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.UpdateGrade>(TimeSpan.FromMilliseconds(300));
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
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);

            vm.RefereeGrade = RefereeMsgs.Grade.Intramural;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.UpdateGrade>(TimeSpan.FromMilliseconds(300)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_intramural_referee_age()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.IntramuralAge == vm.CurrentAge);

            const ushort newAge = RefereeTestHelper.IntramuralAge + 1;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(300));
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
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            Assert.Equal(RefereeMsgs.Grade.Intramural, vm.RefereeGrade);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.IntramuralAge == vm.CurrentAge);

            const ushort newAge = RefereeTestHelper.IntramuralAge;
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(300)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void cannot_update_travel_referee_age()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);

            var newAge = (ushort)RefereeTestHelper.TravelBirthdate.YearsAgo();
            vm.CurrentAge = newAge;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateAge>(TimeSpan.FromMilliseconds(300)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_travel_referee_birthdate()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelBirthdate == vm.Birthdate);

            vm.Birthdate -= TimeSpan.FromDays(1);
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateBirthdate>(TimeSpan.FromMilliseconds(300));
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
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelBirthdate == vm.Birthdate);
            // ReSharper restore AccessToDisposedClosure

            vm.Birthdate = RefereeTestHelper.TravelBirthdate;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateBirthdate>(TimeSpan.FromMilliseconds(300)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_max_age_bracket()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelMaxAgeBracket == vm.MaxAgeBracket);
            // ReSharper restore AccessToDisposedClosure

            const TournamentMsgs.AgeBracket newAgeBracket = TournamentMsgs.AgeBracket.Adult;
            vm.MaxAgeBracket = newAgeBracket;
            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(300));
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
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => RefereeMsgs.Grade.Grassroots == vm.RefereeGrade);
            AssertEx.IsOrBecomesTrue(() => RefereeTestHelper.TravelMaxAgeBracket == vm.MaxAgeBracket);
            // ReSharper restore AccessToDisposedClosure

            const TournamentMsgs.AgeBracket newAgeBracket = RefereeTestHelper.TravelMaxAgeBracket;
            vm.MaxAgeBracket = newAgeBracket;
            vm.Save.Execute().Subscribe();
            Assert.Throws<TimeoutException>(
                () => Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMaxAgeBracket>(TimeSpan.FromMilliseconds(300)));
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_cancel_updating_credentials()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddTravelReferee(refereeId);
            using var vm = new CredentialsVM(refereeId, RefereeTestHelper.FullName, Fixture.Dispatcher, Screen);
            Screen.Router.Navigate.Execute(vm);
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
