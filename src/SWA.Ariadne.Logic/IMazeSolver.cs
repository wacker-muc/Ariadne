using System;
using System.Collections.Generic;
using System.Text;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Logic
{
    public interface IMazeSolver
    {
        void Step(out MazeSquare sq1, out MazeSquare sq2, out bool forward);
    }
}
