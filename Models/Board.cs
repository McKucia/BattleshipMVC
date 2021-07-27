using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleshipMVC.Models
{
    public class Board
    {
        public List<Panel> Panels { get; set; }

        public Board()
        {
            Panels = new List<Panel>();
        }

        public Panel PanelAt(int row, int column)
        {
            return Panels
                .Where(p => p.Position.Row == row && p.Position.Column == column)
                .SingleOrDefault();
        }

    }
}
