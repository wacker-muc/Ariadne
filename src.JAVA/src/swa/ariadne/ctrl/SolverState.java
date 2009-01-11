package swa.ariadne.ctrl;

/**
 * The states an {@link AriadneController} may be in.
 * 
 * @author Stephan.Wacker@web.de
 */
public enum SolverState
{
    /** A maze has been created.  No timer is active. */
    Ready,
    
    /** A solver has been created.  The step timer is active. */
    Running,
    
    /** A solver has been created.  The step timer is inactive. */
    Paused,
    
    /** The current maze is solved. */
    Finished,

    /** A maze has been created.  The repeat timer is active. */
    ReadyAndScheduled,

    /** The current maze is solved.  The repeat timer is active. */
    FinishedAndScheduled,
}
