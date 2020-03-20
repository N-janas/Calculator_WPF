using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double input = 0;
        private double output = 0;
        private string operation = "";
        private string inputCap = "";
        private bool previewOpened = false;
        private bool statePrep = true;
        private bool stateOperate = false;
        public bool PreviewOpened
        {
            get { return previewOpened; }
            set { previewOpened = value; }
        }
        public bool StateOperate
        {
            get { return stateOperate; }
            set { stateOperate = value; }
        }
        public bool StatePrep
        {
            get { return statePrep; }
            set { statePrep = value; }
        }
        public string InputCap
        {
            get { return inputCap; }
            set { inputCap = value; }
        }
        public double Input
        {
            get { return input; }
            set { input = value; }
        }
        public double Output
        {
            get { return output; }
            set
            {
                output = value;

                if (output > double.MaxValue || output < double.MinValue) // Przypisuje 0 po wypisaniu textu więc operacja po tekście to np: 0*... = 0
                {
                    txtboxDisplay.Text = "Przepełniono";
                    output = 0;
                    StatePrep = true;
                    StateOperate = false;
                    PreviewOpened = true;
                }
                else
                {
                    txtboxDisplay.Text = output.ToString();
                }
            }
        }
        public string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        public MainWindow()
        {
            CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            InitializeComponent();
        }

        // Place/remove digits
        private void btn_PlaceDigit(object sender, RoutedEventArgs e)
        {
            var buttonNumber = sender as Button;
            if (PreviewOpened == true)
            {
                txtboxDisplay.Text = buttonNumber.Content.ToString();
                double.TryParse(txtboxDisplay.Text, out double tmp);
                Input = tmp;

                PreviewOpened = false;
                if (StatePrep == false)
                {
                    StateOperate = true;
                }
                StatePrep = true;
            }
            else
            {
                InputCap = txtboxDisplay.Text.Replace(".", "");
                InputCap = InputCap.Replace("-", "");
                if (!(InputCap.Length > 14))
                {
                    if (txtboxDisplay.Text.Equals("0"))
                    {
                        txtboxDisplay.Text = buttonNumber.Content.ToString();
                    }
                    else
                    {
                        txtboxDisplay.Text += buttonNumber.Content.ToString();
                    }

                    double.TryParse(txtboxDisplay.Text, out double tmp);
                    Input = tmp;
                }
            }

        }
        private void btn_Backspace(object sender, RoutedEventArgs e)
        {
            if (StatePrep == true && PreviewOpened == false)
            {
                if (txtboxDisplay.Text.Length == 1 || ( txtboxDisplay.Text.Length == 2 && txtboxDisplay.Text[0] == '-'))
                {
                    txtboxDisplay.Text = "0";
                }
                else
                {
                    txtboxDisplay.Text = txtboxDisplay.Text.Remove(txtboxDisplay.Text.Length - 1);
                }

                double.TryParse(txtboxDisplay.Text, out double tmp);
                Input = tmp;
            }
        }

        // Operations
        private void btn_Operate(object sender, RoutedEventArgs e)
        {
            var operation = sender as Button;

            if (StateOperate == true)
            {
                if (Operation == "+")
                {
                    Output += Input;
                    Input = 0;
                }
                else if (Operation == "-")
                {
                    Output -= Input;
                    Input = 0;
                }
                else if (Operation == "/")
                {
                    if (Input == 0 && Output == 0)
                    {
                        txtboxDisplay.Text = "Symbol nieokreślony";
                        output = 0;
                        StatePrep = true;
                        StateOperate = false;
                        PreviewOpened = true;
                    }
                    else if (Input == 0)
                    {
                        txtboxDisplay.Text = "Nie dziel przez 0";
                        output = 0;
                        StatePrep = true;
                        StateOperate = false;
                        PreviewOpened = true;
                    }
                    else
                    {
                        Output /= Input;
                        Input = 0;
                    }

                }
                else if (Operation == "*")
                {
                    Output *= Input;
                    Input = 0;
                }
                else
                {
                    Output = Input;
                }
            }
            else if (StatePrep == true)
            {
                Output = Input;
                Input = 0;
                StatePrep = false;
                PreviewOpened = true; 
            }

            if (operation.Content.ToString() == "=" && StateOperate == true) // = nie ma efektu kiedy kliknięty po innym znaku operacji
            {
                Input = Output;
                StatePrep = true;
                PreviewOpened = true; 
                Operation = "";
            }
            else if (!(operation.Content.ToString() == "="))
            {
                Operation = operation.Content.ToString();
                PreviewOpened = true; 
                StatePrep = false;
            }

            StateOperate = false;

        }

        // Specials
        private void btn_Clear(object sender, RoutedEventArgs e)
        {
            Input = 0;
            Output = 0;
            PreviewOpened = false;
            StatePrep = true;
            StateOperate = false;
            Operation = "";
            txtboxDisplay.Text = Input.ToString();
        }

        private void btn_ClearEntry(object sender, RoutedEventArgs e)
        {
            Input = 0;
            StateOperate = true;
            txtboxDisplay.Text = Input.ToString();
        }

        private void btn_Dot(object sender, RoutedEventArgs e)
        {
            if (!txtboxDisplay.Text.Contains(".") && StatePrep == true && PreviewOpened == false)
            {
                txtboxDisplay.Text += ".";
            }
            else if (PreviewOpened == true)
            {
                txtboxDisplay.Text = "0.";
                Input = 0;
                PreviewOpened = false;
                if (StatePrep == false)
                {
                    StateOperate = true;
                }
                StatePrep = true;
            }
        }

        private void btn_Negate(object sender, RoutedEventArgs e)
        {
            if (txtboxDisplay.Text[0] == '-' && PreviewOpened == false && txtboxDisplay.Text != "0" && txtboxDisplay.Text != "0.") // zmien na +
            {
                txtboxDisplay.Text = txtboxDisplay.Text.Remove(0, 1);
                Input = -Input;
            }
            else if (!(txtboxDisplay.Text[0] == '-') && PreviewOpened == false && txtboxDisplay.Text != "0" && txtboxDisplay.Text != "0.") // zmien na -
            {
                txtboxDisplay.Text = txtboxDisplay.Text.Insert(0, "-");
                Input = -Input;
            }
        }

    }
}
