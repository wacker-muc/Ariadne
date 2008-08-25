using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SWA.Ariadne.Gui.Painters
{
    /// <summary>
    /// Comprises the callback methods that a client needs to offer to a MazePainter.
    /// </summary>
    public interface IMazePainterClient
    {
        bool Alive { get; }
        Rectangle DisplayRectangle { get; }
        void Update();
    }
}
