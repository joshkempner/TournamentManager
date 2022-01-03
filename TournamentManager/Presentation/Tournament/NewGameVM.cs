using System;
using System.Reactive;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class NewGameVM : ReactiveObject, IDisposable
    {
        public ReactiveCommand<Unit, Unit> AddGame { get; }

        public NewGameVM(
            Guid tournamentId,
            Guid fieldId,
            uint gameDay,
            IDispatcher bus,
            Action<IDisposable> close)
        {
            AddGame = bus.BuildSendCommand(
                            this.WhenAnyValue(
                                x => x.StartTime,
                                x => x.EndTime,
                                x => x.HomeTeam,
                                x => x.AwayTeam,
                                (s, e, h, a) => e > s && h != null && a != null),
                            () => MessageBuilder.New(
                                    () => new GameMsgs.AddGame(
                                                tournamentId,
                                                Guid.NewGuid(),
                                                fieldId,
                                                gameDay,
                                                StartTime,
                                                EndTime,
                                                HomeTeam!.TeamId,
                                                AwayTeam!.TeamId)));

            this.WhenAnyObservable(x => x.AddGame)
                .Subscribe(_ => close(this));
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

        public void Dispose()
        { }
    }
}
