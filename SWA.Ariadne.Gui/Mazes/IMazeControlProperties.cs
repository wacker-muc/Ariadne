﻿using System;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.Gui.Mazes
{
    public interface IMazeControlProperties
    {
        string Code { get; }
        bool IsFinished { get; }
        Maze Maze { get; }
        int XSize { get; }
        int YSize { get; }
    }
}
