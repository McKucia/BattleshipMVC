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

        public void RandomBotCoordinates()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Board.Panels.Add(new Panel(i, j, "0"));
                }
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            foreach (var ship in Ships)
            {
                bool isOpen = true;
                while (isOpen)
                {
                    var startcolumn = rand.Next(0, 10);
                    var startrow = rand.Next(0, 10);
                    int endrow = startrow, endcolumn = startcolumn;
                    var orientation = rand.Next(1, 101) % 2;

                    List<int> panelNumbers = new List<int>();

                    if (orientation == 0)
                        for (int i = 1; i < ship.Size; i++)
                            endrow++;
                    else
                        for (int i = 1; i < ship.Size; i++)
                            endcolumn++;

                    //Pomijamy pola poza planszą
                    if (endrow > 9 || endcolumn > 9)
                    {
                        isOpen = true;
                        continue;
                    }

                    var affectedPanels = Board.Panels.Where(x => x.Position.Row >= startrow
                                 && x.Position.Column >= startcolumn
                                 && x.Position.Row <= endrow
                                 && x.Position.Column <= endcolumn).ToList();

                    if (affectedPanels.Any(x => x.IsOccupied))
                    {
                        isOpen = true;
                        continue;
                    }

                    foreach (var panel in affectedPanels)
                    {
                        panel.State = ship.State;
                    }
                    isOpen = false;
                }
            }
        }
    }
}
