using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for NewReferee.xaml
    /// </summary>
    public partial class NewReferee : IViewFor<NewRefereeVM>
    {
        public NewReferee()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.GivenName, v => v.GivenName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Surname, v => v.Surname.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.EmailAddress, v => v.EmailAddress.Text)
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
                this.Bind(ViewModel, vm => vm.Age, v => v.Age.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Birthdate, v => v.Birthdate.SelectedDate)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.MaxAgeBracket, v => v.MaxAgeBracket.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddReferee, v => v.Add)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(NewRefereeVM),
            typeof(NewReferee),
            new PropertyMetadata(default(NewRefereeVM)));

        public NewRefereeVM ViewModel
        {
            get => (NewRefereeVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (NewRefereeVM)value;
        }
    }
}
