using System;
using SWA.Ariadne.Logic;
using SWA.Ariadne.Settings;

namespace SWA.Ariadne.App
{
    public interface IMazeControl
        : IMazeControlProperties
        , IMazeControlSetup
        , IMazeDrawer
        , IAriadneSettingsSource
    {
    }
}
