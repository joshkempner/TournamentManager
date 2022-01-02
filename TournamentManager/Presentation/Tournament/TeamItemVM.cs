using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TeamItemVM : ReactiveObject
    {
        private readonly Guid _tournamentId;
        private readonly IDispatcher _bus;

        public ReactiveCommand<Unit, Unit> RemoveTeam { get; }
        public Interaction<Unit, bool> ConfirmRemove { get; } = new Interaction<Unit, bool>();

        public ReactiveCommand<object, Unit> ChangeAgeBracket { get; }

        public TeamItemVM(
            Guid tournamentId,
            TeamModel model,
            IDispatcher bus)
        {
            _tournamentId = tournamentId;
            _bus = bus;
            Model = model;
            TeamId = model.TeamId;

            this.WhenAnyValue(x => x.Model.Name)
                .ToProperty(this, x => x.Name, out _name);

            this.WhenAnyValue(x => x.Model.AgeBracket)
                .Subscribe(x =>
                {
                    _lastSavedAgeBracket = x;
                    AgeBracket = x;
                });

            RemoveTeam = ReactiveCommand.CreateFromTask(RemoveTeamFromTournament);

            ChangeAgeBracket = bus.BuildSendCommandEx(
                                    this.WhenAnyValue(x => x.AgeBracket, a => a != _lastSavedAgeBracket),
                                    x => MessageBuilder
                                            .New(() => new TournamentMsgs.ChangeTeamAgeBracket(
                                                            tournamentId,
                                                            TeamId,
                                                            (TournamentMsgs.AgeBracket)x)));

            this.WhenAnyValue(x => x.AgeBracket)
                .InvokeCommand(ChangeAgeBracket);
        }

        private async Task RemoveTeamFromTournament()
        {
            var confirm = await ConfirmRemove.Handle(Unit.Default);
            if (confirm)
                _bus.Send(MessageBuilder.New(() => new TournamentMsgs.RemoveTeamFromTournament(_tournamentId, TeamId)));
        }

        public TeamModel Model { get; }

        public Guid TeamId { get; }

        public string Name => _name.Value;
        private readonly ObservableAsPropertyHelper<string> _name;

        public TournamentMsgs.AgeBracket AgeBracket
        {
            get => _ageBracket;
            set => this.RaiseAndSetIfChanged(ref _ageBracket, value);
        }
        private TournamentMsgs.AgeBracket _ageBracket;

        private TournamentMsgs.AgeBracket _lastSavedAgeBracket;
    }
}
