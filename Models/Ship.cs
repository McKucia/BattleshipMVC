using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipMVC.Models
{
    public abstract class Ship
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Hits { get; set; }
        public FieldState State { get; set; }
    }
}
