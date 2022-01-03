using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for NewField.xaml
    /// </summary>
    public partial class NewField : IViewFor<NewFieldVM>
    {
        public NewField()
        {
            InitializeComponent();


            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.FieldName, v => v.FieldName.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Save, v => v.Add)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(NewFieldVM),
            typeof(NewField),
            new PropertyMetadata(default(NewFieldVM)));

        public NewFieldVM? ViewModel
        {
            get => (NewFieldVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as NewFieldVM;
        }
    }
}
