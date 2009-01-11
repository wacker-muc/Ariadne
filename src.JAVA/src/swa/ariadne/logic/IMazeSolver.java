package swa.ariadne.logic;

import swa.ariadne.model.MazeSquare;

/**
 * The public methods any MazeSolver must offer.
 * 
 * @author Stephan.Wacker@web.de
 */
public interface IMazeSolver
{
    //--------------------- Types

    /**
     * Output parameters of the Step() method.
     */
    public static class StepResult
    {
        /** First (previously visited) square of the step. */
        public MazeSquare sq1;

        /** Next (neighbor) square. */
        public MazeSquare sq2;

        /** True if the neighbor square was not visited previously. */
        public boolean forward;
    }

    //--------------------- Methods

    /**
     * Reset to the initial state (before the maze is solved).
     * Subclasses should call their super class' method first.
     */
    void reset();

    /**
     * Travel from one visited square to a neighbor square (through an open wall).
     * @param out The output parameters: sq1, sq2, forward.
     */
    void step(StepResult out);

    /**
     * Find a path in the maze from the start to the end point.
     */
    void solve();
}
