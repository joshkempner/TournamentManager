using System;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Helpers;

namespace TournamentManager.Presentation
{
    public sealed class TournamentItemVM : TransientViewModel, IActivatableViewModel
    {
        public ReactiveCommand<Unit, Unit> EditTournament { get; }

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
                    (f, l) => $"{f:MMMM d, yyyy}{(l != f ? $" &#x2013; {l:MMMM d, yyyy}" : "")}")
                .ToProperty(this, x => x.TournamentDates, out _tournamentDates);

            EditTournament = CommandBuilder.FromAction(
                                () => Threading.RunOnUiThread(() =>
                                {
                                    HostScreen.Router.Navigate
                                        .Execute(new TournamentMainVM(
                                                        Id,
                                                        bus,
                                                        HostScreen))
                                        .Subscribe();
                                }));
        }

        private Guid Id { get; }

        public string Name => _name?.Value ?? string.Empty;
        private ObservableAsPropertyHelper<string?> _name = ObservableAsPropertyHelper<string?>.Default();

        public DateTime FirstDay => _firstDay.Value;
        private ObservableAsPropertyHelper<DateTime> _firstDay = ObservableAsPropertyHelper<DateTime>.Default();

        public DateTime LastDay => _lastDay.Value;
        private ObservableAsPropertyHelper<DateTime> _lastDay = ObservableAsPropertyHelper<DateTime>.Default();

        public string TournamentDates => _tournamentDates?.Value ?? string.Empty;
        private readonly ObservableAsPropertyHelper<string?> _tournamentDates;

        public override string UrlPathSegment => "Tournament";
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
