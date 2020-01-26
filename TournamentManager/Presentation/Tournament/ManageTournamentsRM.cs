using System;
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
    public sealed class ManageTournamentsRM :
        ReadModelBase,
        IHandle<TournamentMsgs.TournamentAdded>,
        IHandle<TournamentMsgs.TournamentRenamed>,
        IHandle<TournamentMsgs.TournamentRescheduled>
    {
        public ManageTournamentsRM()
            : base(
                nameof(ManageTournamentsRM),
                () => Locator.Current.GetService<IStreamStoreConnection>().GetListener(nameof(ManageTournamentsRM)))
        {
            EventStream.Subscribe<TournamentMsgs.TournamentAdded>(this);
            EventStream.Subscribe<TournamentMsgs.TournamentRenamed>(this);
            EventStream.Subscribe<TournamentMsgs.TournamentRescheduled>(this);
            Start<Tournament>();
        }

        public SourceCache<TournamentModel, Guid> Tournaments { get; } = new SourceCache<TournamentModel, Guid>(x => x.Id);

        public void Handle(TournamentMsgs.TournamentAdded message)
        {
            Tournaments.AddOrUpdate(new TournamentModel(
                                            message.TournamentId,
                                            message.Name,
                                            message.FirstDay,
                                            message.LastDay));
        }

        public void Handle(TournamentMsgs.TournamentRenamed message)
        {
            var tournament = Tournaments.Items.FirstOrDefault(x => x.Id == message.TournamentId);
            if (tournament == null) return;
            tournament.Name = message.Name;
            Tournaments.AddOrUpdate(tournament);
        }

        public void Handle(TournamentMsgs.TournamentRescheduled message)
        {
            var tournament = Tournaments.Items.FirstOrDefault(x => x.Id == message.TournamentId);
            if (tournament == null) return;
            tournament.FirstDay = message.FirstDay;
            tournament.LastDay = message.LastDay;
            Tournaments.AddOrUpdate(tournament);
        }
    }
}
