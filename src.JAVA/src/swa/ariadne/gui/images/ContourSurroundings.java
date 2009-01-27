package swa.ariadne.gui.images;

import java.awt.Color;

/**
 * Comprises the information needed to generate the background gradient around
 * a {@link ContourImage}.
 * This only depends on the background color, not on the actual image objects.
 *
 * @author Stephan.Wacker@web.de
 */
final class ContourSurroundings
{
    //--------------------- Constants

    /** Distance around the image object that will keep the full background color. */
    public static final int ContourDistance =  8;

    /** Additional distance around the image object over which a white background color will be
     * gradually modified to full black.
     * <br> For darker colors, the effective distance will be smaller. */
    public static final int BlurDistanceMax = 16;

    //--------------------- Member variables and Properties

    /** The image's background color. */
    private final Color color;

    /**
     * For each combination of a next left and next right neighbor of a pixel,
     * the influenceRegion is the set of points that are dominated by that pixel
     * and not by one of the neighbors.
     * Pixels closer than ContourDistance-sqrt(2) are not recorded.
     */
    public SurroundingPoints influenceRegions = new SurroundingPoints();

    /**
     * For each combination of a next left and next right neighbor of a pixel,
     * the borderLimit is the set of points on the left and/or right (outside) edge of the influenceRegion.
     * <p>
     * Note: As these points are applied symmetrically on the left and right, only non-negative values are stored.
     */
    public SurroundingPoints borderLimits = new SurroundingPoints();

    /**
     * For each combination of a next left and next right neighbor of a pixel,
     * the contourLimit is the set of points on the left and/or right (inside) edge of the influenceRegion.
     * <p>
     * Note: As these points are applied symmetrically on the left and right, only non-negative values are stored.
     */
    public SurroundingPoints contourLimits = new SurroundingPoints();

    /**
     * @return The width of a region around the image object that should be blurred gradually
     * from the background color to complete black.
     */
    int getBlurDistance()
    {
        float[] hsb = Color.RGBtoHSB(color.getRed(), color.getGreen(), color.getBlue(), null);
        float b = hsb[2];
        return (int)(Math.sqrt(b) * BlurDistanceMax);
    }

    /**
     * @return Gets the width of a region around the image that should not be completely black.
     * That is the ContourDistance plus the {@link #getBlurDistance() BlurDistance}
     * for the image's background color.
     */
    public int getFrameWidth()
    {
        return ContourDistance + getBlurDistance();
    }

    //--------------------- Constructors

    /**
     * Constructor.
     * @param bgColor The image's background color.
     */
    public ContourSurroundings(Color bgColor)
    {
        this.color = bgColor;

        PrepareInfluenceRegions();
    }

