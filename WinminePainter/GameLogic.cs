using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WinminePainter;
using System.Windows.Input;

namespace TetrisCarGame
{
    class GameLogic
    {
        int widthInAuto,  heightInAuto,  playerSpeed,  loopDelay;
        double machinesAtOneLine;
        int pcMovesCount = 1;
        PlayerCar pc;
        GameEngine ge;
        int carW = 3, carH = 4;
        Acceleration acc;
        int machinesAtTenLinesResidue, normalLoopNumber;
        public MinesweeperPainter Painter { get; private set; }

        public GameLogic(double machinesAtOneLine,/* double enemySpeed,*/ int playerSpeed, int loopDelay, MinesweeperPainter painter)
        {
            int widthInAuto = 10;
            int heightInAuto = 6;
            Painter = painter;
            this.widthInAuto = widthInAuto;
            this.heightInAuto = heightInAuto;
            this.machinesAtOneLine = machinesAtOneLine;
            this.playerSpeed = playerSpeed;
            this.loopDelay = loopDelay;
            acc = new Acceleration();
            acc.MoveDirection = MoveDirection.down;
            //acc.Speed = enemySpeed;
            acc.Speed = 1;
            machinesAtTenLinesResidue = (int)(machinesAtOneLine * 10 - ((int)machinesAtOneLine) * 10);
            if (machinesAtTenLinesResidue != 0)
                machinesAtTenLinesResidue = 10 / machinesAtTenLinesResidue;
        }
        public void StartGame()
        {
            Console.Clear();      
            ge = new GameEngine(widthInAuto * carW, heightInAuto*carH);
            pc = new PlayerCar();
            ge.AddEngineModel(pc);
            pc.SetPosition(0, ge.GetHeight()-carH-2);

            StartControlsThread();
            StartRenderThread();

            Random rm = new Random();
            int bigLoopNumber = 0;
            normalLoopNumber = 0;
            while (!pc.IsDisposed())
            {
                if (normalLoopNumber % carH == 0)
                {
                    if (machinesAtTenLinesResidue != 0 && bigLoopNumber % machinesAtTenLinesResidue == 0)
                    {
                        int x = rm.Next(widthInAuto) * carW;
                        BotCar bc = new BotCar();
                        bc.SetPosition(x, 0);
                        bc.SetAcceleration(acc);
                        ge.AddEngineModel(bc);
                    }
                    for (int i = 1; i <= machinesAtOneLine; i++)
                    {
                        int x = rm.Next(widthInAuto) * carW;
                        BotCar bc = new BotCar();
                        bc.SetPosition(x, 0);
                        bc.SetAcceleration(acc);
                        ge.AddEngineModel(bc);
                    }
                    bigLoopNumber++;
                }
                pcMovesCount = 1;
                ge.NextTick();
                normalLoopNumber++;
                Thread.Sleep(loopDelay);      
            }
            OnGameFinish();
        }
        void StartControlsThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                while (!pc.IsDisposed())
                {
                    if (pcMovesCount > playerSpeed)
                        continue;
                    ConsoleKey moveDir = Console.ReadKey(true).Key;
                    switch (moveDir)
                    {
                        case ConsoleKey.LeftArrow:
                            pc.Move(MoveDirection.left, 1);
                            break;
                        case ConsoleKey.RightArrow:
                            pc.Move(MoveDirection.right, 1);
                            break;
                        case ConsoleKey.UpArrow:
                            pc.Move(MoveDirection.up, 1);
                            break;
                        case ConsoleKey.DownArrow:
                            pc.Move(MoveDirection.down, 1);
                            break;
                    }
                }
            }).Start();
        }
        void StartRenderThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                while (!pc.IsDisposed())
                {
                    Console.SetCursorPosition(0, 0);
                    char[,] render = ge.GetRendered();
                    ge.ImposeTexture(render, pc.GetTexture(), (int)pc.GetX(), (int)pc.GetY());
                    //Console.WriteLine(Draw(render));
                    //Console.WriteLine(Draw(ge.GetColisionMap()));
                    //Console.WriteLine("Distance: "+ normalLoopNumber.ToString());

                    Painter.SetScore(normalLoopNumber);
                    Painter.FillScreen(UnitType.Clear);
                    for (int i = 0; i < render.GetLength(0); i++)
                    {
                        for (int j = 0; j < render.GetLength(1); j++)
                        {
                            Painter.PaintPoint(i, j, render[i, j] == ' ' ? UnitType.Clear : UnitType.Mine);
                        }
                    }
                    Painter.Update();

                    Thread.Sleep(50);
                }
            }).Start();
        }
        string Draw(char[,] render)
        {
            string borderStr = "+";

            for (int i = 0; i < render.GetLength(0); i++)
                borderStr += "-";
            borderStr += "+\n";
            string str = "" + borderStr;
            for (int i = 0; i < render.GetLength(1); i++)
            {
                str += "|";
                for (int j = 0; j < render.GetLength(0); j++)
                {
                    str += render[j, i].ToString();
                }
                str += "|\n";
            }
            str += borderStr;
            return str;
        }
        //string Draw(int[,] colisionMap)
        //{
        //    string borderStr = "+";
        //    for (int i = 0; i < colisionMap.GetLength(0); i++)
        //        borderStr += "-";
        //    borderStr += "+\n";
        //    string str = ""+ borderStr;
        //    for (int i = 0; i < colisionMap.GetLength(1); i++)
        //    {
        //        str += "|";
        //        for (int j = 0; j < colisionMap.GetLength(0); j++)
        //        {
        //            str += colisionMap[j, i].ToString();
        //        }
        //        str += "|\n";
        //    }
        //    str+=borderStr;
        //    return str;
        //}
        void OnGameFinish()
        {
            Console.WriteLine("\nYou lose.");
            Console.ReadLine();
            StartGame();
        }
    }
}
