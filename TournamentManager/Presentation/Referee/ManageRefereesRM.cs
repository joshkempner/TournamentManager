﻿using System;
using System.Linq;
using DynamicData;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class ManageRefereesRM :
        ReadModelBase,
        IHandle<RefereeMsgs.RefereeAdded>,
        IHandle<RefereeMsgs.EmailAddressChanged>,
        IHandle<RefereeMsgs.GradeChanged>,
        IHandle<RefereeMsgs.AgeChanged>,
        IHandle<RefereeMsgs.BirthdateChanged>,
        IHandle<RefereeMsgs.MaxAgeBracketChanged>
    {
        public ManageRefereesRM()
            : base(
                nameof(ManageRefereesRM),
                () => Locator.Current.GetService<IStreamStoreConnection>().GetListener(nameof(ManageRefereesRM)))
        {
            EventStream.Subscribe<RefereeMsgs.RefereeAdded>(this);
            EventStream.Subscribe<RefereeMsgs.EmailAddressChanged>(this);
            EventStream.Subscribe<RefereeMsgs.GradeChanged>(this);
            EventStream.Subscribe<RefereeMsgs.AgeChanged>(this);
            EventStream.Subscribe<RefereeMsgs.BirthdateChanged>(this);
            EventStream.Subscribe<RefereeMsgs.MaxAgeBracketChanged>(this);
            Start<Referee>();
        }

        public SourceCache<RefereeModel, Guid> Referees { get; } = new SourceCache<RefereeModel, Guid>(x => x.RefereeId);

        public void Handle(RefereeMsgs.RefereeAdded message)
        {
            Referees.AddOrUpdate(new RefereeModel(
                                        message.RefereeId,
                                        message.GivenName,
                                        message.Surname));
        }

        public void Handle(RefereeMsgs.EmailAddressChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.EmailAddress = message.Email;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.GradeChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.RefereeGrade = message.RefereeGrade;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.AgeChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.CurrentAge = (ushort)message.Age;
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.BirthdateChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.CurrentAge = (ushort)((DateTime.Today - message.Birthdate).TotalDays / 365.25);
            Referees.AddOrUpdate(referee);
        }

        public void Handle(RefereeMsgs.MaxAgeBracketChanged message)
        {
            var referee = Referees.Items.First(x => x.RefereeId == message.RefereeId);
            referee.MaxAgeBracket = message.MaxAgeBracket;
            Referees.AddOrUpdate(referee);
        }
    }
}