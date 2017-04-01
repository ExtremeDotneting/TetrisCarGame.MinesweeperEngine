using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisCarGame
{
    class GameEngine
    {
        List<EngineModel> engineModelsList = new List<EngineModel>();
        double speedModificator=1;
        int width;
        int height;

        public GameEngine(int width, int height)
        {
            this.width = width;
            this.height = height;          
        }
        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }
        public void AddEngineModel(EngineModel engineModel)
        {
            engineModelsList.Add(engineModel);
        }
        public void NextTick()
        {
            List< EngineModel > buf = new List<EngineModel>();
            for (int i = 0; i < engineModelsList.Count; i++)
            {
                EngineModel engMd = engineModelsList[i];
                if (!(engMd == null || engMd.IsDisposed()))
                    buf.Add(engMd);
            }
            engineModelsList = buf;
            CalcAbroad();
            CalcColisions();
            MoveByAcceleration();
            CallLoopEvents();
        }
        public int[,] GetColisionMap()
        {
            int[,] colisionMap = new int[width, height];
            for (int i = 0; i < colisionMap.GetLength(0); i++)
                for (int j = 0; j < colisionMap.GetLength(1); j++)
                    colisionMap[i, j] = 0;

            for (int i = 0; i < engineModelsList.Count; i++)
            {
                EngineModel engMd = engineModelsList[i];
                ImposeFixture(
                    colisionMap,
                    engMd.GetFixture(),
                    (int)engMd.GetX(),
                    (int)engMd.GetY()
                    );
            }
            return colisionMap;
        }
        public char[,] GetRendered()
        {
            char[,] renderMap = new char[width, height];
            for (int i = 0; i < renderMap.GetLength(0); i++)
            {
                for (int j = 0; j < renderMap.GetLength(1); j++)
                {
                    renderMap[i, j] = ' ';
                }
            }
            for (int i = 0; i < engineModelsList.Count; i++)
            {
                EngineModel engMd = engineModelsList[i];
                ImposeTexture(
                    renderMap,
                    engMd.GetTexture(),
                    (int)engMd.GetX(),
                    (int)engMd.GetY()
                    );
            }
            return renderMap;
        }
        public void ImposeFixture(int[,] colisionMap, bool[,] fixt, int x, int y)
        {
            for (int i = 0; i < fixt.GetLength(1); i++)
                for (int j = 0; j < fixt.GetLength(0); j++)
                    if (fixt[j, i] && ValideRange(x + i, y + j))
                        colisionMap[x + i, y + j]++;

        }
        public void ImposeTexture(char[,] renderedMap, char[,] texture, int x, int y)
        {
            for (int i = 0; i < texture.GetLength(1); i++)
                for (int j = 0; j < texture.GetLength(0); j++)
                    if(ValideRange(x + i, y + j))
                        renderedMap[x + i, y + j] = texture[j, i];
        }
        void CallLoopEvents()
        {
            for (int i = 0; i < engineModelsList.Count; i++)
                engineModelsList[i].OnEveryLoopStart();
        }
        void MoveByAcceleration()
        {
            for (int i = 0; i < engineModelsList.Count; i++)
            {
                EngineModel engMd = engineModelsList[i];
                Acceleration ac = engMd?.GetAcceleration();
                if (ac == null)
                    continue;
                double xModification = 0, yModification = 0;
                double distance = speedModificator * ac.Speed;
                switch (ac.MoveDirection)
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
                engMd.SetPosition(
                    engMd.GetX() + xModification,
                    engMd.GetY() + yModification
                    );
            }
        }
        void CalcColisions()
        {
            int[,] colisionMap = GetColisionMap();
            for (int i = 0; i < engineModelsList.Count; i++)
            {
                EngineModel engMd = engineModelsList[i];
                bool haveColision = CheckColision(
                    colisionMap,
                    engMd.GetFixture(),
                    (int)engMd.GetX(),
                    (int)engMd.GetY()
                    );
                if (haveColision)
                    engMd.OnColision();
            }
        }
        bool CheckColision(int[,] colisionMap, bool[,] fixt, int x, int y)
        {
            for (int i = 0; i < fixt.GetLength(1); i++)
                for (int j = 0; j < fixt.GetLength(0); j++)
                    if (ValideRange(x + i, y + j) && colisionMap[x + i, y + j] >1)
                    {
                        //colisionMap[x + i, y + j]++;
                        return true;
                    }
            return false;
        }
        bool ValideRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }        
        void CalcAbroad()
        {
            for (int i = 0; i < engineModelsList.Count; i++)
            {
                EngineModel engMd = engineModelsList[i];
                if (engMd.GetX()<0 ||
                    engMd.GetY() <0 ||
                    engMd.GetX()+engMd.GetFixture().GetLength(0)>width ||
                    engMd.GetY() + engMd.GetFixture().GetLength(0)>height)
                {
                    engMd.OnAbroad();
                }
            }
        }
    }
    enum MoveDirection
    {
        up,
        down,
        left,
        right
    }
}
