package swa.ariadne.outlines;

import java.awt.Dimension;
import java.util.Random;

import swa.util.Point2DPolar;

/**
 * An n-sided polygon as an {@link OutlineShape}.
 * In a regular polygon, the edges lead from one corner to the next (closest) neighbor.
 * In star shaped polygons, one or more corners are skipped.
 */
class PolygonOutlineShape
extends GeometricOutlineShape
{
    //--------------------- Member Variables and Properties

    /** Number of corners of the polygon. */
    private int corners;
    
    /** Numbers greater than 1 result in a star shape. */
    private int windings;

    /**
     * An angle by which the polygon is rotated.
     * 0.0 will put one corner at the top: (0.0, sz).
     */
    private double slant;

    /** 
     * One half of the sector angle, spanned from the shape's center to two of its corners.
     * The full angle is 2 * PI * windings / corners.
     */
    private double halfSectorAngle;
    /**
     * A slice is half a sector, if the windings are not considered.
     */
    private double sliceAngle;

    /** X coordinate of a vertical edge that separates the polygon's inside and outside. */
    private double xEdge;

    /** For every slice, the number of rotations (by fullSectorAngle) to apply to the point before testing it. */
    private int[] sliceRotationMap;

    //--------------------- IOutlineShape implementation

    @Override
    public boolean get(double x, double y)
    {
        Point2DPolar pp = params.makePoint2DPolar(x, y);

        // Rotate by a multiple of 90 degrees.  This will bring one edge to the south.
        pp.phi += (windings + 1.5) * Math.PI;          // phi > +PI/2 > 0

        // Rotate by the given slant.
        pp.phi += this.slant;

        // Rotate by a multiple of the sliceAngle, depending on the slice this point is in.
        // This brings the given point close to a vertical line that separates inside from outside.
        int slice = ((int)(pp.phi / sliceAngle) % sliceRotationMap.length);
        pp.phi += sliceRotationMap[slice] * sliceAngle;

        // Test if the resulting x coordinate is on the "inside", i.e. to the left of a vertical polygon edge.
        return (pp.asCartesian().x <= xEdge);
    }

    //--------------------- Constructor

    /**
     * Constructor.
     * @param corners Number of corners.
     * @param windings 1 for regular polygons, >1 for star shaped polygons; less than corners/2
     * @param slant An angle by which the polygon is rotated; 0.0 will put one corner at the top: (0.0, sz). 
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public PolygonOutlineShape(int corners, int windings, double slant, Dimension size, OutlineShapeParameters params)
    {
        super(size, params);
        this.corners = corners;
        this.windings = windings;
        this.slant = slant;

        // There are 2*corners slices.
        // The full sector is a triangle formed by the center and two corners connected by an edge.
        this.sliceAngle = Math.PI / this.corners;
        this.halfSectorAngle = this.sliceAngle * this.windings;

        // This is the X coordinate of the two corners of a vertical edge (after an adequate rotation).
        this.xEdge = params.sz * Math.cos(this.halfSectorAngle);

        buildSliceRotationMap();
    }

    /**
     * Constructor used by the {@link swa.ariadne.outlines.factory.OutlineShapeFactory OutlineShapeFactory}.
     * <br> Creates a polygon shape with 3 to 12 corners.
     * @param r A source of random numbers.
     * @param size Nominal size of the shape.
     * @param params Characteristic parameters of the shape: location and size.
     */
    public PolygonOutlineShape(Random r, Dimension size, OutlineShapeParameters params)
    {
        super(size, params);
        
        // We build polygons with up to 12 corners.
        this.corners = 3 + r.nextInt(10);
        this.slant = 0;

        // For high number of corners, we prefer star shapes.
        // 5 corners: 50% / 12 corners: 99%
        if (corners > 4 && r.nextInt(100) < 50 + (corners - 5) * 7)
        {
            // Build a star shaped polygon.
            this.windings = 2 + r.nextInt((corners - 3) / 2);
        }
        else
        {
            // Build a regular polygon.
            this.windings = 1;
        }

        // Apply a random rotation of the whole shape.
        // 3 corners: 55% / 12 corners: 10%
        if (r.nextInt(100) < 70 - corners * 5)
        {
            slant = 2.0 * Math.PI * r.nextDouble();
        }
        else
        {
            // Rotate by half the rotation symmetry angle.
            // This brings a corner (instead of an edge) to the south; applies to all shapes.
            if (r.nextInt(100) < 50)
            {
                slant = 2.0 * Math.PI / corners / 2;
            }

            // Rotate by 90 degrees.
            // This has no effect when corners is a multiple of four.
            // For an even number of corners, the effect is the same as the previous rotation, i.e. neutral.
            // For an odd number of corners, the edge is rotated from south/north to east/west.
            if (r.nextInt(100) < 50)
            {
                slant += Math.PI / 4.0;
            }
        }
    }

    //--------------------- Setup methods

    /**
     * Fills the {@link #sliceRotationMap}.
     */
    private void buildSliceRotationMap()
    {
        // A slice is half a sector.
        int n = 2 * corners;
        sliceRotationMap = new int[n];

        // These sectors are naturally in their regular position (next to the vertical edge).
        int p1 = windings - 1, p2 = (n - 1) - p1;

        int gcd = swa.util.Math.gcd(corners, windings);

        // k is the number of full sector rotations required to bring a slice into a regular position...
        for (int k = 0; k < n / gcd / 2; k++)
        {
            // Set the number of full sector rotations required to bring these slices
            // to their regular positions.
            // k full sectors are equal to k * (2 * windings) slices.
            sliceRotationMap[p1] = sliceRotationMap[p2] = k * (2 * windings);

            // For gcd > 1, there are several identical sub-shapes
            // that can be rotated into the position of the first sub-shape.
            for (int s = 2; s < 2 * gcd; s += 2)
            {
                int s1 = (p1 + s) % n, s2 = (p2 + s) % n;
                sliceRotationMap[s1] = sliceRotationMap[s2] = k * (2 * windings) - s;
            }

            // Rotate to the previous set of slices; these require one rotation more, i.e. k+1.
            p1 = (p1 - 2 * windings + n) % n;
            p2 = (p2 - 2 * windings + n) % n;
        }
    }

    //--------------------- OutlineShape implementation

    @Override
    public OutlineShape makeDistortedCopy(Random r)
    {
        switch (r.nextInt(2))
        {
            default:
            case 0:
                return this.makeSpiralDistortedCopy(r);
            case 1:
                return this.makeRadialWaveDistortedCopy(r);
        }
    }

    /**
     * @param r A source of random numbers.
     * @return A {@link DistortedOutlineShape} based on the current shape
     * and a {@link DistortedOutlineShape#SpiralDistortion SpiralDistortion}.
     * <br> A regular polygon with more than six corners is returned unmodified.
     */
    private ContinuousOutlineShape makeSpiralDistortedCopy(Random r)
    {
        if (this.windings == 1 && this.corners > 6)
        {
            return this;
        }

        // d: radial distance between an edge midpoint and a corner
        double d = params.sz - this.xEdge;

        // dr: the same, relative to the shape size
        double dr = d / params.sz;

        // this would wind one corner to the following corner (over the full shape size)
        double maxSpiralWinding = (double)this.windings / (double)this.corners;

        // this would wind a midpoint to a corner (over their radial distance)
        // this is sufficient to make the corner hang over and form a hook
        maxSpiralWinding /= (2.0 * dr);

        // this will produce even stronger overhanging corners
        maxSpiralWinding *= (this.windings == 1 ? 1.5 : 1.2);

        // Choose an actual winding ratio.
        double w = (0.33 + 0.66 * r.nextDouble()) * maxSpiralWinding;
        w = maxSpiralWinding;
        if (r.nextInt(2) == 0)
        {
            w *= -1;
        }

        DistortedOutlineShape.Distortion distortion = DistortedOutlineShape.SpiralDistortion(params.center, params.sz, w);
        return this.makeDistortedCopy(distortion);
    }

    /**
     * @param r A source of random numbers.
     * @return A {@link DistortedOutlineShape} based on the current shape
     * and a {@link DistortedOutlineShape#RadialWaveDistortion RadialWaveDistortion}.
     * <br> The distortion is exactly aligned with the polygon shape.
     * Only the polygon edges will be bent inwards; the points will not be distorted.
     */
    private ContinuousOutlineShape makeRadialWaveDistortedCopy(Random r)
    {
        int n = this.corners;
        int w = this.windings;

        // Note: When slant = 0, our polygons are always oriented so that one edge is in the south.
        double a;
        if (n % 2 == 0)
        {
            // For multiples of 2, there is also an edge in the north.
            // Choose a to be the negative (relative) angle of the eastern corner on that edge.
            a = -(0.25 - 0.5 * w / n);
        }
        else
        {
            // Otherwise, one corner is in the north.
            a = -0.25;
        }
        a += this.slant / (2.0 * Math.PI);
        
        double bMin = 0.2 + (n + w - 2) * 0.03;         // strongly bent edges
        double bMax = 0.75 + (n - 2) * 0.01;            // slightly bent edges
        double b = bMin + r.nextDouble() * (bMax - bMin);

        DistortedOutlineShape.Distortion distortion = DistortedOutlineShape.RadialWaveDistortion(params.center, n, a, b);
        return this.makeDistortedCopy(distortion);
    }
}
