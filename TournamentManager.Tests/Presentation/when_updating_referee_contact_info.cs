using System;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Testing;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public class when_updating_referee_contact_info :
        with_vm_fixtures,
        IDisposable,
        IHandleCommand<RefereeMsgs.AddOrUpdateEmailAddress>,
        IHandleCommand<RefereeMsgs.AddOrUpdateMailingAddress>
    {
        public when_updating_referee_contact_info()
        {
            Fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateEmailAddress>(this);
            Fixture.Dispatcher.Subscribe<RefereeMsgs.AddOrUpdateMailingAddress>(this);
        }

        [Fact]
        public void can_update_email_address()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new ContactInfoVM(
                                refereeId,
                                RefereeTestHelper.FullName,
                                Fixture.Dispatcher,
                                Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);

            const string newEmail = "john.smith@aol.com";
            vm.EmailAddress = newEmail;
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.CanSave);

            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateEmailAddress>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateEmailAddress>(cmd => cmd.RefereeId == refereeId &&
                                                                        cmd.Email == newEmail)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void cannot_update_if_email_address_is_invalid()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new ContactInfoVM(
                                refereeId,
                                RefereeTestHelper.FullName,
                                Fixture.Dispatcher,
                                Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);

            const string newEmail = "john.smith";
            vm.EmailAddress = newEmail;
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesFalse(() => vm.CanSave);
        }

        [Fact]
        public void can_update_mailing_address()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new ContactInfoVM(
                                refereeId,
                                RefereeTestHelper.FullName,
                                Fixture.Dispatcher,
                                Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);

            vm.StreetAddress1 = "1 Main St.";
            vm.City = "Springfield";
            vm.SelectedStateName = "Massachusetts";
            vm.ZipCode = "01234";
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.CanSave);

            vm.Save.Execute().Subscribe();
            Fixture.TestQueue.WaitFor<RefereeMsgs.AddOrUpdateMailingAddress>(TimeSpan.FromMilliseconds(200));
            // ReSharper disable AccessToDisposedClosure
            Fixture
                            .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMailingAddress>(cmd => cmd.RefereeId == refereeId &&
                                                                          cmd.StreetAddress1 == vm.StreetAddress1 &&
                                                                          cmd.StreetAddress2 == vm.StreetAddress2 &&
                                                                          cmd.City == vm.City &&
                                                                          cmd.State == vm.StateAbbreviation &&
                                                                          cmd.ZipCode == vm.ZipCode)
                .AssertEmpty();
            // ReSharper restore AccessToDisposedClosure
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void cannot_update_while_mailing_address_is_incomplete()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new ContactInfoVM(
                refereeId,
                RefereeTestHelper.FullName,
                Fixture.Dispatcher,
                Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);

            vm.StreetAddress1 = string.Empty;
            vm.City = "Springfield";
            vm.SelectedStateName = "Massachusetts";
            vm.ZipCode = "01234";
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesFalse(() => vm.CanSave);

            vm.StreetAddress1 = "1 Main St.";
            vm.City = string.Empty;
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesFalse(() => vm.CanSave);

            vm.City = "Springfield";
            vm.SelectedStateName = string.Empty;
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesFalse(() => vm.CanSave);

            vm.SelectedStateName = "Massachusetts";
            vm.ZipCode = string.Empty;
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesFalse(() => vm.CanSave);
        }

        [Fact]
        public void cannot_update_if_zip_code_is_invalid()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new ContactInfoVM(
                refereeId,
                RefereeTestHelper.FullName,
                Fixture.Dispatcher,
                Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);

            vm.StreetAddress1 = "1 Main St.";
            vm.City = "Springfield";
            vm.SelectedStateName = "Massachusetts";
            vm.ZipCode = "0123";
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesFalse(() => vm.CanSave);
        }

        [Fact]
        public void can_cancel_updating_contact_info()
        {
            var refereeId = Guid.NewGuid();
            RefereeTestHelper.AddIntramuralReferee(refereeId);
            using var vm = new ContactInfoVM(
                refereeId,
                RefereeTestHelper.FullName,
                Fixture.Dispatcher,
                Screen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.FullName == RefereeTestHelper.FullName);

            vm.Cancel.Execute().Subscribe();
            Fixture.TestQueue.AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        public void Dispose()
        {
            Fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateEmailAddress>(this);
            Fixture.Dispatcher.Unsubscribe<RefereeMsgs.AddOrUpdateMailingAddress>(this);
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateEmailAddress command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateMailingAddress command)
        {
            return command.Succeed();
        }
    }
}
