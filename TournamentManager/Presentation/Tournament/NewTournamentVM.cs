using System;
using System.Reactive;
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
        public ReactiveCommand<Unit, Unit> Save { get; }

        public NewTournamentVM(
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            this.WhenAnyValue(x => x.LastDay)
                .Where(x => x >= FirstDay)
                .Select(x => (int)(x - FirstDay).TotalDays + 1) // 1-day tournament has same first and last days
                .ToProperty(this, x => x.TournamentLength, out _tournamentLength);

            this.WhenAnyValue(x => x.FirstDay)
                .Select(x => x.AddDays(TournamentLength - 1)) // 1-day tournament has same first and last days
                .Subscribe(lastDay => LastDay = lastDay);

            this.WhenAnyValue(x => x.FirstDay)
                .ToProperty(this, x => x.EarliestAllowedLastDay, out _earliestAllowedLastDay);

            this.WhenAnyValue(x => x.LastDay)
                .Where(x => x < EarliestAllowedLastDay)
                .Subscribe(_ => LastDay = EarliestAllowedLastDay);

            this.WhenAnyValue(
                    x => x.Name,
                    x => x.FirstDay,
                    x => x.LastDay,
                    (name, first, last) => !string.IsNullOrWhiteSpace(name) && last >= first)
                .ToProperty(this, x => x.CanAddTournament, out _canAddTournament);

            Save = bus.BuildSendCommand(
                    this.WhenAnyValue(x => x.CanAddTournament),
                    () => MessageBuilder.New(
                            () => new TournamentMsgs.AddTournament(
                                        Guid.NewGuid(),
                                        Name,
                                        FirstDay,
                                        LastDay)));

            this.WhenAnyObservable(x => x.Save)
                .InvokeCommand(Complete);

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

        public DateTime EarliestAllowedLastDay => _earliestAllowedLastDay.Value;
        private readonly ObservableAsPropertyHelper<DateTime> _earliestAllowedLastDay;

        public bool CanAddTournament => _canAddTournament.Value;
        private readonly ObservableAsPropertyHelper<bool> _canAddTournament;

        public int TournamentLength => _tournamentLength.Value;
        private readonly ObservableAsPropertyHelper<int> _tournamentLength;

        public override string UrlPathSegment => "New Tournament";
    }
}
