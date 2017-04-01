using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using WinminePainter;

namespace TetrisCarGame
{
    class Game
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 20;
            Console.WindowHeight = 3;
            Console.SetWindowPosition(0, 0);
            Console.Title = @"TetrisCarGame.MinerEdition";
            Console.WriteLine("Enter arrows in console to move!");

            double machinesAtOneLine=0.5;//количество машин генерируемых на одну линию
            int playerSpeed=3;//скорость игрока
            int loopDelay=500;//пауза между тактами игры, влияет на скорость врагов
            GameLogic gl = new GameLogic(
                machinesAtOneLine, 
                playerSpeed, 
                loopDelay, 
                new MinesweeperPainter()
                );
            gl.StartGame();
            
        }
    }

}
