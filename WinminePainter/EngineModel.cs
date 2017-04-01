using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisCarGame
{
    abstract class EngineModel
    {
        protected bool[,] fixture= new bool[,]{ { false } };
        protected char[,] texture=new char[,]{ {'u' } };
        double x=0;
        double y=0;
        Acceleration acceleration;
        bool disposed=false;

        public void SetPosition(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double GetX()
        {
            return x;
        }
        public double GetY()
        {
            return y;
        }
        public Acceleration GetAcceleration()
        {
            return acceleration;
        }
        public void SetAcceleration(Acceleration acceleration)
        {
            this.acceleration = acceleration;
        }
        public abstract void OnColision();
        public abstract void OnAbroad();
        public abstract void OnEveryLoopStart();
        public bool IsDisposed()
        {
            return disposed;
        }
        public void Dispose()
        {
            disposed = true;
            //fixture = null;
            //texture = null;
        }
        public bool[,] GetFixture()
        {
            return fixture;
        }
        public char[,] GetTexture()
        {
            return texture;
        }
    }
}
