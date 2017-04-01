using System;
using System.Diagnostics;

namespace WinminePainter
{
    class MinesweeperPainter : IDisposable
    {
        IntPtr buffHandle = IntPtr.Zero;
        IntPtr winHandle = IntPtr.Zero;
        IntPtr baseAddress = IntPtr.Zero;
        Process MinesweeperProc;

        const int START_OFFSET = 0x5360;
        const int SCORE_OFFSET = 0x5194;

        public int Width { get; private set; } = 30;
        public int Height { get; private set; } = 24;
        

        public MinesweeperPainter()
        {
            var proc = new Process();
            proc.StartInfo.FileName = Environment.CurrentDirectory +"/winmine.exe";
            proc.Start();
            while (!proc.Responding) { }
            ProcessThread procThr=GetUIThread(proc);
            if(procThr!=null)
                procThr.PriorityLevel = ThreadPriorityLevel.Highest;
            proc.PriorityClass = ProcessPriorityClass.High;
            winHandle = proc.MainWindowHandle;
            baseAddress = proc.MainModule.BaseAddress;

            buffHandle = Native.OpenProcess(Native.ProcessAccessFlags.All, false, proc.Id);
            MinesweeperProc = proc;
        }
        public static ProcessThread GetUIThread(Process proc)
        {
            if (proc.MainWindowHandle == null) return null;
            int id = GetWindowThreadProcessId(proc.MainWindowHandle, IntPtr.Zero);
            foreach (ProcessThread pt in proc.Threads)
                if (pt.Id == id) return pt;
            return null;
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr procid);
        public void PaintPoint(int x, int y, UnitType type)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException("x");
            }
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException("y");
            }

            int posX = x + 1;
            int posY = y;
            int offset = (posY > 0) ? posY * 30 + (2 * posY) : 0;
            offset += posX;

            byte[] buffer = null;
            switch (type)
            {
                case UnitType.Clear:
                    buffer = new byte[] { 0x40 };
                    break;
                case UnitType.Mine:
                    buffer = new byte[] { 0xCC };
                    break;
            }

            int hi;
            Native.WriteProcessMemory(buffHandle, new IntPtr(baseAddress.ToInt64() + START_OFFSET + offset), buffer, Convert.ToUInt32(buffer.Length), out hi);
        }

        public void SetScore(int score)
        {
            int hi;
            Native.WriteProcessMemory(buffHandle, new IntPtr(baseAddress.ToInt64() + SCORE_OFFSET), BitConverter.GetBytes(score),  sizeof(Int32), out hi);
        }

        public void Update()
        {
            Native.InvalidateRect(winHandle, IntPtr.Zero, true);
        }
      
        public void FillHorizontalLine(int y, UnitType type)
        {
            for (int i = 0; i < 30; i++)
            {
                PaintPoint(i, y, type);
            }
        }

        public void FillVerticalLine(int x, UnitType type)
        {
            for (int i = 0; i < 24; i++)
            {
                PaintPoint(x, i, type);
            }
        }

        public void FillScreen(UnitType type)
        {
            for (int i = 0; i < 24; i++)
            {
                FillHorizontalLine(i, type);
            }
        }

        public void CloseWriter()
        {
            if (buffHandle != IntPtr.Zero)
            {
                Native.CloseHandle(buffHandle);
            }
        }

        public void Dispose()
        {
            CloseWriter();
            MinesweeperProc?.Close();
        }
    }

    [Flags]
    public enum UnitType
    {
        Clear = 0x10,
        Mine = 0xCC
    }
}
