using System;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class GameModel : ReactiveObject
    {
        public GameModel(
            Guid gameId,
            DateTime startTime,
            DateTime endTime,
            Guid fieldId,
            string fieldName)
        {
            GameId = gameId;
            StartTime = startTime;
            EndTime = endTime;
            FieldId = fieldId;
            FieldName = fieldName;
        }

        public Guid GameId { get; }

        public Guid HomeTeamId
        {
            get => _homeTeamId;
            set => this.RaiseAndSetIfChanged(ref _homeTeamId, value);
        }
        private Guid _homeTeamId;

        public Guid AwayTeamId
        {
            get => _awayTeamId;
            set => this.RaiseAndSetIfChanged(ref _awayTeamId, value);
        }
        private Guid _awayTeamId;

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

        public Guid FieldId { get; }
        public string FieldName { get; }
    }
}
