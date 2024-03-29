﻿using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for TournamentsHost.xaml
    /// </summary>
    public partial class TournamentsHost : IViewFor<TournamentsHostVM>
    {
        public TournamentsHost()
        {
            InitializeComponent();


            this.WhenActivated(disposables =>
            {
                // Bind the view model router to the RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                    .DisposeWith(disposables);
            });

            Loaded += (sender, args) => ViewModel?.NavigateToInitialView();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TournamentsHostVM),
            typeof(TournamentsHost),
            new PropertyMetadata(default(TournamentsHostVM)));

        public TournamentsHostVM? ViewModel
        {
            get => (TournamentsHostVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TournamentsHostVM;
        }
    }
}
