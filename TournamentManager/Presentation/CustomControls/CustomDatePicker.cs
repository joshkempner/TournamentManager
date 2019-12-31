using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TournamentManager.Presentation
{
    public class CustomDatePicker : DatePicker
    {
        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(CustomDatePicker), new PropertyMetadata("Datum wählen..."));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var box = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            if (box == null) return;
            box.ApplyTemplate();

            var watermark = box.Template.FindName("PART_Watermark", box) as ContentControl;
            if (watermark == null) return;
            watermark.Content = WatermarkText;
        }
    }
}
