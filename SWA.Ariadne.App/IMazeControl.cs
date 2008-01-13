using System;
using SWA.Ariadne.Model;

namespace SWA.Ariadne.App
{
    public interface IMazeControl
        : SWA.Ariadne.Settings.IAriadneSettingsSource
    {
        string Code { get; }
        bool IsSolved { get; }
        int XSize { get; }
        int YSize { get; }
    }
}
