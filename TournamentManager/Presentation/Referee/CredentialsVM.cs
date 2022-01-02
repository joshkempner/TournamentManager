using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public sealed class CredentialsVM : TransientViewModel
    {
        private readonly CredentialsRM _rm;

        public ReactiveCommand<Unit, Unit> Save { get; }

        public CredentialsVM(
            Guid refereeId,
            string fullName,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            RefereeId = refereeId;
            FullName = fullName;
            _rm = new CredentialsRM(refereeId);

            _rm.RefereeGrade
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    if (x == null) return;
                    LastSavedGrade = x.Value;
                    RefereeGrade = x.Value;
                });

            _rm.Birthdate
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedBirthdate = x;
                    Birthdate = x;
                });

            _rm.CurrentAge
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedAge = x;
                    CurrentAge = x;
                });

            _rm.MaxAgeBracket
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedMaxAgeBracket = x;
                    MaxAgeBracket = x;
                });

            this.WhenAnyValue(x => x.Birthdate)
                .Subscribe(x =>
                {
                    if (x == default || RefereeGrade == RefereeMsgs.Grade.Intramural) return;
                    CurrentAge = (ushort)x.YearsAgo();
                });

            Save = CommandBuilder.FromAction(
                    () =>
                    {
                        if (RefereeGrade != LastSavedGrade)
                        {
                            bus.Send(MessageBuilder.New(() => new RefereeMsgs.UpdateGrade(
                                                                    refereeId,
                                                                    RefereeGrade)));
                        }

                        if (RefereeGrade == RefereeMsgs.Grade.Intramural && CurrentAge != LastSavedAge)
                        {
                            bus.Send(MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateAge(
                                                                    refereeId,
                                                                    CurrentAge)));
                        }

                        if (RefereeGrade != RefereeMsgs.Grade.Intramural && LastSavedBirthdate != Birthdate)
                        {
                            bus.Send(MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateBirthdate(
                                                                    refereeId,
                                                                    Birthdate)));
                        }
                        
                        if (MaxAgeBracket != LastSavedMaxAgeBracket)
                        {
                            bus.Send(MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMaxAgeBracket(
                                                                    refereeId,
                                                                    MaxAgeBracket)));
                        }
                    });

            this.WhenAnyObservable(x => x.Save)
                .InvokeCommand(Complete);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _rm.Dispose();
            base.Dispose(disposing);
        }

        public Guid RefereeId { get; }

        public string FullName { get; }

        private RefereeMsgs.Grade LastSavedGrade { get; set; }

        public RefereeMsgs.Grade RefereeGrade
        {
            get => _refereeGrade;
            set => this.RaiseAndSetIfChanged(ref _refereeGrade, value);
        }
        private RefereeMsgs.Grade _refereeGrade;

        private ushort LastSavedAge { get; set; }

        public ushort CurrentAge
        {
            get => _currentAge;
            set => this.RaiseAndSetIfChanged(ref _currentAge, value);
        }
        private ushort _currentAge;

        private DateTime LastSavedBirthdate { get; set; }

        public DateTime Birthdate
        {
            get => _birthdate;
            set => this.RaiseAndSetIfChanged(ref _birthdate, value);
        }
        private DateTime _birthdate;

        private TournamentMsgs.AgeBracket LastSavedMaxAgeBracket { get; set; }

        public TournamentMsgs.AgeBracket MaxAgeBracket
        {
            get => _maxAgeBracket;
            set => this.RaiseAndSetIfChanged(ref _maxAgeBracket, value);
        }
        private TournamentMsgs.AgeBracket _maxAgeBracket;

        public override string UrlPathSegment => "Referee Credentials";
    }
}
