using System;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.App
{
    public interface IMazeControlProperties
    {
        string Code { get; }
        bool IsSolved { get; }
        Maze Maze { get; }
        int XSize { get; }
        int YSize { get; }
    }
}
