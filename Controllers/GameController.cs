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
            //strzał gracza
            //trafiony
            var pa = _bot.Board.PanelAt(row, column).State;
            if (_bot.Board.PanelAt(row, column).IsOccupied)
            {
                //zwiększamy licznik hitów na konkretnym statku
                var ship = _bot.Ships
                    .First(s => s.State == _bot.Board.PanelAt(row, column).State);
                ship.Hits++;

                if (ship.Hits == ship.Size)
                    _bot.Ships.Remove(ship);
                    _bot.Board.PanelAt(row, column).State = FieldState.Hit;
            }
            //nietrafiony
            else
                _bot.Board.PanelAt(row, column).State = FieldState.Miss;

            var hitNeighbors = GetHitNeighbors();
            Position coords;
            if (hitNeighbors.Any())
                coords = SearchingShot();
            else
                coords = RandomShot();

            //strzał bota
            var paa = _player.Board.PanelAt(coords.Row, coords.Column).State;
            if (_player.Board.PanelAt(coords.Row, coords.Column).IsOccupied)
            {
                //zwiększamy licznik hitów na konkretnym statku
                var ship = _player.Ships
                    .First(s => s.State == _player.Board.PanelAt(coords.Row, coords.Column).State);
                ship.Hits++;

                if (ship.Hits == ship.Size)
                    _player.Ships.Remove(ship);
                    _player.Board.PanelAt(coords.Row, coords.Column).State = FieldState.Hit;
            }
            else
                _player.Board.PanelAt(coords.Row, coords.Column).State = FieldState.Miss;

            string winner = null;
            if (!_player.Ships.Any())
            {
                winner = "Bot";
            }
            if (!_bot.Ships.Any())
            {
                winner = _player.Name;
            }

            var players = new List<Player>() { _player, _bot };
            return Json(new { winner = winner });
        }

        private List<Panel> GetNeighbors(Position position)
        {
            //zwraca sąsiadów kafelka
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
            //wszystkie trafne strzały bota
            var hits = _player.Board.Panels.Where(x => x.State == FieldState.Hit).ToList();

            //sąsiednie kafelki trafnych strzałów bota
            foreach (var hit in hits)
                panels.AddRange(GetNeighbors(hit.Position).ToList());
            
            //wykluczamy kafelki w które bot już celował
            return panels.Distinct()
                         .Where(x => x.State != FieldState.Miss && x.State != FieldState.Hit)
                         .Select(x => x.Position)
                         .ToList();
        }

        private Position RandomShot()
        {
            var availablePanels = _player.Board.Panels.Where(x => x.State != FieldState.Hit 
            && x.State != FieldState.Miss)
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

        //funckja losowo wybierająca pozycje statków dla bota
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

                    var affectedPanels = _bot.Board.Panels.Where(x => x.Position.Row >= startrow
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