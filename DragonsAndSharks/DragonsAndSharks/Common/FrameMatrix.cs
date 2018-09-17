using System.Collections.Generic;

namespace DragonsAndSharks.Common
{
    public class FrameMatrix
    {
        public Frame[,] Frames { get; set; }

        public FrameMatrix(int x, int y, List<Frame> frames)
        {
            Frames = new Frame[x, y];
            var i = 0;

            for (int _x = 0; _x < x; _x++)
            {
                for (int _y = 0; _y < y; _y++)
                {
                    Frames[x, y] = frames[i];
                    i++;
                }
                Frames[x, y] = frames[i];
                i++;
            }
        }
        public Frame this[int x, int y]
        {
            get { return Frames[x, y]; }
        }
    }
}
