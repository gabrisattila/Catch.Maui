using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catch.Maui.ViewModel
{
    public class Field : ViewModelBase
    {
        private Brush _color;
        public Brush Color { get { return _color; } set { _color = value; OnPropertyChanged(); } }
        public int X { get; set; }
        public int Y { get; set; }
        public int Number { get; set; }
        public DelegateCommand StepCommand { get; set; }
    }
}
