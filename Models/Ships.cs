using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipMVC.Models
{
    public class AircraftCarrier : Ship
    {
        public AircraftCarrier()
        {
            Name = "AircraftCarrier";
            Size = 5;
            State = FieldState.AircraftCarrier;
        }
    }

    public class Destroyer : Ship
    {
        public Destroyer()
        {
            Name = "Destroyer";
            Size = 4;
            State = FieldState.Destroyer;
        }
    }

    public class Cruiser : Ship
    {
        public Cruiser()
        {
            Name = "Cruiser";
            Size = 3;
            State = FieldState.Cruiser;
        }
    }

    public class Frigate : Ship
    {
        public Frigate()
        {
            Name = "Frigate";
            Size = 2;
            State = FieldState.Frigate;
        }
    }

    public class Submarine : Ship
    {
        public Submarine()
        {
            Name = "Submarine";
            Size = 1;
            State = FieldState.Submarine;
        }
    }
}
