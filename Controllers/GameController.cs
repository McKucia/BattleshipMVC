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

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewGame(Player player)
        {
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

            return View("ChoosePositions");
        }

        [HttpPost]
        public ActionResult ParseCoordinates(List<List<string>> coordinates)
        {
            for(int i = 0; i < coordinates.Count(); i++)
            {
                for (int j = 0; j < coordinates.Count(); j++)
                {
                    if(coordinates[i][j] != null)
                        _player.Board.Panels.Add(new Panel(i, j, coordinates[i][j]));
                    else
                        _player.Board.Panels.Add(new Panel(i, j, "0"));
                }
            }
            return Json(new { success = true });
        }

        public ActionResult PlayGame()
        {
            return View(_player);
        }
    }
}