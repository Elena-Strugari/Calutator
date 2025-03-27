using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calutator
{
    public class Memory
    {
        private List<double> _memoryValues;

        public Memory()
        {
            _memoryValues = new List<double>();
        }

        public void Store(double value)
        {
            _memoryValues.Add(value);
        }

        public double Recall()
        {
            return _memoryValues.Count > 0 ? _memoryValues.Last() : 0;
        }

        public void Clear()
        {
            _memoryValues.Clear();
        }

        public void Add(double value)
        {
            if (_memoryValues.Count > 0)
                _memoryValues[_memoryValues.Count - 1] += value;
            else
                _memoryValues.Add(value);
        }

        public void Subtract(double value)
        {
            if (_memoryValues.Count > 0)
                _memoryValues[_memoryValues.Count - 1] -= value;
            else
                _memoryValues.Add(-value);
        }

        public List<double> GetMemoryValues()
        {
            return new List<double>(_memoryValues);
        }
    }
}
