using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for ContactInfo.xaml
    /// </summary>
    public partial class ContactInfo : IViewFor<ContactInfoVM>
    {
        public ContactInfo()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.FullName, v => v.FullName.Text, s => $"Contact info for {s}")
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.EmailAddress, v => v.EmailAddress.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.StreetAddress1, v => v.StreetAddress1.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.StreetAddress2, v => v.StreetAddress2.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.City, v => v.City.Text)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.StateNames, v => v.State.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedStateName, v => v.State.SelectedItem)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ZipCode, v => v.ZipCode.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Save, v => v.Save)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(ContactInfoVM),
            typeof(ContactInfo),
            new PropertyMetadata(default(ContactInfoVM)));

        public ContactInfoVM? ViewModel
        {
            get => (ContactInfoVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as ContactInfoVM;
        }
    }
}
