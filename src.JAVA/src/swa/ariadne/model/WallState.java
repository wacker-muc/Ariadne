package swa.ariadne.model;

/**
 * States of a wall: open, closed or not determined.
 * 
 * @author Stephan.Wacker@web.de
 */
public
enum WallState
{
    /** Undetermined, needs to be initialized. */
    WS_MAYBE,
    
    /**  Undetermined, should be part of an outline shape. */
    WS_OUTLINE,
    
    /** Open wall, may be passed. */
    WS_OPEN,
    
    /** Closed wall, may not be passed. */
    WS_CLOSED;
}
