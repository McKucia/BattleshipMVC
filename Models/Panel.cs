using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BattleshipMVC.Models
{
    public class Panel
    {
        public FieldState State { get; set; }
        public Position Position { get; set; }

        public Panel(int row, int column, string state)
        {
            Position = new Position(row, column);
            State = (FieldState)int.Parse(state);
        }

        public bool IsOccupied
        {
            get
            {
                return State == FieldState.AircraftCarrier
                    || State == FieldState.Cruiser
                    || State == FieldState.Destroyer
                    || State == FieldState.Frigate
                    || State == FieldState.Submarine;
            }
        }
    }
}
