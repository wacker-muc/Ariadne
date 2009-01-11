package swa.ariadne.settings;

/**
 * Set of parameters defining the constructed maze.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class AriadneSettingsData
{
    /**
     * Three options for the visibility of walls.
     */
    public enum WallVisibilityEnum
    {
        /** All walls are painted when the maze is first displayed. */
        Always,
        /** Walls have zero width and are never painted. */
        Never,
        /** Walls are only painted around the already visited squares. */   
        WhenVisited,
    }
    
    /** Defines which walls are visible. */
    public WallVisibilityEnum wallVisibility;
}
