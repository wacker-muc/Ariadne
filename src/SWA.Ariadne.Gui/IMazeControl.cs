using System;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.Gui
{
    public interface IMazeControl
        : IMazeControlProperties
        , IMazeControlSetup
        , IMazeDrawer
        , IAriadneSettingsSource
    {
    }
}
