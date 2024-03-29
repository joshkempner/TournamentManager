using System;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_using_referee_aggregate
    {
        private readonly Guid _refId = Guid.NewGuid();
        private const string GivenName = "John";
        private const string Surname = "Smith";
        private const RefereeMsgs.Grade Grade = RefereeMsgs.Grade.Grassroots;

        [Fact]
        public void can_add_referee()
        {
            var referee = new Referee(
                                _refId,
                                GivenName,
                                Surname,
                                Grade,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            Assert.True(referee.HasRecordedEvents);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.RefereeAdded evt &&
                                 evt.RefereeId == _refId &&
                                 evt.GivenName == GivenName &&
                                 evt.Surname == Surname &&
                                 evt.RefereeGrade == Grade));
        }

        private Referee AddReferee(RefereeMsgs.Grade grade = RefereeMsgs.Grade.Grassroots)
        {
            var referee = new Referee(
                                _refId,
                                GivenName,
                                Surname,
                                grade,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            referee.TakeEvents();
            ((ICorrelatedEventSource)referee).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return referee;
        }

        [Fact]
        public void cannot_add_referee_with_invalid_id()
        {
            Assert.Throws<ArgumentException>(() => new Referee(
                                                        Guid.Empty,
                                                        GivenName,
                                                        Surname,
                                                        Grade,
                                                        MessageBuilder.New(() => new TestCommands.Command1())));
        }

        [Fact]
        public void cannot_add_referee_with_invalid_correlation()
        {
            Assert.Throws<ArgumentException>(() => new Referee(
                                                        _refId,
                                                        GivenName,
                                                        Surname,
                                                        Grade,
                                                        new TestCommands.Command1()));
        }

        [Fact]
        public void can_update_given_name()
        {
            const string newName = "Joe";
            var referee = AddReferee();
            referee.UpdateGivenName(newName);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.GivenNameChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.GivenName == newName));
        }

        [Fact]
        public void cannot_update_given_name_with_invalid_name()
        {
            var referee = AddReferee();
            Assert.Throws<ArgumentNullException>(() => referee.UpdateGivenName(string.Empty));
            Assert.Throws<ArgumentNullException>(() => referee.UpdateGivenName(" "));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void can_update_surname()
        {
            const string newName = "Jones";
            var referee = AddReferee();
            referee.UpdateSurname(newName);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.SurnameChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.Surname == newName));
        }

        [Fact]
        public void cannot_update_surname_with_invalid_name()
        {
            var referee = AddReferee();
            Assert.Throws<ArgumentNullException>(() => referee.UpdateSurname(string.Empty));
            Assert.Throws<ArgumentNullException>(() => referee.UpdateSurname(" "));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void can_update_referee_grade()
        {
            const RefereeMsgs.Grade newGrade = RefereeMsgs.Grade.Regional;
            var referee = AddReferee();
            referee.UpdateRefereeGrade(newGrade);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.GradeChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.RefereeGrade == newGrade));
        }

        [Fact]
        public void can_add_or_update_birthdate()
        {
            var bd1 = new DateTime(1990, 7, 1);
            var bd2 = new DateTime(1990, 7, 2);
            var referee = AddReferee();
            referee.AddOrUpdateBirthdate(bd1);
            referee.AddOrUpdateBirthdate(bd2);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.BirthdateChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.Birthdate == bd1),
                e => Assert.True(e is RefereeMsgs.BirthdateChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.Birthdate == bd2));
        }

        [Fact]
        public void cannot_add_or_update_birthdate_for_invalid_date()
        {
            var referee = AddReferee();
            var futureDate = DateTime.Today + TimeSpan.FromDays(1);
            Assert.Throws<ArgumentException>(() => referee.AddOrUpdateBirthdate(futureDate));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void can_add_or_update_age()
        {
            const ushort age1 = 14;
            const ushort age2 = 15;
            var referee = AddReferee();
            referee.AddOrUpdateAge(age1);
            referee.AddOrUpdateAge(age2);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.AgeChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.Age == age1),
                e => Assert.True(e is RefereeMsgs.AgeChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.Age == age2));
        }

        [Fact]
        public void cannot_add_or_update_age_to_zero()
        {
            var referee = AddReferee();
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateAge(0));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void can_add_or_update_email_address()
        {
            const string email = "john.smith@aol.com";
            var referee = AddReferee();
            referee.AddOrUpdateEmailAddress(email);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.EmailAddressChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.Email == email));
        }

        [Fact]
        public void cannot_add_or_update_with_invalid_email_address()
        {
            var referee = AddReferee();
            Assert.Throws<FormatException>(() => referee.AddOrUpdateEmailAddress("john.smith"));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void can_add_or_update_mailing_address()
        {
            const string address1 = "21 Main St.";
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "MA";
            const string zip = "01234";
            var referee = AddReferee();
            referee.AddOrUpdateMailingAddress(
                address1,
                string.Empty,
                city,
                state,
                zip);
            referee.AddOrUpdateMailingAddress(
                address1,
                address2,
                city,
                state,
                zip);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.MailingAddressChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.StreetAddress1 == address1 &&
                                 evt.StreetAddress2 == string.Empty &&
                                 evt.City == city &&
                                 evt.State == state &&
                                 evt.ZipCode == zip),
                e => Assert.True(e is RefereeMsgs.MailingAddressChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.StreetAddress1 == address1 &&
                                 evt.StreetAddress2 == address2 &&
                                 evt.City == city &&
                                 evt.State == state &&
                                 evt.ZipCode == zip));
        }

        [Fact]
        public void state_abbreviations_are_normalized()
        {
            const string address1 = "21 Main St.";
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "ma";
            const string zip = "01234";
            var referee = AddReferee();
            referee.AddOrUpdateMailingAddress(
                address1,
                address2,
                city,
                state,
                zip);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.MailingAddressChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.StreetAddress1 == address1 &&
                                 evt.StreetAddress2 == address2 &&
                                 evt.City == city &&
                                 evt.State == state.ToUpperInvariant() &&
                                 evt.ZipCode == zip));
        }

        [Fact]
        public void cannot_add_or_update_mailing_address_with_invalid_street_address()
        {
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "MA";
            const string zip = "01234";
            var referee = AddReferee();
            Assert.Throws<ArgumentNullException>(() => referee.AddOrUpdateMailingAddress(
                                                        string.Empty,
                                                        address2,
                                                        city,
                                                        state,
                                                        zip));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_or_update_mailing_address_with_invalid_State()
        {
            const string address1 = "21 Main St.";
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "ZZ";
            const string zip = "01234";
            var referee = AddReferee();
            Assert.Throws<ArgumentException>(() => referee.AddOrUpdateMailingAddress(
                                                    address1,
                                                    address2,
                                                    city,
                                                    state,
                                                    zip));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_or_update_mailing_address_with_invalid_zip()
        {
            const string address1 = "21 Main St.";
            const string address2 = "Apt 12";
            const string city = "Springfield";
            const string state = "MA";
            var referee = AddReferee();
            Assert.Throws<ArgumentException>(() => referee.AddOrUpdateMailingAddress(
                                                    address1,
                                                    address2,
                                                    city,
                                                    state,
                                                    "012345"));
            Assert.Throws<ArgumentException>(() => referee.AddOrUpdateMailingAddress(
                                                    address1,
                                                    address2,
                                                    city,
                                                    state,
                                                    "0123a"));
            Assert.False(referee.HasRecordedEvents);
        }

        [Fact]
        public void can_add_or_update_max_age_bracket()
        {
            const TournamentMsgs.AgeBracket bracket1 = TournamentMsgs.AgeBracket.U14;
            const TournamentMsgs.AgeBracket bracket2 = TournamentMsgs.AgeBracket.U18;
            var referee = AddReferee();
            referee.AddOrUpdateMaxAgeBracket(bracket1);
            referee.AddOrUpdateMaxAgeBracket(bracket2);
            var events = referee.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is RefereeMsgs.MaxAgeBracketChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.MaxAgeBracket == bracket1),
                e => Assert.True(e is RefereeMsgs.MaxAgeBracketChanged evt &&
                                 evt.RefereeId == _refId &&
                                 evt.MaxAgeBracket == bracket2));
        }

        [Fact]
        public void cannot_set_max_age_above_u8_for_intramural_referee()
        {
            var referee = AddReferee(RefereeMsgs.Grade.Intramural);
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.U10));
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.U12));
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.U14));
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.U16));
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.U18));
            Assert.Throws<ArgumentOutOfRangeException>(() => referee.AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket.Adult));
            Assert.False(referee.HasRecordedEvents);
        }
    }
}
