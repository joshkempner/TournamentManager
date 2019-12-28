using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests
{
    public class when_using_referee_service : IDisposable
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly RefereeSvc _svc;

        private readonly Guid _refId = Guid.NewGuid();
        private const string GivenName = "John";
        private const string Surname = "Smith";
        private const RefereeMsgs.Grade Grade = RefereeMsgs.Grade.Grassroots;

        public when_using_referee_service()
        {
            _svc = new RefereeSvc(_fixture.Dispatcher, _fixture.Repository);
        }

        public void Dispose()
        {
            _svc?.Dispose();
        }

        private void AddReferee()
        {
            var referee = new Referee(
                                _refId,
                                GivenName,
                                Surname,
                                Grade,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            var repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
            repo.Save(referee);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.RefereeAdded>(TimeSpan.FromMilliseconds(100));
            _fixture.ClearQueues();
        }

        [Fact]
        public void can_add_referee()
        {
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddReferee(
                                                    _refId,
                                                    GivenName,
                                                    Surname,
                                                    Grade));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.RefereeAdded>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddReferee>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.RefereeAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(GivenName, evt.GivenName);
            Assert.Equal(Surname, evt.Surname);
            Assert.Equal(Grade, evt.RefereeGrade);
        }

        [Fact]
        public void can_update_referee_given_name()
        {
            AddReferee();

            const string newName = "Joe";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.UpdateGivenName(
                                                    _refId,
                                                    newName));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.GivenNameChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateGivenName>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.GivenNameChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(newName, evt.GivenName);
        }

        [Fact]
        public void cannot_update_given_name_for_invalid_referee()
        {
            AddReferee();

            const string newName = "Joe";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.UpdateGivenName(
                                                        Guid.NewGuid(),
                                                        newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.UpdateGivenName(
                                                        Guid.Empty,
                                                        newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateGivenName>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.UpdateGivenName>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_referee_surname()
        {
            AddReferee();

            const string newName = "Jones";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.UpdateSurname(
                                                    _refId,
                                                    newName));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.SurnameChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateSurname>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.SurnameChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(newName, evt.Surname);
        }

        [Fact]
        public void cannot_update_surname_for_invalid_referee()
        {
            AddReferee();

            const string newName = "Jones";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.UpdateSurname(
                                                        Guid.NewGuid(),
                                                        newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.UpdateSurname(
                                                        Guid.Empty,
                                                        newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateSurname>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.UpdateSurname>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_referee_grade()
        {
            AddReferee();

            const RefereeMsgs.Grade newGrade = RefereeMsgs.Grade.Regional;
            var cmd = MessageBuilder.New(() => new RefereeMsgs.UpdateGrade(
                                                    _refId,
                                                    newGrade));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.GradeChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateGrade>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.GradeChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(newGrade, evt.RefereeGrade);
        }

        [Fact]
        public void cannot_update_referee_grade_for_invalid_referee()
        {
            AddReferee();

            const RefereeMsgs.Grade newGrade = RefereeMsgs.Grade.Regional;
            var cmd = MessageBuilder.New(() => new RefereeMsgs.UpdateGrade(
                                                        Guid.NewGuid(),
                                                        newGrade));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.UpdateGrade(
                                                        Guid.Empty,
                                                        newGrade));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.UpdateGrade>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.UpdateGrade>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_or_update_birthdate()
        {
            AddReferee();

            var birthdate = new DateTime(1990, 7, 1);
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateBirthdate(
                                                    _refId,
                                                    birthdate));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.BirthdateChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateBirthdate>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.BirthdateChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(birthdate, evt.Birthdate);
        }

        [Fact]
        public void cannot_add_or_update_birthdate_for_invalid_referee()
        {
            AddReferee();

            var birthdate = new DateTime(1990, 7, 1);
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateBirthdate(
                                                        Guid.NewGuid(),
                                                        birthdate));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateBirthdate(
                                                        Guid.Empty,
                                                        birthdate));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateBirthdate>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateBirthdate>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_or_update_age()
        {
            AddReferee();

            const ushort age = 14;
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateAge(
                                                    _refId,
                                                    age));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.AgeChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateAge>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.AgeChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(age, evt.Age);
        }

        [Fact]
        public void cannot_add_or_update_age_for_invalid_referee()
        {
            AddReferee();

            const ushort age = 14;
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateAge(
                                                        Guid.NewGuid(),
                                                        age));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateAge(
                                                        Guid.Empty,
                                                        age));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateAge>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateAge>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_or_update_email_address()
        {
            AddReferee();

            const string emailAddress = "john.smith@aol.com";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateEmailAddress(
                                                    _refId,
                                                    emailAddress));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.EmailAddressChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateEmailAddress>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.EmailAddressChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(emailAddress, evt.Email);
        }

        [Fact]
        public void cannot_add_or_update_email_address_for_invalid_referee()
        {
            AddReferee();

            const string emailAddress = "john.smith@aol.com";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateEmailAddress(
                                                        Guid.NewGuid(),
                                                        emailAddress));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateEmailAddress(
                                                        Guid.Empty,
                                                        emailAddress));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateEmailAddress>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateEmailAddress>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_or_update_mailing_address()
        {
            AddReferee();

            const string address1 = "21 Main St.";
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "MA";
            const string zip = "01234";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMailingAddress(
                                                    _refId,
                                                    address1,
                                                    address2,
                                                    city,
                                                    state,
                                                    zip));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.MailingAddressChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMailingAddress>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.MailingAddressChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(address1, evt.StreetAddress1);
            Assert.Equal(address2, evt.StreetAddress2);
            Assert.Equal(city, evt.City);
            Assert.Equal(state, evt.State);
            Assert.Equal(zip, evt.ZipCode);
        }

        [Fact]
        public void cannot_add_or_update_mailing_address_for_invalid_referee()
        {
            AddReferee();

            const string address1 = "21 Main St.";
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "MA";
            const string zip = "01234";
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMailingAddress(
                                                        Guid.NewGuid(),
                                                        address1,
                                                        address2,
                                                        city,
                                                        state,
                                                        zip));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMailingAddress(
                                                        Guid.Empty,
                                                        address1,
                                                        address2,
                                                        city,
                                                        state,
                                                        zip));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMailingAddress>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateMailingAddress>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_or_update_max_age_bracket()
        {
            AddReferee();

            const TeamMsgs.AgeBracket bracket = TeamMsgs.AgeBracket.U14;
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMaxAgeBracket(
                                                    _refId,
                                                    bracket));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<RefereeMsgs.MaxAgeBracketChanged>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<RefereeMsgs.MaxAgeBracketChanged>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_refId, evt.RefereeId);
            Assert.Equal(bracket, evt.MaxAgeBracket);
        }

        [Fact]
        public void cannot_add_or_update_max_age_bracket_for_invalid_referee()
        {
            AddReferee();

            const TeamMsgs.AgeBracket bracket = TeamMsgs.AgeBracket.U14;
            var cmd = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMaxAgeBracket(
                                                        Guid.NewGuid(),
                                                        bracket));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            var cmd2 = MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMaxAgeBracket(
                                                        Guid.Empty,
                                                        bracket));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd2));
            _fixture
                .TestQueue
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd.CorrelationId)
                .AssertNext<RefereeMsgs.AddOrUpdateMaxAgeBracket>(cmd2.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }
    }
}
