package swa.ariadne.gui.tests;

import java.awt.Dimension;

import static junit.framework.Assert.*;
import org.junit.Test;

import swa.ariadne.gui.maze.MazeGeometry;

/**
 * This is a test class for {@link swa.ariadne.gui.maze.MazeGeometry}.
 * 
 * @author Stephan.Wacker@web.de
 */
public class MazeGeometryTest
{
    //--------------------- Unit tests for MazeGeometry.XCoordinate

    /**
     * Test method for {@link MazeGeometry#xCoordinate(int, boolean)}.
     */
    @Test
    public final void MG_testXCoordinate_01()
    {
        MazeGeometry g = new MazeGeometry(8, 2, 6);
        Dimension mazeSize = new Dimension(30, 20);
        Dimension targetSize = new Dimension(mazeSize.width * g.gridWidth + 20 - 2, mazeSize.height * g.gridWidth + 16 - 2); 
        g.setOffset(targetSize, mazeSize);
        
        assertEquals("wrong offset.x", 10, g.offset.x);
        
        for (int expected = 0; expected <= 2; expected++)
        {
            for (int x = 0; x < g.squareWidth; x++)
            {
                testCoordinateX(g, x + expected * g.gridWidth, true, expected);
                testCoordinateX(g, x + expected * g.gridWidth, false, expected);
            }
            for (int x = 0; x < g.wallWidth; x++)
            {
                testCoordinateX(g, g.squareWidth + x + expected * g.gridWidth, true, expected);
                testCoordinateX(g, -1 - x + expected * g.gridWidth, false, expected);
            }
        }
    }

    /**
     * Test method for {@link MazeGeometry#yCoordinate(int, boolean)}.
     */
    @Test
    public final void MG_testYCoordinate_01()
    {
        MazeGeometry g = new MazeGeometry(8, 2, 6);
        Dimension mazeSize = new Dimension(30, 20);
        Dimension targetSize = new Dimension(mazeSize.width * g.gridWidth + 20 - 2, mazeSize.height * g.gridWidth + 16 - 2); 
        g.setOffset(targetSize, mazeSize);
        
        assertEquals("wrong offset.y", 8, g.offset.y);
        
        for (int expected = 0; expected <= 2; expected++)
        {
            for (int x = 0; x < g.squareWidth; x++)
            {
                testCoordinateY(g, x + expected * g.gridWidth, true, expected);
                testCoordinateY(g, x + expected * g.gridWidth, false, expected);
            }
            for (int x = 0; x < g.wallWidth; x++)
            {
                testCoordinateY(g, g.squareWidth + x + expected * g.gridWidth, true, expected);
                testCoordinateY(g, -1 - x + expected * g.gridWidth, false, expected);
            }
        }
    }

    //--------------------- Auxiliary methods

    /**
     * @param g
     * @param x
     * @param leftBiased 
     * @param expected
     */
    private static void testCoordinateX(MazeGeometry g, int x, boolean leftBiased, int expected)
    {
        int xc = g.offset.x + x;
        int actual = g.xCoordinate(xc, leftBiased);
        assertEquals(String.format("wrong XCoordinate(%d, %s)", xc, (leftBiased ? "left" : "right")), actual, expected);
    }

    /**
     * @param g
     * @param y
     * @param topBiased 
     * @param expected
     */
    private static void testCoordinateY(MazeGeometry g, int y, boolean topBiased, int expected)
    {
        int yc = g.offset.y + y;
        int actual = g.yCoordinate(yc, topBiased);
        assertEquals(String.format("wrong YCoordinate(%d, %s)", yc, (topBiased ? "top" : "bottom")), actual, expected);
    }
}
