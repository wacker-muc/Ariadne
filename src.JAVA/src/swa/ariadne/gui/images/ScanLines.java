package swa.ariadne.gui.images;

import java.awt.Point;
import java.awt.Rectangle;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

/**
 * Contains one {@link ScanLine} for every horizontal line in an image.
 *
 * @author Stephan.Wacker@web.de
 */
final
class ScanLines
{
    //--------------------- Member variables and Properties.

    /** Array of scan lines. */
    private final ArrayList<ScanLine> data;

    /**
     * @return The number of scan lines.
     */
    public int size()
    {
        return data.size();
    }

    /**
     * @param y The scan line index.
     * @return The {@link ScanLine} at the given index.
     */
    private ScanLine get(int y)
    {
        return data.get(y);
    }

    /**
     * @param y The scan line index.
     * @param x The position within the scan line.
     * @param value The value that is written to the given position of the indicated scan line.
     */
    public void set(int y, int x, int value)
    {
        data.get(y).data.set(x, value);
    }

    /**
     * @param y The scan line index.
     * @param x The position within the scan line.
     * @return The value that at the given position of the indicated scan line.
     */
    public int get(int y, int x)
    {
        return data.get(y).data.get(x);
    }

    /**
     * @param y The scan line index.
     * @param value The value that is added to the indicated scan line.
     */
    public void add(int y, int value)
    {
        data.get(y).data.add(value);
    }

    /**
     * @param y The scan line index.
     * @return The number of points on the indicated scan line.
     */
    public int size(int y)
    {
        return data.get(y).size();
    }

    /**
     * @return The index of the first object area's left coordinate.
     * <br> Depending on the number of terminator values, this can be 0 or 1.
     */
    private int firstAreaPosition()
    {
        return (this.data.get(0).size() % 2 == 0 ? 1 : 0);
    }

    //--------------------- Constructors.

    /**
     * Constructor.
     * @param height Number of horizontal scan lines, height of the image.
     * @param width Maximum pixel position on a scan line +1, width of the image.
     * @param bothTerminators If true, the scan line will be terminated on both ends,
     * otherwise only on the right end.
     */
    public ScanLines(int height, int width, boolean bothTerminators)
    {
        this(height);

        for (int i = 0; i < height; i++)
        {
            data.set(i, new ScanLine(width, bothTerminators));
        }
    }

    /**
     * Constructor.
     * <br> Only used by the {@link #emptyClone()} method.
     * @param height Number of horizontal scan lines, height of the image.
     */
    public ScanLines(int height)
    {
        data = new ArrayList<ScanLine>(height);
    }

    /**
     * @return A {@link ScanLines} object of the same size as its source.
     * The terminator entries are copied, but otherwise the scan lines will be empty.
     */
    private ScanLines emptyClone()
    {
        int height = this.size();
        ScanLines result = new ScanLines(height);

        for (int i = 0; i < height; i++)
        {
            result.data.set(i, this.data.get(i).emptyClone());
        }

        return result;
    }

    //--------------------- Methods for managing the object points.

    /**
     * Insert a point into the indicated scan line.
     * @param y The scan line index.
     * @param x The X coordinate of a point on the image object contour.
     */
    public void InsertObjectPoint(int y, int x)
    {
        data.get(y).InsertObjectPoint(x);
    }

    //--------------------- Methods for managing the border and contour points.

    /**
     * Insert two points into the indicated scan line.
     * @param y The scan line index.
     * @param xL The X coordinate of the left border point.
     * @param xR The X coordinate of the right border point.
     */
    public void InsertPair(int y, int xL, int xR)
    {
        data.get(y).InsertPair(xL, xR);
    }

