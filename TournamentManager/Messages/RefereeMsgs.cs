using System;
using ReactiveDomain.Messaging;

namespace TournamentManager.Messages
{
    public class RefereeMsgs
    {
        public enum Grade
        {
            Intramural,
            Grassroots,
            Regional
        }

        public class AddReferee : Command
        {
            public readonly Guid RefereeId;
            public readonly string GivenName;
            public readonly string Surname;
            public readonly Grade RefereeGrade;

            public AddReferee(
                Guid refereeId,
                string givenName,
                string surname,
                Grade refereeGrade)
            {
                RefereeId = refereeId;
                GivenName = givenName;
                Surname = surname;
                RefereeGrade = refereeGrade;
            }
        }

        public class RefereeAdded : Event
        {
            public readonly Guid RefereeId;
            public readonly string GivenName;
            public readonly string Surname;
            public readonly Grade RefereeGrade;

            public RefereeAdded(
                Guid refereeId,
                string givenName,
                string surname,
                Grade refereeGrade)
            {
                RefereeId = refereeId;
                GivenName = givenName;
                Surname = surname;
                RefereeGrade = refereeGrade;
            }
        }

        public class UpdateGivenName : Command
        {
            public readonly Guid RefereeId;
            public readonly string GivenName;

            public UpdateGivenName(
                Guid refereeId,
                string givenName)
            {
                RefereeId = refereeId;
                GivenName = givenName;
            }
        }

        public class GivenNameChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly string GivenName;

            public GivenNameChanged(
                Guid refereeId,
                string givenName)
            {
                RefereeId = refereeId;
                GivenName = givenName;
            }
        }

        public class UpdateSurname : Command
        {
            public readonly Guid RefereeId;
            public readonly string Surname;

            public UpdateSurname(
                Guid refereeId,
                string surname)
            {
                RefereeId = refereeId;
                Surname = surname;
            }
        }

        public class SurnameChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly string Surname;

            public SurnameChanged(
                Guid refereeId,
                string surname)
            {
                RefereeId = refereeId;
                Surname = surname;
            }
        }

        public class UpdateGrade : Command
        {
            public readonly Guid RefereeId;
            public readonly Grade RefereeGrade;

            public UpdateGrade(
                Guid refereeId,
                Grade refereeGrade)
            {
                RefereeId = refereeId;
                RefereeGrade = refereeGrade;
            }
        }
        
        public class GradeChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly Grade RefereeGrade;

            public GradeChanged(
                Guid refereeId,
                Grade refereeGrade)
            {
                RefereeId = refereeId;
                RefereeGrade = refereeGrade;
            }
        }

        public class AddOrUpdateBirthdate : Command
        {
            public readonly Guid RefereeId;
            public readonly DateTime Birthdate;

            public AddOrUpdateBirthdate(
                Guid refereeId,
                DateTime birthdate)
            {
                RefereeId = refereeId;
                Birthdate = birthdate;
            }
        }

        public class BirthdateChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly DateTime Birthdate;

            public BirthdateChanged(
                Guid refereeId,
                DateTime birthdate)
            {
                RefereeId = refereeId;
                Birthdate = birthdate;
            }
        }

        public class AddOrUpdateAge : Command
        {
            public readonly Guid RefereeId;
            public readonly ushort Age;

            public AddOrUpdateAge(
                Guid refereeId,
                ushort age)
            {
                RefereeId = refereeId;
                Age = age;
            }
        }

        public class AgeChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly int Age;

            public AgeChanged(
                Guid refereeId,
                int age)
            {
                RefereeId = refereeId;
                Age = age;
            }
        }

        public class AddOrUpdateEmailAddress : Command
        {
            public readonly Guid RefereeId;
            public readonly string Email;

            public AddOrUpdateEmailAddress(
                Guid refereeId,
                string email)
            {
                RefereeId = refereeId;
                Email = email;
            }
        }

        public class EmailAddressChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly string Email;

            public EmailAddressChanged(
                Guid refereeId,
                string email)
            {
                RefereeId = refereeId;
                Email = email;
            }
        }

        public class AddOrUpdateMailingAddress : Command
        {
            public readonly Guid RefereeId;
            public readonly string StreetAddress1;
            public readonly string StreetAddress2;
            public readonly string City;
            public readonly string State;
            public readonly string ZipCode;

            public AddOrUpdateMailingAddress(
                Guid refereeId,
                string streetAddress1,
                string streetAddress2,
                string city,
                string state,
                string zipCode)
            {
                RefereeId = refereeId;
                StreetAddress1 = streetAddress1;
                StreetAddress2 = streetAddress2;
                City = city;
                State = state;
                ZipCode = zipCode;
            }
        }

        public class MailingAddressChanged : Event
        {
            public readonly Guid RefereeId;
            public readonly string StreetAddress1;
            public readonly string StreetAddress2;
            public readonly string City;
            public readonly string State;
            public readonly string ZipCode;

            public MailingAddressChanged(
                Guid refereeId,
                string streetAddress1,
                string streetAddress2,
                string city,
                string state,
                string zipCode)
            {
                RefereeId = refereeId;
                StreetAddress1 = streetAddress1;
                StreetAddress2 = streetAddress2;
                City = city;
                State = state;
                ZipCode = zipCode;
            }
        }
    }
}