    /**
     * Calculate the regions that are influenced by a contour pixel:
     * {@link #influenceRegions}, {@link #borderLimits}, {@link #contourLimits}.
     */
    private void PrepareInfluenceRegions()
    {
        int influenceRange = getFrameWidth();

        // One full pixel inside the fully covered contour range.
        int range2Min = (ContourDistance - 1) * (ContourDistance - 1);
        // Slightly inside the given influence range.
        int range2Max = influenceRange * influenceRange - 1;

        this.influenceRegions = new SurroundingPoints();
        this.borderLimits = new SurroundingPoints();
        this.contourLimits = new SurroundingPoints();

        /* Note: Even when influenceRange = ContourDistance
         * we still have at least one border pixel in the given range
         * on every scan line.
         */

        // Prepare a mapping of d2 (distance-squared) values to alpha values.
        int[] alphaMap = new int[range2Max + 1];
        for (int d2 = range2Min; d2 <= range2Max; d2++)
        {
            double d = Math.sqrt(d2);
            int a = (int)(255 * (d - ContourDistance) / (influenceRange - ContourDistance));
            alphaMap[d2] = a;
        }

        /* Consider four points:
         * - the pixel for which an influence region is calculated, at (0, 0)
         * - a left neighbor at nbL
         * - a right neighbor at nbR
         * - for contour angles >= 180: a pixel inside the object on the middle angle between left and right neighbor: nbI
         */
        for (int nbR = 0; nbR < 8; nbR++)
        {
            int dxR = RelativePoint.NbDX[nbR], dyR = RelativePoint.NbDY[nbR];

            for (int nbL = 0; nbL < 8; nbL++)
            {
                int dxL = RelativePoint.NbDX[nbL], dyL = RelativePoint.NbDY[nbL];

                // Beginning at nbR, turn by the half angle between left and right.
                int nbI = (nbL + ((nbR - nbL + 8) % 8) / 2) % 8;
                int dxI = RelativePoint.NbDX[nbI], dyI = RelativePoint.NbDY[nbI];

                // Avoid a situation where nbR and nbI lie exactly on opposite sides of the contour pixel.
                if (Math.abs(nbR - nbI) == 4)
                {
                    // As the original pixel was opposite nbR, we subtract the nbL vector,
                    // thus placing it on the diagonal.
                    // Note: This point is still sufficiently close to the focus pixel.
                    dxI -= dxL;
                    dyI -= dyL;
                }

                // [start] Calculate the influence region, contour and border limits for the current left and right neighbor.

                // For each scan line: Leftmost and rightmost point in the influence region.
                // Use index [dy + influenceRange].
                // Enter dx + influenceRange which is > 0.
                // Note: We need to collect left and right limits separately.
                //       Otherwise, a non-influence on one side would overwrite an influence on the other side.
                int[] leftLimitsB = new int[2 * influenceRange], rightLimitsB = new int[2 * influenceRange];
                int[] leftLimitsC = new int[2 * influenceRange], rightLimitsC = new int[2 * influenceRange];

                // We want to traverse dx from the center outwards,
                // facilitating the registry of left and right limits.
                for (int i = 1; i < 2 * influenceRange; i++)
                {
                    // 0, 1, -1, 2, -2, ..., r-1, -(r-1)
                    int dx = (i / 2) * (i % 2 == 0 ? +1 : -1);
                    int dxAbs = Math.abs(dx) + influenceRange; // TODO: rename

                    for (int j = 1; j < 2 * influenceRange; j++)
                    {
                        // 0, 1, -1, 2, -2, ..., r-1, -(r-1)
                        int dy = (j / 2) * (j % 2 == 0 ? +1 : -1);

                        // Get the distance to the three points.
                        int d2 = dx * dx + dy * dy;
                        int d2L = (dx - dxL) * (dx - dxL) + (dy - dyL) * (dy - dyL);
                        int d2R = (dx - dxR) * (dx - dxR) + (dy - dyR) * (dy - dyR);
                        int d2I = (dx - dxI) * (dx - dxI) + (dy - dyI) * (dy - dyI);

                        if (d2 <= range2Max)
                        {
                            // This value will be entered into the left/right limits if the current point is in the influence region.
                            int borderLimitsEntry = dxAbs;
                            int contourLimitsEntry = -1;

                            // Check if the center pixel is closest to the point.
                            // For equal distance, the left neighbor should dominate.
                            // Only distances in the relevant range are recorded.
                            if (d2 < d2L && d2 <= d2R && d2 < d2I)
                            {
                                if (range2Min <= d2)
                                {
                                    // The point is partially influenced.
                                    influenceRegions.add(nbL, nbR, new RelativePoint(dx, dy, alphaMap[d2]));
                                }
                                else
                                {
                                    // The point is fully influenced.
                                    contourLimitsEntry = dxAbs;
                                }
                            }
                            else
                            {
                                // Overwrite and remove a previous entry in the left/right border limits.
                                borderLimitsEntry = -1;
                            }

                            if (dx < 0)
                            {
                                leftLimitsB[dy + influenceRange] = borderLimitsEntry;
                                if (d2 < range2Min)
                                {
                                    leftLimitsC[dy + influenceRange] = contourLimitsEntry;
                                }
                            }
                            if (dx > 0)
                            {
                                rightLimitsB[dy + influenceRange] = borderLimitsEntry;
                                if (d2 < range2Min)
                                {
                                    rightLimitsC[dy + influenceRange] = contourLimitsEntry;
                                }
                            }
                        }
                    }
                }

                // [end]

                // [start] Transfer information from the locally collected left and right limits to the static variables.

                for (int i = 0; i < 2 * influenceRange; i++)
                {
                    int dy = i - influenceRange;

                    if (leftLimitsB[i] > 0 || rightLimitsB[i] > 0)
                    {
                        int dx = Math.max(leftLimitsB[i], rightLimitsB[i]) - influenceRange;
                        borderLimits.add(nbL, nbR, new RelativePoint(dx, dy));
                    }

                    if (leftLimitsC[i] > 0 || rightLimitsC[i] > 0)
                    {
                        int dx = Math.max(leftLimitsC[i], rightLimitsC[i]) - influenceRange;
                        contourLimits.add(nbL, nbR, new RelativePoint(dx, dy));
                    }
                }

                // [end]
            }
        }

        // [start] Special handling of horizontal stretches.

        /* On a wide horizontal object border (flat top or bottom),
         * the border points registered above are not sufficient.
         * There might be a gap between the region influenced by the pixels
         * on the left and right ends of the stretch.
         */

        // Add a single border pixel right above or below the horizontal directions.
        int dyS = influenceRange - 1, dyN = -dyS;
        borderLimits.add(RelativePoint.NbW, RelativePoint.NbE, new RelativePoint(0, dyN));
        borderLimits.add(RelativePoint.NbE, RelativePoint.NbW, new RelativePoint(0, dyS));

        dyS = ContourDistance - 1; dyN = -dyS;
        contourLimits.add(RelativePoint.NbW, RelativePoint.NbE, new RelativePoint(0, dyN));
        contourLimits.add(RelativePoint.NbE, RelativePoint.NbW, new RelativePoint(0, dyS));

        // [end]
    }
}
