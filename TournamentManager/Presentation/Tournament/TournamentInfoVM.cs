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
    public sealed class TournamentInfoVM : TransientViewModel
    {
        private readonly TournamentInfoRM _rm;

        public TournamentInfoVM(
            Guid tournamentId,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            Id = tournamentId;
            _rm = new TournamentInfoRM(tournamentId);

            _rm.TournamentName
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedName = x;
                    Name = x;
                });

            _rm.FirstDay
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedFirstDay = x;
                    FirstDay = x;
                });

            _rm.LastDay
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedLastDay = x;
                    LastDay = x;
                });

            Save = CommandBuilder.FromAction(
                    () =>
                    {
                        if (Name != LastSavedName)
                            bus.Send(MessageBuilder.New(() => new TournamentMsgs.RenameTournament(
                                                                    tournamentId,
                                                                    Name)));
                        if (LastSavedFirstDay != FirstDay || LastSavedLastDay != LastDay)
                            bus.Send(MessageBuilder.New(() => new TournamentMsgs.RescheduleTournament(
                                                                    tournamentId,
                                                                    FirstDay,
                                                                    LastDay)));
                    });
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_disposed) return;
            if (disposing)
                _rm.Dispose();
            _disposed = true;
        }

        private Guid Id { get; }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        private string _name = string.Empty;

        public string LastSavedName
        {
            get => _lastSavedName;
            set => this.RaiseAndSetIfChanged(ref _lastSavedName, value);
        }
        private string _lastSavedName = string.Empty;

        public DateTime FirstDay
        {
            get => _firstDay;
            set => this.RaiseAndSetIfChanged(ref _firstDay, value);
        }
        private DateTime _firstDay;

        public DateTime LastSavedFirstDay
        {
            get => _lastSavedFirstDay;
            set => this.RaiseAndSetIfChanged(ref _lastSavedFirstDay, value);
        }
        private DateTime _lastSavedFirstDay;

        public DateTime LastDay
        {
            get => _lastDay;
            set => this.RaiseAndSetIfChanged(ref _lastDay, value);
        }
        private DateTime _lastDay;

        public DateTime LastSavedLastDay
        {
            get => _lastSavedLastDay;
            set => this.RaiseAndSetIfChanged(ref _lastSavedLastDay, value);
        }
        private DateTime _lastSavedLastDay;

        public override string UrlPathSegment => "Edit Tournament Info";
    }
}
