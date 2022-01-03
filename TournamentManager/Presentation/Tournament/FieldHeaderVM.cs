using System;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class FieldHeaderVM : ReactiveObject, IDisposable, IActivatableViewModel
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

            AddGame = CommandBuilder.FromAction(
                        this.WhenAnyValue(
                            x => x.NewGameViewModel,
                            x => x.GameDay,
                            (vm, day) => vm is null && day >= 0),
                        () => NewGameViewModel = new NewGameVM(
                                                      tournamentId,
                                                      fieldId,
                                                      (uint)GameDay,
                                                      bus,
                                                      vm =>
                                                      {
                                                          vm.Dispose();
                                                          NewGameViewModel = null;
                                                      }));

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

        public NewGameVM? NewGameViewModel { get; set; }

        public void Dispose()
        {
            NewGameViewModel?.Dispose();
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
