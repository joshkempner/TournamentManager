using System;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public sealed class TournamentItemVM : TransientViewModel, IActivatableViewModel
    {
        public ReactiveCommand<Unit, IRoutableViewModel> EditTournament { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> ManageTournament { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> ManageTeams { get; }

        public TournamentItemVM(
            IDispatcher bus,
            TournamentModel model,
            IScreen screen)
            : base(screen)
        {
            Id = model.Id;

            this.WhenActivated(disposables =>
            {
                model.WhenAnyValue(x => x.Name)
                    .ToProperty(this, x => x.Name, out _name)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.FirstDay)
                    .ToProperty(this, x => x.FirstDay, out _firstDay)
                    .DisposeWith(disposables);

                model.WhenAnyValue(x => x.LastDay)
                    .ToProperty(this, x => x.LastDay, out _lastDay)
                    .DisposeWith(disposables);
            });

            this.WhenAnyValue(
                    x => x.FirstDay,
                    x => x.LastDay,
                    (f, l) => $"{f:MMMM d, yyyy}{(l != f ? $" \u2013 {l:MMMM d, yyyy}" : "")}")
                .ToProperty(this, x => x.TournamentDates, out _tournamentDates);

            EditTournament = ReactiveCommand.CreateFromObservable(
                                () => HostScreen
                                        .Router
                                        .Navigate
                                        .Execute(new TournamentInfoVM(
                                                        Id,
                                                        bus,
                                                        HostScreen)));

            ManageTournament = ReactiveCommand.CreateFromObservable(
                                () => HostScreen
                                        .Router
                                        .Navigate
                                        .Execute(new TournamentScheduleVM(
                                                        Id,
                                                        bus,
                                                        HostScreen)));

            ManageTeams = ReactiveCommand.CreateFromObservable(
                                () => HostScreen
                                        .Router
                                        .Navigate
                                        .Execute(new TournamentTeamsVM(
                                                        bus,
                                                        Id,
                                                        HostScreen)));
        }

        public Guid Id { get; }

        public string Name => _name?.Value ?? string.Empty;
        private ObservableAsPropertyHelper<string?> _name = ObservableAsPropertyHelper<string?>.Default();

        public DateTime FirstDay => _firstDay.Value;
        private ObservableAsPropertyHelper<DateTime> _firstDay = ObservableAsPropertyHelper<DateTime>.Default();

        public DateTime LastDay => _lastDay.Value;
        private ObservableAsPropertyHelper<DateTime> _lastDay = ObservableAsPropertyHelper<DateTime>.Default();

        public string TournamentDates => _tournamentDates.Value ?? string.Empty;
        private readonly ObservableAsPropertyHelper<string?> _tournamentDates;

        public override string UrlPathSegment => "Tournament";
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
