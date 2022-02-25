using System;
using System.Reactive;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class NewGameVM : OverlayViewModel
    {
        public ReactiveCommand<Unit, Unit> AddGame { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public NewGameVM(
            Guid tournamentId,
            Guid fieldId,
            uint gameDay,
            IDispatcher bus)
        {
            AddGame = CommandBuilder.FromAction(
                            this.WhenAnyValue(
                                x => x.StartTime,
                                x => x.EndTime,
                                x => x.HomeTeam,
                                x => x.AwayTeam,
                                (s, e, h, a) => e > s && h != null && a != null),
                            () =>
                            {
                                var gameId = Guid.NewGuid();
                                var add =
                                    MessageBuilder.New(
                                        () => new GameMsgs.AddGame(
                                                    tournamentId,
                                                    gameId,
                                                    fieldId,
                                                    gameDay,
                                                    StartTime,
                                                    EndTime));
                                bus.Send(add);
                                if (HomeTeam != null)
                                    bus.Send(MessageBuilder
                                                .From(add)
                                                .Build(() => new GameMsgs.UpdateHomeTeam(
                                                                    tournamentId,
                                                                    gameId,
                                                                    HomeTeam.TeamId)));
                                if (AwayTeam != null)
                                    bus.Send(MessageBuilder
                                                .From(add)
                                                .Build(() => new GameMsgs.UpdateAwayTeam(
                                                                    tournamentId,
                                                                    gameId,
                                                                    AwayTeam.TeamId)));
                            });

            Cancel = CommandBuilder.FromAction(() => { /* This is effectively a dummy command */ });

            this.WhenAnyObservable(
                    x => x.AddGame,
                    x => x.Cancel)
                .InvokeCommand(Done);
        }

        public DateTime StartTime
        {
            get => _startTime;
            set => this.RaiseAndSetIfChanged(ref _startTime, value);
        }
        private DateTime _startTime;

        public DateTime EndTime
        {
            get => _endTime;
            set => this.RaiseAndSetIfChanged(ref _endTime, value);
        }
        private DateTime _endTime;

        public TeamModel? HomeTeam
        {
            get => _homeTeam;
            set => this.RaiseAndSetIfChanged(ref _homeTeam, value);
        }
        private TeamModel? _homeTeam;

        public TeamModel? AwayTeam
        {
            get => _awayTeam;
            set => this.RaiseAndSetIfChanged(ref _awayTeam, value);
        }
        private TeamModel? _awayTeam;
    }
}
