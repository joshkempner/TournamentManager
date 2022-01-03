using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public sealed class TournamentInfoRM :
        ReadModelBase,
        IHandle<TournamentMsgs.TournamentAdded>,
        IHandle<TournamentMsgs.TournamentRenamed>,
        IHandle<TournamentMsgs.TournamentRescheduled>
    {
        public TournamentInfoRM(Guid tournamentId)
            : base(
                nameof(TournamentInfoRM),
                () => Bootstrap.GetListener(nameof(TournamentInfoRM)))
        {
            EventStream.Subscribe<TournamentMsgs.TournamentAdded>(this);
            EventStream.Subscribe<TournamentMsgs.TournamentRenamed>(this);
            EventStream.Subscribe<TournamentMsgs.TournamentRescheduled>(this);
            Start<Domain.Tournament>(tournamentId);
        }

        public IObservable<string> TournamentName => _tournamentName;
        private readonly ReadModelProperty<string> _tournamentName = new ReadModelProperty<string>(string.Empty);

        public IObservable<DateTime> FirstDay => _firstDay;
        private readonly ReadModelProperty<DateTime> _firstDay = new ReadModelProperty<DateTime>(default);

        public IObservable<DateTime> LastDay => _lastDay;
        private readonly ReadModelProperty<DateTime> _lastDay = new ReadModelProperty<DateTime>(default);

        public void Handle(TournamentMsgs.TournamentAdded message)
        {
            _tournamentName.Update(message.Name);
            _firstDay.Update(message.FirstDay);
            _lastDay.Update(message.LastDay);
        }

        public void Handle(TournamentMsgs.TournamentRenamed message)
        {
            _tournamentName.Update(message.Name);
        }

        public void Handle(TournamentMsgs.TournamentRescheduled message)
        {
            _firstDay.Update(message.FirstDay);
            _lastDay.Update(message.LastDay);
        }
    }
}
