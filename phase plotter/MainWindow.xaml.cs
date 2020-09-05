using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhasePlotter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }


        private double CoefFromTextBox(TextBox textbox)
        {
            double result;
            var str = textbox.Text;
            if (!double.TryParse(str.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
            {
                result = 0;
                if (!textbox.IsFocused)
                {
                    textbox.Text = "0";
                }
            }
            return Math.Round(result, 5);
        }

        private double CoefFromTextBox(string textBoxName)
        {
            var textbox = ChildFinder.FindChild<TextBox>(this, textBoxName);
            return CoefFromTextBox(textbox);
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

    }
}