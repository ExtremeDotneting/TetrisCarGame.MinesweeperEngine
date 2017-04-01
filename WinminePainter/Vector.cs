namespace WinminePainter
{
    struct Vector
    {
        private int x;

        private int y;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Vector v1, Vector v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Vector v1, Vector v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return false;
            }
            return true;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }

            if (((Vector)obj).X == X && ((Vector)obj).Y == Y)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return (x.ToString() + ";" + Y.ToString());
        }
    }
}
