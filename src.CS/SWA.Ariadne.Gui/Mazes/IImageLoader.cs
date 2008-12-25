using System;
namespace SWA.Ariadne.Gui.Mazes
{
    public interface IImageLoader
    {
        ContourImage GetNext(Random r);
        void Shutdown();
        int YieldNullPercentage { set; }
    }
}
