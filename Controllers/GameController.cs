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
            //gracz
            _player = new Player();
            _player.Name = player.Name;
            _player.Ships = new List<Ship>()
            {
                new Destroyer(),
                new Submarine(),
                new Cruiser(),
                new Frigate(),
                new AircraftCarrier()
            };
            _player.Board = new Board();

            //bot
            _bot = new Player();
            _bot.Name = "Bot";
            _bot.Ships = new List<Ship>()
            {
                new Destroyer(),
                new Submarine(),
                new Cruiser(),
                new Frigate(),
                new AircraftCarrier()
            };

            _bot.Board = new Board();
            _bot.RandomBotCoordinates();

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
            if (_bot.Board.PanelAt(row, column).IsOccupied)
                _bot.Board.PanelAt(row, column).State = FieldState.Hit;
            else
                _bot.Board.PanelAt(row, column).State = FieldState.Miss;

            var hitNeighbors = GetHitNeighbors();
            Position coords;
            if (hitNeighbors.Any())
                coords = SearchingShot();
            else
                coords = RandomShot();

            if(_player.Board.PanelAt(coords.Row, coords.Column).IsOccupied)
                _player.Board.PanelAt(coords.Row, coords.Column).State = FieldState.Hit;
            else
                _player.Board.PanelAt(coords.Row, coords.Column).State = FieldState.Miss;

            var players = new List<Player>() { _player, _bot };
            return View("PlayGame", players);
        }

        private List<Panel> GetNeighbors(Position position)
        {
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
            var hits = _player.Board.Panels.Where(x => x.State == FieldState.Hit).ToList();

            foreach (var hit in hits)
            {
                panels.AddRange(GetNeighbors(hit.Position).ToList());
            }
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
    }
}