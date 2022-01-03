using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class TournamentScheduleVM : TransientViewModel
    {
        private readonly TournamentScheduleRM _rm;

        public ReactiveCommand<Unit, IRoutableViewModel> AddField { get; }

        public TournamentScheduleVM(
            Guid tournamentId,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            _rm = new TournamentScheduleRM(tournamentId);

            AddField = ReactiveCommand.CreateFromObservable(
                        () => HostScreen.Router.Navigate.Execute(new NewFieldVM(
                                                                        tournamentId,
                                                                        bus,
                                                                        HostScreen)));

            this.WhenActivated(disposables =>
            {
                _rm.FirstDay
                    .CombineLatest(_rm.LastDay, (f, l) => (f, l))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        x =>
                        {
                            var (first, last) = x;
                            using var d = TournamentDates.SuspendNotifications();
                            TournamentDates.Clear();
                            for (var day = first; day <= last; day += TimeSpan.FromDays(1))
                            {
                                TournamentDates.Add(day.ToShortDateString());
                            }
                        })
                    .DisposeWith(disposables);
            });

            var fieldsConnection = _rm.Fields.Connect();

            fieldsConnection
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new FieldHeaderVM(
                                        tournamentId,
                                        x.FieldId,
                                        x.FieldName,
                                        this.WhenAnyValue(t => t.SelectedTournamentDateIndex),
                                        bus))
                .Bind(FieldHeaders)
                .DisposeMany()
                .Subscribe();
        }

        public IObservableCollection<string> TournamentDates { get; } = new ObservableCollectionExtended<string>();

        public IObservableCollection<FieldHeaderVM> FieldHeaders { get; } = new ObservableCollectionExtended<FieldHeaderVM>();

        public int SelectedTournamentDateIndex
        {
            get => _selectedTournamentDateIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedTournamentDateIndex, value);
        }
        private int _selectedTournamentDateIndex;

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _rm.Dispose();
            _disposed = true;
            base.Dispose(disposing);
        }

        public override string UrlPathSegment => "TournamentSchedule";
    }
}
