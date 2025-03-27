using System;
using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace Calutator
{
    public class Programator
    {
        private TextBox _display;
        private TextBlock _hexValue;
        private TextBlock _decValue;
        private TextBlock _octValue;
        private TextBlock _binValue;
        private bool _isNewEntry = false;
        private string _expression = "";

        private int _currentBase = 10;


        public Programator(TextBox display, TextBlock hex, TextBlock dec, TextBlock oct, TextBlock bin)
        {
            _display = display;
            _hexValue = hex;
            _decValue = dec;
            _octValue = oct;
            _binValue = bin;
        }

        public void UpdateNumberBases()
        {
            if (int.TryParse(_display.Text, out int number))
            {
                _hexValue.Text = number.ToString("X");  // HEX
                _decValue.Text = number.ToString();     // DEC
                _octValue.Text = Convert.ToString(number, 8);  // OCT
                _binValue.Text = Convert.ToString(number, 2);  // BIN
            }
        }

        public void SetBase(int numberBase)
        {
            _currentBase = numberBase; // Set the selected base
                                       // UpdateNumberBases(); // Update number representation

            if (int.TryParse(_display.Text, out int number))
            {
                switch (_currentBase)
                {
                    case 2:  // Binary
                        _display.Text = Convert.ToString(number, 2);
                        break;
                    case 8:  // Octal
                        _display.Text = Convert.ToString(number, 8);
                        break;
                    case 10: // Decimal
                        _display.Text = number.ToString();
                        break;
                    case 16: // Hexadecimal
                        _display.Text = number.ToString("X");
                        break;
                }
            }
        }


        public void NumberButton_ClickP(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string input = button.Content.ToString().ToUpper(); // Convert to uppercase for HEX

            if (IsValidCharacter(input)) // Only add if valid
            {
                if (_isNewEntry && _display.Text != "0")
                {
                    _display.Text += input;
                    _isNewEntry = false;
                }
                else
                {
                    _display.Text = (_display.Text == "0") ? input : _display.Text + input;
                }

                _expression += input;
                UpdateNumberBases();
            }
        }
        private bool IsValidCharacter(string input)
        {
            switch (_currentBase)
            {
                case 2:  // Binary (0-1)
                    return input == "0" || input == "1";

                case 8:  // Octal (0-7)
                    return "01234567".Contains(input);

                case 10: // Decimal (0-9)
                    return "0123456789".Contains(input);

                case 16: // Hexadecimal (0-9, A-F)
                    return "0123456789ABCDEF".Contains(input);

                default:
                    return false;
            }
        }


        public void OperatorButton_ClickP(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string op = button.Content.ToString();

            if (_expression.Length > 0 && "+-*/×÷".Contains(_expression[^1]))
            {
                _expression = _expression.Remove(_expression.Length - 1) + op;
            }
            else
            {
                _expression += " " + op + " ";
            }

            _display.Text = _expression;
            _isNewEntry = true;
        }

        public void EqualsButton_ClickP(object sender, RoutedEventArgs e)
        {
            try
            {
                double result = EvaluateExpressionP(_expression); // Use Correct Function
                string resultString = result.ToString(CultureInfo.InvariantCulture);
                _display.Text = resultString;
                _expression = resultString;
            }
            catch
            {
                _display.Text = "Error";
                _expression = "";
            }
            _isNewEntry = true;
            UpdateNumberBases(); // Update values after calculation
        }


        private double EvaluateExpressionP(string expression)
        {
            expression = expression.Replace("×", "*").Replace("÷", "/"); // Convert symbols

            Stack<double> values = new Stack<double>();
            Stack<char> operators = new Stack<char>();
            int i = 0;

            while (i < expression.Length)
            {
                if (char.IsDigit(expression[i]) || expression[i] == '.')
                {
                    string number = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        number += expression[i];
                        i++;
                    }
                    values.Push(double.Parse(number, CultureInfo.InvariantCulture));
                    continue;
                }
                else if ("+-*/".Contains(expression[i]))
                {
                    while (operators.Count > 0 && GetPrecedence(operators.Peek()) >= GetPrecedence(expression[i]))
                        ApplyOperator(values, operators.Pop());

                    operators.Push(expression[i]);
                }
                i++;
            }

            while (operators.Count > 0)
                ApplyOperator(values, operators.Pop());

            return values.Pop();
        }

        private int GetPrecedence(char op)
        {
            return op switch
            {
                '+' or '-' => 1,
                '*' or '/' => 2,
                _ => 0,
            };
        }

        private void ApplyOperator(Stack<double> values, char op)
        {
            if (values.Count < 2) return;
            double b = values.Pop();
            double a = values.Pop();

            switch (op)
            {
                case '+': values.Push(a + b); break;
                case '-': values.Push(a - b); break;
                case '*': values.Push(a * b); break;
                case '/': values.Push(b != 0 ? a / b : double.NaN); break;
            }
        }

        public void ClearButton_ClickP(object sender, RoutedEventArgs e)
        {
            _display.Text = "0";
            _expression = "";
            _isNewEntry = true;
            UpdateNumberBases();
        }

        public void BackspaceButton_ClickP(object sender, RoutedEventArgs e)
        {
            if (_display.Text.Length > 1)
            {
                _display.Text = _display.Text.Substring(0, _display.Text.Length - 1);
                _expression = _expression.Substring(0, _expression.Length - 1);
            }
            else
            {
                _display.Text = "0";
                _expression = "";
            }
            UpdateNumberBases();
        }

        public void NegateButton_ClickP(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(_display.Text, out int number))
            {
                number = -number; // Negate the value
                _display.Text = number.ToString(); // Update display
                _expression = _display.Text; // Update expression string
                UpdateNumberBases(); // Update HEX, DEC, OCT, BIN values
            }
        }


    }
}

