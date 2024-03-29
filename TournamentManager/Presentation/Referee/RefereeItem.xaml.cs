﻿using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Navigation;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for RefereeItem.xaml
    /// </summary>
    public partial class RefereeItem : IViewFor<RefereeItemVM>
    {
        public RefereeItem()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.FullName, v => v.FullName.Text)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.FullEmail, v => v.EmailAddress.Text)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.FullEmail, v => v.MailReferee.NavigateUri,
                        m => new Uri($"mailto:{m.Address}"))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.AgeRange, v => v.AgeRange.Text)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.RefereeGrade, v => v.RefereeGrade.Text)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.MaxAgeBracket, v => v.MaxAgeBracket.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.EditContactInfo, v => v.EditContactInfo)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.EditCredentials, v => v.EditCredentials)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(RefereeItemVM),
            typeof(RefereeItem),
            new PropertyMetadata(default(RefereeItemVM)));

        public RefereeItemVM? ViewModel
        {
            get => (RefereeItemVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as RefereeItemVM;
        }

        private void OpenUri(object sender, RequestNavigateEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName =  e.Uri.AbsoluteUri,
                UseShellExecute = true
            };
            Process.Start(psi);
            e.Handled = true;
        }
    }
}
