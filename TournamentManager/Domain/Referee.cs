using System;
using System.Net.Mail;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Util;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Domain
{
    public class Referee : AggregateRoot
    {
        private RefereeMsgs.Grade _refereeGrade;

        public Referee(
            Guid refereeId,
            string givenName,
            string surname,
            RefereeMsgs.Grade grade,
            ICorrelatedMessage source)
            : this()
        {
            Ensure.NotEmptyGuid(refereeId, nameof(refereeId));
            Ensure.NotEmptyGuid(source.CorrelationId, nameof(source.CorrelationId));
            Ensure.NotEmptyGuid(source.CorrelationId, nameof(source.CorrelationId));
            if (source.CausationId == Guid.Empty)
                Ensure.NotEmptyGuid(source.MsgId, nameof(source.MsgId));
            ((ICorrelatedEventSource)this).Source = source;
            Raise(new RefereeMsgs.RefereeAdded(
                        refereeId,
                        givenName,
                        surname,
                        grade));
        }

        private Referee()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Register<RefereeMsgs.RefereeAdded>(e =>
            {
                Id = e.RefereeId;
                _refereeGrade = e.RefereeGrade;
            });
            Register<RefereeMsgs.GradeChanged>(e => _refereeGrade = e.RefereeGrade);
        }

        public void UpdateGivenName(string newName)
        {
            Ensure.NotNullOrEmpty(newName.Trim(), nameof(newName));
            Raise(new RefereeMsgs.GivenNameChanged(
                        Id,
                        newName));
        }

        public void UpdateSurname(string newName)
        {
            Ensure.NotNullOrEmpty(newName.Trim(), nameof(newName));
            Raise(new RefereeMsgs.SurnameChanged(
                        Id,
                        newName));
        }

        public void UpdateRefereeGrade(RefereeMsgs.Grade grade)
        {
            Raise(new RefereeMsgs.GradeChanged(
                        Id,
                        grade));
        }

        public void AddOrUpdateBirthdate(DateTime birthdate)
        {
            Ensure.LessThan(DateTime.Today.Ticks, birthdate.Ticks, nameof(birthdate));
            Raise(new RefereeMsgs.BirthdateChanged(
                        Id,
                        birthdate));
        }

        public void AddOrUpdateAge(ushort age)
        {
            Ensure.Positive(age, nameof(age));
            Raise(new RefereeMsgs.AgeChanged(
                        Id,
                        age));
        }

        public void AddOrUpdateEmailAddress(string emailAddress)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new MailAddress(emailAddress); // performs validation on the provided address.
            Raise(new RefereeMsgs.EmailAddressChanged(
                        Id,
                        emailAddress));
        }

        public void AddOrUpdateMailingAddress(
            string streetAddress1,
            string streetAddress2,
            string city,
            string state,
            string zipCode)
        {
            var normalizedState = state.ToUpperInvariant();
            Ensure.NotNullOrEmpty(streetAddress1, nameof(streetAddress1));
            Ensure.NotNullOrEmpty(city, nameof(city));
            Ensure.True(() => StringUtilities.IsValidStateAbbreviation(normalizedState), nameof(state));
            Ensure.True(() => StringUtilities.IsValidUSZipCode(zipCode), nameof(zipCode));
            Raise(new RefereeMsgs.MailingAddressChanged(
                        Id,
                        streetAddress1,
                        streetAddress2,
                        city,
                        normalizedState,
                        zipCode));
        }

        public void AddOrUpdateMaxAgeBracket(TournamentMsgs.AgeBracket ageBracket)
        {
            if (_refereeGrade == RefereeMsgs.Grade.Intramural && ageBracket > TournamentMsgs.AgeBracket.U8)
                throw new ArgumentOutOfRangeException(nameof(ageBracket),
                    "Intramural referees cannot do games above U8.");
            Raise(new RefereeMsgs.MaxAgeBracketChanged(
                        Id,
                        ageBracket));
        }
    }
}
