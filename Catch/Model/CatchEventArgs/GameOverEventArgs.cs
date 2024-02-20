using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catch.Model.CatchEventArgs
{
    public class GameOverEventArgs : EventArgs
    {
        public Ends _end { get; private set; }
        public GameOverEventArgs(Ends end)
        {
            _end = end;
        }
    }
}
