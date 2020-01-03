using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using Splat;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class RefereeCredentialsVM : ReactiveObject, IRoutableViewModel, IDisposable
    {
        private readonly RefereeCredentialsRM _rm;

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public RefereeCredentialsVM(
            Guid refereeId,
            IDispatcher bus,
            IScreen screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _rm = new RefereeCredentialsRM(refereeId);

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
                .ToProperty(this, x => x.Birthdate, out _birthdate);

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

                        if (MaxAgeBracket != LastSavedMaxAgeBracket)
                        {
                            bus.Send(MessageBuilder.New(() => new RefereeMsgs.AddOrUpdateMaxAgeBracket(
                                                                    refereeId,
                                                                    MaxAgeBracket)));
                        }
                    });

            this.WhenAnyObservable(x => x.Save)
                .InvokeCommand(HostScreen.Router.NavigateBack);

            Cancel = HostScreen.Router.NavigateBack;
        }

        public void Dispose()
        {
            _rm.Dispose();
        }

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

        public DateTime Birthdate => _birthdate.Value;
        private readonly ObservableAsPropertyHelper<DateTime> _birthdate;

        private TeamMsgs.AgeBracket LastSavedMaxAgeBracket { get; set; }

        public TeamMsgs.AgeBracket MaxAgeBracket
        {
            get => _maxAgeBracket;
            set => this.RaiseAndSetIfChanged(ref _maxAgeBracket, value);
        }
        private TeamMsgs.AgeBracket _maxAgeBracket;

        public string UrlPathSegment => "Referee Credentials";
        public IScreen HostScreen { get; }
    }
}
