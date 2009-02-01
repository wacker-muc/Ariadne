namespace SWA.Ariadne.Logic
{
    /// <summary>
    /// The states an AriadneController may be in.
    /// </summary>
    public enum SolverState
    {
        /// <summary>
        /// A maze has been created.  No timer is active.
        /// </summary>
        Ready = 1,

        /// <summary>
        /// A solver has been created.  The step timer is active.
        /// </summary>
        Running,

        /// <summary>
        /// A solver has been created.  The step timer is inactive.
        /// </summary>
        Paused,

        /// <summary>
        /// The current maze is solved.
        /// </summary>
        Finished,

        /// <summary>
        /// Variant of the basic states, when the repeat timer is active.
        /// </summary>
        Scheduled = 16,

        /// <summary>
        /// A maze has been created.  The repeat timer is active.
        /// </summary>
        ReadyAndScheduled = Ready | Scheduled,

        /// <summary>
        /// The current maze is solved.  The repeat timer is active.
        /// </summary>
        FinishedAndScheduled = Finished | Scheduled,
    }
}

