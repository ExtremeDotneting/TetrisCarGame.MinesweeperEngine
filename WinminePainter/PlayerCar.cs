using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TetrisCarGame
{
    class PlayerCar : EngineModel
    {
        public PlayerCar() {
            texture = new char[,]
            {
                { ' ', 'O', ' '},
                { 'O', 'O', 'O'},
                { ' ', 'O', ' '},
                { 'O', ' ', 'O'}
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
            Dispose();
        }
        public override void OnAbroad()
        {
            Dispose();
        }
        public override void OnEveryLoopStart() { }
        public void Move(MoveDirection moveDirection, double distance)
        {
            double xModification = 0, yModification = 0;
            switch (moveDirection)
            {
                case MoveDirection.up:
                    yModification -= distance;
                    break;
                case MoveDirection.down:
                    yModification += distance;
                    break;
                case MoveDirection.left:
                    xModification -= distance;
                    break;
                case MoveDirection.right:
                    xModification += distance;
                    break;
            }
            SetPosition(
                GetX() + xModification,
                GetY() + yModification
                    );
        }
    }
}
