using System;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public sealed class TournamentModel : ReactiveObject
    {
        public TournamentModel(
            Guid id,
            string name,
            DateTime firstDay,
            DateTime lastDay)
        {
            Id = id;
            Name = name;
            FirstDay = firstDay;
            LastDay = lastDay;
        }

        public Guid Id { get; }

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
    }
}
