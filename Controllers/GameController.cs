using BattleshipMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleshipMVC.Controllers
{
    public class GameController : Controller
    {
        public static Player _player { get; set; }
        public static Player _bot { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewGame(Player player)
        {
            _player = new Player();
            _player.Name = player.Name;

            _bot = new Player();
            _bot.Name = "Bot";
            RandomBotCoordinates();

            return View("ChoosePositions");
        }

        [HttpPost]
        public ActionResult ParseCoordinates(List<List<string>> coordinates)
        {
            for (int i = 0; i < coordinates.Count(); i++)
            {
                for (int j = 0; j < coordinates.Count(); j++)
                {
                    if (coordinates[i][j] != null)
                        _player.Board.Panels.Add(new Panel(i, j, coordinates[i][j]));
                    else
                        _player.Board.Panels.Add(new Panel(i, j, "0"));
                }
            }
            return Json(new { success = true });
        }

        public ActionResult PlayGame()
        {
            var players = new List<Player>() { _player, _bot };
            return View(players);
        }

        public ActionResult Fire(int row, int column)
        {
            //player's shot
            //hit
            if (_bot.Board.PanelAt(row, column).IsOccupied)
            {
                //increase the hit counter on a specific ship
                var ship = _bot.Ships.First(s => s.State == _bot.Board.PanelAt(row, column).State);
                ship.Hits++;

                if (ship.Hits == ship.Size)
                    _bot.Ships.Remove(ship);

                _bot.Board.PanelAt(row, column).State = FieldState.Hit;
            }
            //miss
            else
                _bot.Board.PanelAt(row, column).State = FieldState.Miss;

            var hitNeighbors = GetHitNeighbors();
            Position coords;
            if (hitNeighbors.Any())
                coords = SearchingShot();
            else
                coords = RandomShot();

            //bot's shot
            if (_player.Board.PanelAt(coords.Row, coords.Column).IsOccupied)
            {
                //increase the hit counter on a specific ship
                var ship = _player.Ships.First(s => s.State == _player.Board.PanelAt(coords.Row, coords.Column).State);
                ship.Hits++;

                if (ship.Hits == ship.Size)
                    _player.Ships.Remove(ship);

                _player.Board.PanelAt(coords.Row, coords.Column).State = FieldState.Hit;
            }
            else
                _player.Board.PanelAt(coords.Row, coords.Column).State = FieldState.Miss;

            string winner = null;
            if (!_player.Ships.Any())
                winner = "Bot";

            if (!_bot.Ships.Any())
                winner = _player.Name;

            var players = new List<Player>() { _player, _bot };
            return Json(new { winner = winner });
        }

        private List<Panel> GetNeighbors(Position position)
        {
            //returns the tile's neighbors
            return new List<Panel>
            {
                _player.Board.PanelAt(position.Row - 1, position.Column),
                _player.Board.PanelAt(position.Row + 1, position.Column),
                _player.Board.PanelAt(position.Row, position.Column - 1),
                _player.Board.PanelAt(position.Row, position.Column + 1)
            };
        }

        private List<Position> GetHitNeighbors()
        {
            List<Panel> panels = new List<Panel>();
            //all bot's hits
            var hits = _player.Board.Panels.Where(x => x.State == FieldState.Hit).ToList();

            //adjacent tiles to hits
            foreach (var hit in hits)
                panels.AddRange(GetNeighbors(hit.Position).ToList());

            //exclude tiles that the bot has already hit
            return panels.Distinct()
                         .Where(x => x.State != FieldState.Miss && x.State != FieldState.Hit)
                         .Select(x => x.Position)
                         .ToList();
        }

        private Position RandomShot()
        {
            var availablePanels = _player.Board.Panels
                .Where(x => x.State != FieldState.Hit && x.State != FieldState.Miss)
                .Select(x => x.Position)
                .ToList();

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            var panelID = rand.Next(availablePanels.Count);

            return availablePanels[panelID];
        }

        private Position SearchingShot()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            var hitNeighbors = GetHitNeighbors();
            var neighborID = rand.Next(hitNeighbors.Count);

            return hitNeighbors[neighborID];
        }

        //function that randomly selects ship positions for the bot
        private void RandomBotCoordinates()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _bot.Board.Panels.Add(new Panel(i, j, "0"));
                }
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            foreach (var ship in _bot.Ships)
            {
                bool isOpen = true;
                while (isOpen)
                {
                    var startCol = rand.Next(0, 10);
                    var startRow = rand.Next(0, 10);
                    int endRow = startRow, endCol = startCol;
                    var orientation = rand.Next(1, 101) % 2;

                    List<int> panelNumbers = new List<int>();

                    if (orientation == 0)
                        for (int i = 1; i < ship.Size; i++)
                            endRow++;
                    else
                        for (int i = 1; i < ship.Size; i++)
                            endCol++;

                    //skip the fields outside of the board
                    if (endRow > 9 || endCol > 9)
                    {
                        isOpen = true;
                        continue;
                    }

                    var affectedPanels = _bot.Board.Panels.Where(x => x.Position.Row >= startRow
                                 && x.Position.Column >= startCol
                                 && x.Position.Row <= endRow
                                 && x.Position.Column <= endCol).ToList();

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