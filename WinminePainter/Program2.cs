using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WinminePainter
{
    class Program2
    {
        static void Main(string[] args)
        {
            SnakeGame game = new SnakeGame();
            game.RunTest();
            while (true)
            {
                game.KeyDown(Console.ReadKey());
            }
        }
    }

    class SnakeGame
    {
        private bool[,] screen;
        private MinesweeperPainter painter;
        
        Snake snake = new Snake(new Vector(0, 0));

        public SnakeGame()
        {
            screen = new bool[30, 24];
            ClearScreen();
            painter = new MinesweeperPainter("winmine");
        }

        public void Close()
        {
            painter.Dispose();
        }

        public void RunTest()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    ClearScreen();
                    WritePointsToScreen(snake.Update());
                    painter.SetScore(snake.Score);
                    CopyToMinesweer();
                    System.Threading.Thread.Sleep(100);
                }
            });
        }

        public void KeyDown(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    snake.ChangeVelocity(new Vector(0, -1));
                    break;
                case ConsoleKey.DownArrow:
                    snake.ChangeVelocity(new Vector(0, 1));
                    break;
                case ConsoleKey.LeftArrow:
                    snake.ChangeVelocity(new Vector(-1, 0));
                    break;
                case ConsoleKey.RightArrow:
                    snake.ChangeVelocity(new Vector(1, 0));
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }

        private void WritePointsToScreen(Vector[] points)
        {
            foreach (var p in points)
            {
                if (p.X < 0 || p.X > 29 || p.Y < 0 || p.Y > 23)
                {
                    continue;
                }
                screen[p.X, p.Y] = true;
            }
        }

        private void ClearScreen()
        {
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    screen[x, y] = false;
                }
            }
        }

        private void CopyToMinesweer()
        {
            painter.FillScreen(UnitType.Clear);
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    if (screen[x, y])
                    {
                        painter.PaintPoint(x, y, UnitType.Mine);
                    }
                }
            }
            painter.Update();
        }

        class Snake
        {
            private Random rand = new Random();

            Vector food;
            Vector me;
            List<Vector> trail;
            int score = 0;

            bool state = true;

            private Vector velocity;

            public int Score
            {
                get { return score; }
            }

            public Snake(Vector position)
            {
                velocity = new Vector(1, 0);
                me = position;
                food = new Vector(rand.Next(30), rand.Next(24));
                trail = new List<Vector>();
            }

            public Vector[] Update()
            {
                trail.Add(me);
                me += velocity;

                if (trail.Count > 0)
                {
                    trail.RemoveAt(0);
                }

                if (me == food)
                {
                    ++score;
                    trail.Add(me);
                    food = new Vector(rand.Next(30), rand.Next(24));
                }

                if (trail.Count > 1)
                {
                    for (int i = 0; i < trail.Count - 1; i++)
                    {
                        if (me == trail[i])
                        {
                            state = false;
                        }
                    }
                }

                if (me.X < 0)
                {
                    me = new Vector(29, me.Y);
                }
                if (me.X > 29)
                {
                    me = new Vector(0, me.Y);
                }
                if (me.Y < 0)
                {
                    me = new Vector(me.X, 23);
                }
                if (me.Y > 23)
                {
                    me = new Vector(me.X, 0);
                }

                if (state == true)
                {
                    var result = new List<Vector>();
                    result.Add(me);
                    result.Add(food);
                    foreach (var i in trail)
                    {
                        result.Add(i);
                    }

                    return result.ToArray();
                }
                else
                {
                    var result = new List<Vector>();
                    for (int x = 0; x < 30; x++)
                    {
                        for (int y = 0; y < 24; y++)
                        {
                            result.Add(new Vector(x, y));
                        }
                    }

                    return result.ToArray();
                }
            }

            internal void ChangeVelocity(Vector vector)
            {
                velocity = vector;
            }
        }
    }
}
