using System.Windows;
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
                this.OneWayBind(ViewModel, vm => vm.FullName, v => v.FullName.Text);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(RefereeItemVM),
            typeof(RefereeItem),
            new PropertyMetadata(default(RefereeItemVM)));

        public RefereeItemVM ViewModel
        {
            get => (RefereeItemVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (RefereeItemVM)value;
        }
    }
}
