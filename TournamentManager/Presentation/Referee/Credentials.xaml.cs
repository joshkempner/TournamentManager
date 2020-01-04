using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for Credentials.xaml
    /// </summary>
    public partial class Credentials : IViewFor<CredentialsVM>
    {
        public Credentials()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.FullName, v => v.FullName.Text, s => $"Credentials for {s}")
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.RefereeGrade, v => v.RefereeGrade.SelectedItem)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.RefereeGrade, v => v.Age.Visibility,
                        g => g == RefereeMsgs.Grade.Intramural ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.RefereeGrade, v => v.MaxAgeBracket.IsEnabled,
                        g => g != RefereeMsgs.Grade.Intramural)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.RefereeGrade, v => v.Birthdate.Visibility,
                        g => g != RefereeMsgs.Grade.Intramural ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.RefereeGrade, v => v.CalculatedAge.Visibility,
                        g => g != RefereeMsgs.Grade.Intramural ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CurrentAge, v => v.Age.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Birthdate, v => v.Birthdate.SelectedDate)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.CurrentAge, v => v.CalculatedAge.Text, s => $"Current age is {s}")
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.MaxAgeBracket, v => v.MaxAgeBracket.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Save, v => v.Save)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(CredentialsVM),
            typeof(Credentials),
            new PropertyMetadata(default(CredentialsVM)));

        public CredentialsVM ViewModel
        {
            get => (CredentialsVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CredentialsVM)value;
        }
    }
}
