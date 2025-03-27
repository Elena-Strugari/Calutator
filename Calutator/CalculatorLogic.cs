using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Calutator
{
    public class CalculatorLogic
    {
        private string _expression = "";
        private bool _isNewEntry;
        private bool _cascadeMode = false;

        public TextBox Display { get; set; }


        public CalculatorLogic(TextBox display)
        {
            Display = display;
            Display.Text = "0";
        }
        public void ToggleCascadeMode()
        {
            _cascadeMode = !_cascadeMode;
        } 

        public void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            if (_isNewEntry && Display.Text != "0")
            {
                Display.Text += button.Content.ToString();
                _isNewEntry = false;
            }
            else
            {
                if (Display.Text == "0")
                    Display.Text = button.Content.ToString();
                else
                    Display.Text += button.Content.ToString();
            }

            _expression += button.Content.ToString();
            if (_cascadeMode)
            {
                EvaluateAndUpdate();
            }
        }

        public void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string op = button.Content.ToString();

            if (_expression.Length > 0 && "+-*/×÷".Contains(_expression.Last()))
            {
                _expression = _expression.Remove(_expression.Length - 1) + op;
            }
            else
            {
                _expression += " " + op + " ";
            }

            Display.Text = _expression;
            _isNewEntry = true;
            if (_cascadeMode)
            {
                EvaluateAndUpdate();
            }
        }


        public void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double result = EvaluateExpression(_expression);
                string resultString = result.ToString(CultureInfo.InvariantCulture);
                Display.Text = resultString;
                _expression = resultString;
            }
            catch
            {
                Display.Text = "Error";
                _expression = "";
            }
            _isNewEntry = true;
        }

        private void EvaluateAndUpdate()
        {
            try
            {
                double result = EvaluateExpressionCascade(_expression);
                Display.Text = result.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {

                string problematicOperator = FindProblematicOperator(_expression);

                // Afișăm mesajul de eroare cu operatorul identificat
                //MessageBox.Show(problematicOperator);
                Display.Text =problematicOperator;
            }
        }

        private string FindProblematicOperator(string expression)
        {


            foreach (char c in expression)
            {
                if ("+-*/×÷".Contains(c)) 
                {

                    //if (c == '*')
                    //    return "×";
                    //else if (c == '/')
                    //    return "÷";
                    //else if (c == '-')
                    //    return "-";
                    //else if (c == '+')
                    //    return "+";
                    //else
                    return c.ToString(); // Returnează orice alt operator (+ sau -)
                }
            }
            return "?"; // Dacă nu găsim nimic, returnăm un semn de întrebare
        }
        

        private double EvaluateExpressionCascade(string expression)
        {
            expression = expression.Replace("×", "*").Replace("÷", "/"); 

            List<string> tokens = new List<string>(); 
            StringBuilder numberBuilder = new StringBuilder(); 

            foreach (char c in expression)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    numberBuilder.Append(c); 
                }
                else if ("+-*/".Contains(c))
                {
                    if (numberBuilder.Length > 0)
                    {
                        tokens.Add(numberBuilder.ToString()); 
                        numberBuilder.Clear();
                    }
                    tokens.Add(c.ToString()); 
                }
            }

            if (numberBuilder.Length > 0)
                tokens.Add(numberBuilder.ToString()); 

            double result = double.Parse(tokens[0], CultureInfo.InvariantCulture); 

            for (int i = 1; i < tokens.Count; i += 2)
            {
                string op = tokens[i]; // Operatorul curent
                double nextNumber = double.Parse(tokens[i + 1], CultureInfo.InvariantCulture); 

                switch (op)
                {
                    case "+":
                        result += nextNumber;
                        break;
                    case "-":
                        result -= nextNumber;
                        break;
                    case "*":
                        result *= nextNumber;
                        break;
                    case "/":
                        if (nextNumber == 0)
                            return double.NaN;
                        result /= nextNumber;
                        break;
                }
            }

            return result;
        }




        private double EvaluateExpression(string expression)
        {
            expression = expression.Replace("×", "*").Replace("÷", "/"); // Convert symbols

            Stack<double> values = new Stack<double>();
            Stack<char> operators = new Stack<char>();
            int i = 0;
            bool expectNegativeNumber = true;


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
            if (op == '+' || op == '-') return 1;
            if (op == '*' || op == '/') return 2;
            return 0;
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
        //memory

        public void CEButton_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
        }

        public void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            _expression = "";
            _isNewEntry = true;
        }

        public void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (Display.Text.Length > 1)
            {
                Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
                _expression = _expression.Substring(0, _expression.Length - 1);
            }
            else
            {
                Display.Text = "0";
                _expression = "";
            }
        }

        public void ReciprocalButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double number = double.Parse(Display.Text);
                Display.Text = (1 / number).ToString();
                _expression = Display.Text;
            }
            catch
            {
                Display.Text = "Error";
            }
        }

        public void SquareButton_Click(object sender, RoutedEventArgs e)
        {
            double number = double.Parse(Display.Text);
            Display.Text = (number * number).ToString();
            _expression = Display.Text;
        }

        public void SquareRootButton_Click(object sender, RoutedEventArgs e)
        {
            double number = double.Parse(Display.Text);
            if (number >= 0)
                Display.Text = Math.Sqrt(number).ToString();
            else
                Display.Text = "Error";

            _expression = Display.Text;
        }

        public void PercentageButton_Click(object sender, RoutedEventArgs e)
        {
            double number = double.Parse(Display.Text);
            Display.Text = (number / 100).ToString();
            _expression = Display.Text;
        }

        public void NegateButton_Click(object sender, RoutedEventArgs e)
        {
            double number = double.Parse(Display.Text);
            Display.Text = (-number).ToString(CultureInfo.InvariantCulture);
            _expression = Display.Text;
        }

        public void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Display.Text.Contains("."))
            {
                Display.Text += ".";
                _expression += ".";
            }
        }
    }
}



