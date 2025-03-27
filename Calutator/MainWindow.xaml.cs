using System;
using System.Globalization;
using System.Linq.Expressions;
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

namespace Calutator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private CalculatorLogic _calculator;
        private Programator _programator;
        private Memory _memory;
        private string _clipboardText = "";
        private DigitGrouping _digitGrouping;
        private bool _digitGroupingEnabled = false;
        private AppSettings _userSettings;


        public MainWindow()
        {
            InitializeComponent();
            _calculator = new CalculatorLogic(StandardDisplay);
            _programator= new Programator(ProgrammerDisplay, HexValue, DecValue, OctValue, BinValue);
            _memory = new Memory();
            _digitGrouping = new DigitGrouping(StandardDisplay);
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            StandardDisplay.TextChanged += StandardDisplay_TextChanged;


            _userSettings = AppSettings.LoadSettings(); 

            // Aplica settings 

            _digitGroupingEnabled = _userSettings.DigitGroupingEnabled;
            if (_digitGroupingEnabled)
            {
                _digitGrouping.ToggleDigitGrouping(true);
            }

            if (_userSettings.LastUsedMode == "Programmer")
                MainTabControl.SelectedIndex = 1;
            else
                MainTabControl.SelectedIndex = 0;

            this.Closing += MainWindow_Closing;

        }
        // save settings 

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _userSettings.DigitGroupingEnabled = _digitGroupingEnabled;
            _userSettings.LastUsedMode = MainTabControl.SelectedIndex == 1 ? "Programmer" : "Standard";

            _userSettings.SaveSettings();
        }
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_userSettings == null) return; 

            _userSettings.LastUsedMode = MainTabControl.SelectedIndex == 1 ? "Programmer" : "Standard";
            _userSettings.SaveSettings();
        }



        //Digit grouping 
        private void StandardDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            _digitGrouping.FormatDisplay();
        }
       
        private void ToggleDigitGrouping_Click(object sender, RoutedEventArgs e)
        {
            _digitGroupingEnabled = !_digitGroupingEnabled;

            // Aplică digit grouping doar pentru UK (fără RO)
            _digitGrouping.ToggleDigitGrouping(_digitGroupingEnabled);

            _userSettings.DigitGroupingEnabled = _digitGroupingEnabled;
            _userSettings.SaveSettings();
        }


        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            _digitGrouping.InsertDecimalSeparator();
        }



        // keys
        private void StandardDisplay_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Prevent user input (except handled by buttons)
            if (!(e.Key == Key.C && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) &&
                !(e.Key == Key.V && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) &&
                !(e.Key == Key.X && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)))
            {
                e.Handled = true; // Stop input
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
                switch (e.Key)
                {
                    case >= Key.D0 and <= Key.D9 when !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift):
                        _calculator.NumberButton_Click(new Button { Content = ((int)(e.Key - Key.D0)).ToString() }, null);
                        break;

                    case >= Key.NumPad0 and <= Key.NumPad9:
                        _calculator.NumberButton_Click(new Button { Content = ((int)(e.Key - Key.NumPad0)).ToString() }, null);
                        break;

                    case Key.Add or Key.OemPlus:
                        _calculator.OperatorButton_Click(new Button { Content = "+" }, null);
                        break;

                    case Key.Subtract or Key.OemMinus:
                        _calculator.OperatorButton_Click(new Button { Content = "-" }, null);
                        break;

                    case Key.Multiply or Key.D8 when Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift):
                        _calculator.OperatorButton_Click(new Button { Content = "*" }, null);
                        break;

                    case Key.Divide or Key.OemQuestion:
                        _calculator.OperatorButton_Click(new Button { Content = "/" }, null);
                        break;

                    case Key.Enter or Key.Return:
                        _calculator.EqualsButton_Click(null, null);
                        break;

                    case Key.Back:
                        _calculator.BackspaceButton_Click(null, null);
                        break;

                    case Key.Delete:
                        _calculator.ClearButton_Click(null, null);
                        break;

                    case Key.Decimal or Key.OemPeriod:
                        _calculator.DecimalButton_Click(null, null);
                        break;
                    case Key.Escape:
                        _calculator.ClearButton_Click(null, null);
                        break;

                    case Key.C when Keyboard.Modifiers.HasFlag(ModifierKeys.Control):
                        Clipboard.SetText(StandardDisplay.Text);
                        break;

                    case Key.V when Keyboard.Modifiers.HasFlag(ModifierKeys.Control):
                        if (Clipboard.ContainsText())
                        {
                            foreach (char c in Clipboard.GetText())
                            {
                                _calculator.NumberButton_Click(new Button { Content = c.ToString() }, null);
                            }
                        }
                        break;

                    case Key.X when Keyboard.Modifiers.HasFlag(ModifierKeys.Control):
                        Clipboard.SetText(StandardDisplay.Text);
                        StandardDisplay.Text = "";
                        break;
                 }
        }

        private void ToggleCascadeMode_Click(object sender, RoutedEventArgs e)
        {
            _calculator.ToggleCascadeMode();
        }



        //buttons

        private void NumberButton_Click(object sender, RoutedEventArgs e) => _calculator.NumberButton_Click(sender, e);
        private void OperatorButton_Click(object sender, RoutedEventArgs e) => _calculator.OperatorButton_Click(sender, e);
        private void EqualsButton_Click(object sender, RoutedEventArgs e) => _calculator.EqualsButton_Click(sender, e);
        private void ClearButton_Click(object sender, RoutedEventArgs e) => _calculator.ClearButton_Click(sender, e);
        private void CEButton_Click(object sender, RoutedEventArgs e) => _calculator.CEButton_Click(sender, e);

        private void BackspaceButton_Click(object sender, RoutedEventArgs e) => _calculator.BackspaceButton_Click(sender, e);
        private void ReciprocalButton_Click(object sender, RoutedEventArgs e) => _calculator.ReciprocalButton_Click(sender, e);
        private void SquareButton_Click(object sender, RoutedEventArgs e) => _calculator.SquareButton_Click(sender, e);
        private void SquareRootButton_Click(object sender, RoutedEventArgs e) => _calculator.SquareRootButton_Click(sender, e);

        private void PercentageButton_Click(object sender, RoutedEventArgs e) => _calculator.PercentageButton_Click(sender, e);
        private void NegateButton_Click(object sender, RoutedEventArgs e) => _calculator.NegateButton_Click(sender, e);

        //Memory 

        private void MemoryStore_Click(object sender, RoutedEventArgs e)
        {
            _memory.Store(double.Parse(StandardDisplay.Text));
        }


        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            double recalledValue = _memory.Recall(); 
            string recalledText = recalledValue.ToString(CultureInfo.InvariantCulture);

            foreach (char c in recalledText)
            {
                _calculator.NumberButton_Click(new Button { Content = c.ToString() }, null);
            }
        }

        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            _memory.Clear();
        }

        private void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            _memory.Add(double.Parse(StandardDisplay.Text));
        }

        private void MemorySubtract_Click(object sender, RoutedEventArgs e)
        {
            _memory.Subtract(double.Parse(StandardDisplay.Text));
        }

        private void MemoryShowStack_Click(object sender, RoutedEventArgs e)
        {
            MemoryList.Items.Clear();
            foreach (var value in _memory.GetMemoryValues())
            {
                MemoryList.Items.Add(value.ToString());
            }
            MemoryList.Visibility = MemoryList.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        // button for copy, paste, cut 

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(StandardDisplay.Text);
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                foreach (char c in Clipboard.GetText())
                {
                    _calculator.NumberButton_Click(new Button { Content = c.ToString() }, null);
                }
            }
        }
        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(StandardDisplay.Text);
            StandardDisplay.Text = "";
        }

        // programator 
        //button
        private void NumberButton_ClickP(object sender, RoutedEventArgs e) => _programator.NumberButton_ClickP(sender, e);
        private void OperatorButton_ClickP(object sender, RoutedEventArgs e) => _programator.OperatorButton_ClickP(sender, e);
        private void EqualsButton_ClickP(object sender, RoutedEventArgs e) => _programator.EqualsButton_ClickP(sender, e);
        private void ClearButton_ClickP(object sender, RoutedEventArgs e) => _programator.ClearButton_ClickP(sender, e);
        private void BackspaceButton_ClickP(object sender, RoutedEventArgs e) => _programator.BackspaceButton_ClickP(sender, e);
        private void NegateButton_ClickP(object sender, RoutedEventArgs e) => _programator.NegateButton_ClickP(sender, e);



        private void SetBaseToBinary(object sender, RoutedEventArgs e) => _programator.SetBase(2);
        private void SetBaseToOctal(object sender, RoutedEventArgs e) => _programator.SetBase(8);
        private void SetBaseToDecimal(object sender, RoutedEventArgs e) => _programator.SetBase(10);
        private void SetBaseToHex(object sender, RoutedEventArgs e) => _programator.SetBase(16);
    }

}
