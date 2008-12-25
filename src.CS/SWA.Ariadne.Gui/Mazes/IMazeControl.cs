using System;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui.Mazes
{
    public interface IMazeControl
        : IMazeControlProperties
        , IMazeControlSetup
        , IAriadneSettingsSource
    {
    }
}
