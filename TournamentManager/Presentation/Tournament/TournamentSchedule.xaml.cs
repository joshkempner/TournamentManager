using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TournamentScheduleVM),
            typeof(TournamentSchedule),
            new PropertyMetadata(default(TournamentScheduleVM)));

        public TournamentScheduleVM ViewModel
        {
            get => (TournamentScheduleVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TournamentScheduleVM)value;
        }
    }
}
