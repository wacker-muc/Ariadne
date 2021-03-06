using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Ariadne.Ctrl
{
    /// <summary>
    /// The AriadneController will call these methods on a MazeForm or other client.
    /// </summary>
    public interface IAriadneEventHandler
    {
        /// <summary>
        /// Called before processing every timer event.
        /// Should return false if the event is to be ignored.
        /// </summary>
        bool Alive { get; }

        /// <summary>
        /// Called when the controller's state has changed.
        /// </summary>
        void NotifyControllerStateChanged();

        /// <summary>
        /// Called when starting a new run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNew(object sender, EventArgs e);

        /// <summary>
        /// Called when the current maze has been solved.
        /// </summary>
        void PrepareForNextStart();

        /// <summary>
        /// Called when the controller has been reset.
        /// </summary>
        void ResetCounters();
        
        /// <summary>
        /// Called for updating the display before a time consuming operation.
        /// </summary>
        void Update();
    }
}
