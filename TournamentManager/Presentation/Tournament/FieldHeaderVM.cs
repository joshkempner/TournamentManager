using System;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class FieldHeaderVM : ReactiveObject, IActivatableViewModel
    {
        public ReactiveCommand<Unit, Unit> AddGame { get; }

        public FieldHeaderVM(
            Guid tournamentId,
            Guid fieldId,
            string fieldName,
            IObservable<int> gameDay,
            IDispatcher bus)
        {
            FieldName = fieldName;

            AddGame = bus.BuildSendCommand(
                        this.WhenAnyValue(
                            x => x.GameDay,
                            day => day >= 0),
                        () => MessageBuilder.New(
                                () => new HostViewMsgs.DisplayOverlay(
                                            () => new NewGameVM(
                                                          tournamentId,
                                                          fieldId,
                                                          (uint)GameDay,
                                                          bus))));

            this.WhenActivated(disposables =>
            {
                gameDay
                    .ToProperty(this, x => x.GameDay, out _gameDay)
                    .DisposeWith(disposables);
            });
        }

        public string FieldName { get; }

        public int GameDay => _gameDay.Value;
        private ObservableAsPropertyHelper<int> _gameDay = ObservableAsPropertyHelper<int>.Default();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
