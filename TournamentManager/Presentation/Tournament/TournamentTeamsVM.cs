using System;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TournamentTeamsVM : TransientViewModel
    {
        private readonly TournamentTeamsRM _rm;

        public ReactiveCommand<Unit, Unit> AddTeam { get; }

        public TournamentTeamsVM(
            IDispatcher bus,
            Guid tournamentId,
            IScreen screen)
            : base(screen)
        {
            _newTeamName = string.Empty;

            _rm = new TournamentTeamsRM(tournamentId);

            _rm.TournamentTeams
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new TeamItemVM(tournamentId, x, bus))
                .Bind(Teams)
                .DisposeMany()
                .Subscribe();

            AddTeam = CommandBuilder.FromAction(
                            this.WhenAnyValue(
                                x => x.NewTeamName,
                                name => !string.IsNullOrWhiteSpace(name) && !_rm.IsTeamInTournament(name)),
                            () =>
                            {
                                var team = _rm.GetTeam(NewTeamName);
                                var teamId = team?.TeamId ?? Guid.NewGuid();
                                if (team is null)
                                {
                                    var cmd = MessageBuilder.New(
                                                () => new TeamMsgs.CreateTeam(
                                                            teamId,
                                                            NewTeamName));
                                    bus.Send(cmd);
                                    bus.Send(MessageBuilder
                                                .From(cmd)
                                                .Build(() => new TournamentMsgs.AddTeamToTournament(
                                                                    tournamentId,
                                                                    teamId,
                                                                    NewTeamAgeBracket)));
                                }
                                else
                                {
                                    bus.Send(MessageBuilder
                                                 .New(() => new TournamentMsgs.AddTeamToTournament(
                                                                    tournamentId,
                                                                    teamId,
                                                                    NewTeamAgeBracket)));
                                }
                                Threading.RunOnUiThread(() => NewTeamName = string.Empty);
                            });
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _rm.Dispose();
            _disposed = true;
            base.Dispose(disposing);
        }

        public IObservableCollection<TeamItemVM> Teams { get; } = new ObservableCollectionExtended<TeamItemVM>();

        public string NewTeamName
        {
            get => _newTeamName;
            set => this.RaiseAndSetIfChanged(ref _newTeamName, value);
        }
        private string _newTeamName;

        public TournamentMsgs.AgeBracket NewTeamAgeBracket
        {
            get => _newTeamAgeBracket;
            set => this.RaiseAndSetIfChanged(ref _newTeamAgeBracket, value);
        }
        private TournamentMsgs.AgeBracket _newTeamAgeBracket;

        public override string UrlPathSegment => "TournamentTeams";
    }
}
