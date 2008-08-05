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
        /// Called when the controller state is changed.
        /// </summary>
        /// <param name="state"></param>
        /// TODO: Rename to StateHasChanged and call more often.
        /// TODO: Remove state parameter.
        void FixStateDependantControls(SWA.Ariadne.Logic.SolverState state);

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
        /// TODO: Call this from FixStateDependantControls().
        void ResetCounters();
        
        /// <summary>
        /// Called for updating the display before a time consuming operation.
        /// </summary>
        void Update();
    }
}
