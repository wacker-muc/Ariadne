using System;

namespace SWA.Ariadne.Settings
{
    /// <summary>
    /// Supports exchanging data between an application object and a settings dialog.
    /// </summary>
    public interface IAriadneSettingsSource
    {
        void FillParametersInto(AriadneSettingsData data);
        bool TakeParametersFrom(AriadneSettingsData data);
    }
}
