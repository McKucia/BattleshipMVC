using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleshipMVC.Models
{
    public class Player
    {
        public string Name { get; set; }
        public Board Board { get; set; }
        public List<Ship> Ships { get; set; }
    }
}
