package swa.ariadne.ctrl;

/**
 * The {@link AriadneController} will call these methods on a MazeForm or other client.
 * 
 * @author Stephan.Wacker@web.de
 */
public interface IAriadneEventHandler
{
    /**
     * Called before processing every timer event.
     * @return False if the event is to be ignored.
     */
    boolean Alive();

    /**
     * Creates a new (different) maze.
     */
    void OnNew();

    /// <summary>
    /// Called when the current maze has been solved.
    /// </summary>
    // TODO: void PrepareForNextStart();

    /// <summary>
    /// Called when the controller has been reset.
    /// </summary>
    // TODO: void ResetCounters();
    
    /// <summary>
    /// Called for updating the display before a time consuming operation.
    /// </summary>
    // TODO: void Update();
}
