﻿using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for TournamentSchedule.xaml
    /// </summary>
    public partial class TournamentSchedule : IViewFor<TournamentScheduleVM>
    {
        public TournamentSchedule()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.TournamentDates, v => v.GameDay.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedTournamentDateIndex, v => v.GameDay.SelectedIndex)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.FieldHeaders, v => v.FieldHeaders.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddField, v => v.AddField)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Done)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TournamentScheduleVM),
            typeof(TournamentSchedule),
            new PropertyMetadata(default(TournamentScheduleVM)));

        public TournamentScheduleVM? ViewModel
        {
            get => (TournamentScheduleVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TournamentScheduleVM;
        }
    }
}
