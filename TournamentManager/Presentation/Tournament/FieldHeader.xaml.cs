using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for FieldHeader.xaml
    /// </summary>
    public partial class FieldHeader : IViewFor<FieldHeaderVM>
    {
        public FieldHeader()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.FieldName, v => v.FieldName.Text)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.AddGame, v => v.AddGame)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(FieldHeaderVM),
            typeof(FieldHeader),
            new PropertyMetadata(default(FieldHeaderVM)));

        public FieldHeaderVM? ViewModel
        {
            get => (FieldHeaderVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as FieldHeaderVM;
        }
    }
}
