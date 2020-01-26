using System;
using System.Reactive.Linq;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public sealed class NewTournamentVM : TransientViewModel
    {
        public NewTournamentVM(
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            this.WhenAnyValue(x => x.LastDay)
                .Select(x => (int)(x - FirstDay).TotalDays + 1) // 1-day tournament has same first and last days
                .ToProperty(this, x => x.TournamentLength, out _tournamentLength);

            this.WhenAnyValue(x => x.FirstDay)
                .Select(x => x.AddDays(TournamentLength - 1)) // 1-day tournament has same first and last days
                .Subscribe(lastDay => LastDay = lastDay);

            Save = bus.BuildSendCommand(
                    () => MessageBuilder.New(
                            () => new TournamentMsgs.AddTournament(
                                        Guid.NewGuid(),
                                        Name,
                                        FirstDay,
                                        LastDay)));

            FirstDay = DateTime.Today;
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        private string _name = string.Empty;

        public DateTime FirstDay
        {
            get => _firstDay;
            set => this.RaiseAndSetIfChanged(ref _firstDay, value);
        }
        private DateTime _firstDay;

        public DateTime LastDay
        {
            get => _lastDay;
            set => this.RaiseAndSetIfChanged(ref _lastDay, value);
        }
        private DateTime _lastDay;

        public int TournamentLength => _tournamentLength.Value;
        private readonly ObservableAsPropertyHelper<int> _tournamentLength;

        public override string UrlPathSegment => "New Tournament";
    }
}
