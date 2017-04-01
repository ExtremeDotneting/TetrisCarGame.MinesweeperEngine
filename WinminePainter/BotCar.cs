using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisCarGame
{
    class BotCar : EngineModel
    {
        bool killed=false;
        int lastLifeTicks = 5;

        public BotCar()
        {
            texture = new char[,]
            {
                { ' ', 'H', ' '},
                { 'H', 'H', 'H'},
                { ' ', 'H', ' '},
                { 'H', ' ', 'H'}
            };
            fixture = new bool[,]
            {
                { false, true, false},
                { true, true, true},
                { false, true, false},
                { true, false,true}
            };
        }
        public override void OnColision()
        {
            Kill();
        }
        public override void OnAbroad()
        {
            Kill();
        }
        public override void OnEveryLoopStart()
        {
            if (killed && lastLifeTicks--<=0)
                Dispose();
        }
        void Kill()
        {
            texture = new char[,]
            {
                { '\'', '\'', '%', '\'', '\''},
                { '\'', '\'', '\'', '\'', '\''},
                { '%', '\'', '%', '\'', '%'},
                { '\'', '\'', '\'', '\'', '\''},
                { '\'', '\'','%', '\'', '\''},
                { '\'', '\'', '\'', '\'', '\''},
                { '%',  '\'','\'', '\'', '%'}
            };
            fixture = new bool[,] { { false } };
            SetPosition(GetX() - 1, GetY() - 1);
            killed = true;
        }
    }
}
