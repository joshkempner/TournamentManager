using System;
using System.Linq;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Helpers;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_managing_referees
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _hostScreen = new MockHostScreen();
        private readonly CorrelatedStreamStoreRepository _repo;

        private readonly Guid _ref1Id = Guid.NewGuid();
        private readonly Guid _ref2Id = Guid.NewGuid();

        public when_managing_referees()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));

            _repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
            // Add some referees
            var ref1 = new Referee(
                            _ref1Id,
                            "John",
                            "Smith",
                            RefereeMsgs.Grade.Grassroots,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            ref1.AddOrUpdateEmailAddress("john.smith@aol.com");
            ref1.AddOrUpdateBirthdate(DateTime.Today.AddYears(-71));
            _repo.Save(ref1);
            var ref2 = new Referee(
                            _ref2Id,
                            "Janet",
                            "Jones",
                            RefereeMsgs.Grade.Grassroots,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            ref2.AddOrUpdateEmailAddress("janet.jones@aol.com");
            ref2.AddOrUpdateBirthdate(DateTime.Today.AddYears(-24));
            _repo.Save(ref2);
        }

        [Fact]
        public void can_see_existing_referees()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Referees.Count == 2);
        }

        [Fact]
        public void can_see_newly_added_referee()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            var ref3 = new Referee(
                            Guid.NewGuid(),
                            "Robert",
                            "Smith",
                            RefereeMsgs.Grade.Intramural,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            ref3.AddOrUpdateEmailAddress("bob.smith@aol.com");
            ref3.AddOrUpdateAge(18);
            _repo.Save(ref3);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Referees.Count == 3, 1500);
        }

        [Fact]
        public void can_display_updates_to_referee_data()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Referees.Count == 2);
            foreach (var refereeItemVM in vm.Referees) refereeItemVM.Activator.Activate();

            const string givenName = "Jonathan";
            const string surname = "Smyth";
            const string emailAddress = "jon.smyth@gmail.com";
            const RefereeMsgs.Grade refereeGrade = RefereeMsgs.Grade.Regional;
            var birthdate = DateTime.Today.AddYears(-72);
            const TeamMsgs.AgeBracket maxAge = TeamMsgs.AgeBracket.Adult;
            var ref1 = _repo.GetById<Referee>(_ref1Id, MessageBuilder.New(() => new TestCommands.Command1()));
            ref1.UpdateGivenName(givenName);
            ref1.UpdateSurname(surname);
            ref1.AddOrUpdateEmailAddress(emailAddress);
            ref1.UpdateRefereeGrade(refereeGrade);
            ref1.AddOrUpdateBirthdate(birthdate);
            ref1.AddOrUpdateMaxAgeBracket(maxAge);
            _repo.Save(ref1);

            var referee = vm.Referees.First(x => x.RefereeId == _ref1Id);
            AssertEx.IsOrBecomesTrue(() => referee.GivenName == givenName);
            AssertEx.IsOrBecomesTrue(() => referee.Surname == surname);
            AssertEx.IsOrBecomesTrue(() => referee.EmailAddress.Address == emailAddress);
            AssertEx.IsOrBecomesTrue(() => referee.RefereeGrade == refereeGrade);
            AssertEx.IsOrBecomesTrue(() => referee.AgeRange == RefereeItemVM.AgeToAgeRange((ushort)birthdate.YearsAgo()));
            AssertEx.IsOrBecomesTrue(() => referee.MaxAgeBracket == maxAge);
        }

        [Fact]
        public void can_add_referee()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            vm.AddReferee.Execute().Subscribe();
            IRoutableViewModel currentVM = null;
            vm.HostScreen.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            AssertEx.IsOrBecomesTrue(() => currentVM is NewRefereeVM);
        }
    }
}