    /**
     * Identifies and eliminates areas in or between the objects defined by the scan lines
     * that are completely enclosed by the objects.
     * <p>
     * Note: The shape described in the scan lines must not touch any image border;
     *       otherwise cut off areas will be considered "inside" and eliminated.
     * @param y0 Index of first valid scan line.
     * @param sy Step between consecutive valid scan lines.
     * @return Number of inside regions found.
     */
    public int EliminateInsideRegions(int y0, int sy)
    {
        /**
         * Java does not allow arrays of generic types.
         * Therefore we encapsulate the ArrayList<Point> inside this class.
         */
        final class PointList
        implements Iterable<Point>
        {
            //--------------------- Member variables and Properties

            /** The list of Points. */
            private final ArrayList<Point> points;

            //--------------------- Constructors

            /**
             * Constructor.
             * @param initialCapacity The initial capacity of the list.
             */
            public PointList(int initialCapacity)
            {
                this.points = new ArrayList<Point>(initialCapacity);
            }

            /**
             * Constructor.
             */
            public PointList()
            {
                this.points = new ArrayList<Point>();
            }

            //--------------------- List behavior

            /**
             * Appends another item to the list.
             * @param point A Point item.
             */
            public void add(Point point)
            {
                points.add(point);
            }

            /**
             * @param index An index within the list of points.
             * @return The item at that position.
             */
            public Point get(int index)
            {
                return points.get(index);
            }

            /**
             * @return The number of items in this list.
             */
            public int size()
            {
                return points.size();
            }

            /**
             * Removes all items from the list.
             */
            public void clear()
            {
                points.clear();
            }

            //--------------------- Iterable implementation

            @Override
            public Iterator<Point> iterator()
            {
                return points.iterator();
            }
        }

        // [start] Create local data structures.

        /* Unresolved enclosed areas up to the current scan line.
         *
         * The Points in this data structure are interpreted like this:
         * y = index in a list or array of scan lines.
         * x = index within the scan line; references the first of a pair of start .. end positions on the scan line.
         *
         * scanLineReferenceGroups[i] is a list of scan line references that belong to the same (potentially) enclosed area.
         */
        ArrayList<PointList> scanLineReferenceGroups = new ArrayList<PointList>();

        /* Unresolved enclosed areas on current and previous scan line.
         * The two lists are used alternatingly; saPrev and saCurr are the indices.
         *
         * The Points in this data structure are interpreted like this:
         * y = index in scanLineReferenceGroups, ID of that group.
         * x = index within the scan line; references the first of a pair of start .. end positions on the scan line.
         */
        PointList[] scanAreas = new PointList[2];
        int saPrev = 0, saCurr = 1;
        scanAreas[0] = new PointList();
        scanAreas[1] = new PointList();

        /* Effective ID of a reference group.
         * When two areas are found to be connected, the one with the greater ID will be united with the other group.
         * For independent groups, unitedGroupIds[i] == i.
         */
        List<Integer> unitedGroupIds = new ArrayList<Integer>();

        // [end]

        // [start] Set up a group referencing the outside region.

        // Add all outside areas on the first scan line to a first reference group.
        // As the scan line is normalized, these areas are between even and odd positions.
        int outsideGroupId = scanLineReferenceGroups.size(); // == 0
        unitedGroupIds.add(outsideGroupId);
        scanLineReferenceGroups.add(new PointList((size(y0) - 1) / 2));
        for (int p = 0, q = 1; q < size(y0); p += 2, q += 2)
        {
            scanLineReferenceGroups.get(outsideGroupId).add(new Point(p, y0));
            scanAreas[saPrev].add(new Point(p, outsideGroupId));
        }

        // [end]

        // [start] Collect the inside areas of all following scan lines in reference groups.

        for (int y = y0 + sy; y < size(); y += sy)
        {
            int a = 0;                  // index into scanAreas[saPrev]
            Point pa = scanAreas[saPrev].get(a);
            int i = pa.x, j = i + 1;    // indices into scanLines
            // Note that the x coordinates are those of the object; the spaces between are two pixels smaller.
            int xi = get(y - sy, i) + 1, xj = get(y - sy, j) - 1;

            // Process the areas between objects on the current scan line.
            // As the scan line is normalized, these areas are between even and odd positions.
            for (int p = 0, q = 1; q < size(y); p += 2, q += 2)
            {
                int xp = get(y, p) + 1, xq = get(y, q) - 1;

                // Advance the previous line's scan area until it lies at or ahead of this line's area.
                // Note: The last (outside) area on the previous line extends to the right image border.
                while (xp > xj)
                {
                    pa = scanAreas[saPrev].get(++a);
                    i = pa.x; j = i + 1;
                    xi = get(y - sy, i) + 1; xj = get(y - sy, j) - 1;
                }

                // Determine which group the current area should belong to.
                int groupId;
                if (xi <= xq) // The two areas overlap: xp <= xi <= xq <= xj.
                {
                    // Add this area to the previous area's group.
                    groupId = pa.y;
                }
                else
                {
                    // Open a new group.
                    groupId = scanLineReferenceGroups.size();
                    unitedGroupIds.add(groupId);
                    scanLineReferenceGroups.add(new PointList((size(y) - 1) / 2));
                }

                // Add the current area to the selected group.
                scanLineReferenceGroups.get(groupId).add(new Point(p, y));
                scanAreas[saCurr].add(new Point(p, groupId));

                // See if other areas on the previous line also overlap with the current area.
                while (a + 1 < scanAreas[saPrev].size())
                {
                    int a1 = a + 1;
                    Point pa1 = scanAreas[saPrev].get(a1);
                    int i1 = pa1.x, j1 = i1 + 1;
                    int xi1 = get(y - sy, i1) + 1, xj1 = get(y - sy, j1) - 1;

                    if (xi1 <= xq)
                    {
                        // Get the other area's group ID.
                        int id1 = pa1.y;

                        // Use the effective group IDs.
                        while (unitedGroupIds.get(id1) < id1) { id1 = unitedGroupIds.get(id1); }
                        while (unitedGroupIds.get(groupId) < groupId) { groupId = unitedGroupIds.get(groupId); }

                        // Unite the two areas' groups.
                        if (groupId < id1)
                        {
                            unitedGroupIds.set(id1, groupId);
                        }
                        else if (id1 < groupId)
                        {
                            unitedGroupIds.set(groupId, groupId = id1);
                        }
                        else
                        {
                            // Do nothing; these separate areas already belong to the same group.
                        }

                        // Advance on the previous scan line.
                        a = a1; pa = pa1; i = i1; j = j1; xi = xi1; xj = xj1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Swap the two scan line indices.
            int saTmp = saPrev;
            saPrev = saCurr;
            saCurr = saTmp;
            scanAreas[saCurr].clear();
        }

        // [end]

        // [start] Mark the areas of all inside groups for elimination.

        int result = 0;

        for (int g = 0; g < scanLineReferenceGroups.size(); g++)
        {
            // Determine effective group id.
            int id = g; while (unitedGroupIds.get(id) < id) { id = unitedGroupIds.get(id); }
            if (id == outsideGroupId)
            {
                continue;
            }

            if (unitedGroupIds.get(g) == g)
            {
                ++result;
            }

            for (Point slr : scanLineReferenceGroups.get(g))
            {
                int y = slr.y, p = slr.x, q = p + 1;

                // Mark the two RL points for deletion.
                set(y, p, -1);
                set(y, q, -1);
            }
        }

        /* As all reference groups other than the outside group have been marked for elimination,
         * only the outside group will be left over.
         * The resulting scanLines define a single connected object shape without inclusions.
         */

        // [end]

        if (result > 0)
        {
            // [start] Remove all inner RL pairs that have been marked for deletion.
            for (int j = 0; j < size(); j++)
            {
                // R-L|RLR...LRL|R-L
                for (int p = size(j) - 4; p >= 2; p -= 2)
                {
                    if (get(j, p) < 0)
                    {
                        get(j).RemoveRange(p, 2);
                    }
                }
            }
            // [end]
        }

        return result;
    }

    /**
     * Returns a copy of this ScanLines object in which a strip along the border is taken away.
     * <p>
     * <em>Example:</em>
     * If the current object describes the outlines of a circle with radius <em>r</em>,
     * the result will be a circle of radius <em>r - margin</em>.
     * @param margin The number of pixels by which the object shall be reduced.
     * @return A set of scan lines that are reduced by the given margin in all directions.
     */
    public ScanLines ShrinkRegion(int margin)
    {
        ScanLines result = this.emptyClone();

        // [start] Start with the border scan lines, indented left and right by the margin distance.

        for (int i = 0; i < result.size(); i++)
        {
            for (int p = 1, q = p + 1; q < size(i); p += 2, q += 2)
            {
                int xp = this.get(i, p) + margin, xq = this.get(i, q) - margin;
                if (xp <= xq)
                {
                    result.InsertPair(i, xp, xq);
                }
            }
        }

        // [end]

        // [start] Overlay the border scan lines, translated by all points on a circle of radius margin.

        int d2Max = (margin + 1) * (margin + 1) - 1;
        for (int dx = 0, dy = margin; dx <= dy; dx++)
        {
            // Four octants, close to Y axis.
            result.IntersectScanLines(this, +dx, +dy);
            result.IntersectScanLines(this, +dx, -dy);
            result.IntersectScanLines(this, -dx, +dy);
            result.IntersectScanLines(this, -dx, -dy);

            // Four octants, close to X axis.
            result.IntersectScanLines(this, +dy, +dx);
            result.IntersectScanLines(this, +dy, -dx);
            result.IntersectScanLines(this, -dy, +dx);
            result.IntersectScanLines(this, -dy, -dx);

            // Advance south if we would leave the circle.
            if ((dx + 1) * (dx + 1) + dy * dy > d2Max)
            {
                dy -= 1;
            }
        }

        // [end]

        return result;
    }

    /**
     * Will leave only the object scan line areas where the (translated) source overlaps with this object.
     * @param source Another {@link ScanLines} object.
     * @param dx Distance in X direction by which the source is translated.
     * @param dy Distance in Y direction by which the source is translated.
     */
    private void IntersectScanLines(ScanLines source, int dx, int dy)
    {
        IntersectScanLines(source, dx, dy, 1);
    }

    /**
     * Will add the source's object scan line areas to those of the current object.
     * @param source Another {@link ScanLines} object.
     */
    public void UniteScanLines(ScanLines source)
    {
        // Uniting the regions on two scan lines is like intersecting the gaps in-between them.  :-)
        IntersectScanLines(source, 0, 0, 0);
    }

    /**
     * Builds an intersection of the source's and this object's scan line areas.
     * Depending on the parameter p0, either the object areas or the gaps in-between are intersected.
     * @param source Another {@link ScanLines} object.
     * @param dx Distance in X direction by which the source is translated.
     * @param dy Distance in Y direction by which the source is translated.
     * @param p0 When 1, areas are intersected; when 0, gaps are intersected.
     */
    private void IntersectScanLines(ScanLines source, int dx, int dy, int p0)
    {
        for (int yt = 0, ys = yt - dy; yt < size(); yt++, ys++)
        {
            int tc = this.size(yt);

            // Skip empty target scan lines.
            if (tc <= 2 && p0 % 2 == 1)
            {
                continue;
            }

            int sc;

            if (ys < 0 || ys >= source.size() || (sc = source.size(ys)) <= 2)
            {
                if (p0 % 2 == 1)
                {
                    // Clear the target scan line.
                    this.get(yt).RemoveRange(1, tc - 2);
                }
                continue;
            }

            int ps = p0, pt = p0, qs = ps + 1, qt = pt + 1; // left and right margin of first region
            int sl = source.get(ys, ps) + dx, sr = source.get(ys, qs) + dx;
            int tl = this.get(yt, pt), tr = this.get(yt, qt);

            while (true)
            {
                if (tr < sl) // c left of b
                {
                    // Remove the current contour region.
                    this.get(yt).RemoveRange(pt, 2); tc -= 2;
                    if (qt >= tc) // behind last target region
                    {
                        break;
                    }
                    tl = this.get(yt, pt); tr = this.get(yt, qt);

                    continue;
                }
                else if (sr < tl) // b left of c
                {
                    // Advance to the next border region.
                    ps += 2; qs += 2;
                    if (qs >= sc) // behind last source region
                    {
                        // Remove the current and all other remaining target regions.
                        this.get(yt).RemoveRange(pt, tc - qt);
                        break;
                    }
                    sl = source.get(ys, ps) + dx; sr = source.get(ys, qs) + dx;

                    continue;
                }

                if (tl < sl)
                {
                    // Adjust the left margin of this target region.
                    this.set(yt, pt, tl = sl);
                }

                if (sr < tr) // c extends to the right of b
                {
                    // Split this contour region.
                    this.get(yt).data.add(pt + 2, sr); // split location, will be corrected in the following iterations
                    this.get(yt).data.add(pt + 3, tr); // current right margin
                    tc += 2;
                    this.set(yt, qt, tr = sr); // split location

                    // Advance to the next source region.
                    ps += 2; qs += 2;
                    if (qs >= sc) // behind last source region
                    {
                        // Remove the remaining target regions.
                        this.get(yt).RemoveRange(pt + 2, tc - qt - 2);
                        break;
                    }
                    sl = source.get(ys, ps) + dx; sr = source.get(ys, qs) + dx;
                }

                // Advance to the next target region.
                pt += 2; qt += 2;
                if (qt >= tc) // behind last target region
                {
                    break;
                }
                tl = this.get(yt, pt); tr = this.get(yt, qt);
            }
        }
    }

    /**
     * @param p0 Index of the left end of the first area in each scan line.
     * @return The number of pixels covered by the scan lines.
     */
    // TODO: eliminate parameter p0; use firstAreaPosition() instead
    public int ScanLinesArea(int p0)
    {
        int result = 0;

        for (int i = 0; i < size(); i++)
        {
            for (int p = p0, q = p + 1; q < size(i); p += 2, q += 2)
            {
                int w = get(i, q) - get(i, p) + 1;
                result += w;
            }
        }

        return result;
    }

    //--------------------- Mask related methods.

    /**
     * @return A {@link Rectangle} that tightly encloses the object defined by these scan lines.
     */
    public Rectangle BoundingBox()
    {
        int bbxMin = Integer.MAX_VALUE, bbxMax = Integer.MIN_VALUE, bbyMin = bbxMin, bbyMax = bbxMax;

        for (int y = 0; y < this.size(); y++)
        {
            if (this.size(y) > 2)
            {
                if (bbyMin == Integer.MAX_VALUE) { bbyMin = y; }
                bbyMax = y;
                bbxMin = Math.min(bbxMin, this.get(y, 1));
                bbxMax = Math.max(bbxMax, this.get(y, size(y) - 2));
            }
        }

        return new Rectangle(bbxMin, bbyMin, bbxMax - bbxMin + 1, bbyMax - bbyMin + 1);
    }
}
