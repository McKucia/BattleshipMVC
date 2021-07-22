using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BattleshipMVC.Models
{
    public enum FieldState
    {
        Empty = 0,
        AircraftCarrier = 5,
        Cruiser = 3,
        Destroyer = 4,
        Frigate = 2,
        Submarine = 1,
        Miss = 10,
        Hit = 11
    }
}
