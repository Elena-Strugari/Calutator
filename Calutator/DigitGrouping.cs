using System;
using System.Globalization;
using System.Windows.Controls;

namespace Calutator
{
    public class DigitGrouping
    {
        private TextBox _display;
        private CultureInfo _currentCulture;
        private bool _isEnabled;

        public DigitGrouping(TextBox display)
        {
            _display = display;
            _currentCulture = new CultureInfo("en-GB"); // Setăm UK ca implicit
            _isEnabled = false; // Default: Disabled
        }

        public void ToggleDigitGrouping(bool isEnabled)
        {
            _isEnabled = isEnabled;
            FormatDisplay();
        }

        public void FormatDisplay()
        {
            if (_isEnabled && double.TryParse(_display.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double number))
            {
                 _display.Text = number.ToString("N0", _currentCulture);
                _display.CaretIndex = _display.Text.Length; // Muta cursorul la final
            }
            else if (!_isEnabled)
            {
                // Eliminăm gruparea dacă este dezactivată
                _display.Text = _display.Text.Replace(_currentCulture.NumberFormat.NumberGroupSeparator, "");
            }
        }

        public void InsertDecimalSeparator()
        {
            string separator = "."; // Forțăm punctul ca separator decimal
            if (!_display.Text.Contains(separator))
                _display.Text += separator;
        }

    }
}
