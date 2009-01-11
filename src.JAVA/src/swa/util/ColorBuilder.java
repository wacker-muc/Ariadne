package swa.util;

import java.awt.Color;
import java.lang.Math;
import java.util.Random;

/**
 * Provides methods for defining colors in the HSB color model.
 * 
 * @author Stephan.Wacker@web.de
 */
public final
class ColorBuilder
{
    //--------------------- Constants     
    
    /** Maximum hue value. */
    public static final float MaxHue = 1.0F;
    /** Maximum saturation value. */
    public static final float MaxSaturation = 1.0F;
    /** Maximum brightness value. */
    public static final float MaxBrightness = 1.0F;
    
    //--------------------- Static methods
    
    /**
     * @param h hue, 0.0 .. 1.0
     * @param s saturation, 0.0 .. 1.0
     * @param b brightness, 0.0 .. 1.0
     * @return The resulting color.
     */
    public static Color convertHSBToColor(float h, float s, float b)
    {
        Color result = new Color(Color.HSBtoRGB(h / MaxHue, s, b));
        return result;
    }

    /**
     * Computes two random colors.
     * Both saturation and brightness values will be in the range given by the two reference colors.
     * The hues will have a minimum distance of one quadrant (25%) in the HSB color model.
     * 
     * @param ref1 A bright reference color.
     * @param ref2 A dark reference color.
     * @return An array of two colors: {foregroundColor, backgroundColor}.
     */
    public static Color[] suggestColors(Color ref1, Color ref2)
    {
        Random r = RandomFactory.createRandom();

        float hDist = 0.25F * MaxHue;
        float sDist = 0.50F * MaxSaturation;
        float bDist = 0.50F * MaxBrightness;

        float[] ref1HSB = Color.RGBtoHSB(ref1.getRed(), ref1.getGreen(), ref1.getBlue(), null);        
        float[] ref2HSB = Color.RGBtoHSB(ref2.getRed(), ref2.getGreen(), ref2.getBlue(), null);
        
        float sMin = Math.min(ref1HSB[1], ref2HSB[1]);
        float sMax = Math.max(ref1HSB[1], ref2HSB[1]);
        float bMin = Math.min(ref1HSB[2], ref2HSB[2]);
        float bMax = Math.max(ref1HSB[2], ref2HSB[2]);

        float sRange = sMax - sMin;
        float bRange = bMax - bMin;

        //--------------------- Choose a random backward color, using the HSB model

        float h = ColorBuilder.MaxHue;
        float s = sMin;
        float b = bMin;

        h *= (float)r.nextDouble();
        s += (float)(((1.0F - sDist) * sRange) * r.nextDouble());
        b += (float)(((1.0F - bDist) * bRange) * r.nextDouble());

        Color backwardColor = ColorBuilder.convertHSBToColor(h, s, b);

        //--------------------- Choose a random forward color: at a distance and brighter than the backward color

        // Minimum hue distance: 1/4 of the hue range.
        float hueMin = h + hDist;
        float hueMax = h - hDist + ColorBuilder.MaxHue;

        h = hueMin + (hueMax - hueMin) * (float)r.nextDouble();
        if (h > ColorBuilder.MaxHue) { h -= ColorBuilder.MaxHue; }

        s += sDist * sRange;
        b += bDist * bRange;

        //saturation += (float)((maxSaturation - saturation) * r.NextDouble());
        //brightness += (float)((maxBrightness - brightness) * r.NextDouble());

        Color forwardColor = ColorBuilder.convertHSBToColor(h, s, b);
        
        //---------------------
        
        System.out.println("SuggestColors: ref1 = " + ref1);
        System.out.println("SuggestColors: fwd  = " + forwardColor);
        System.out.println("SuggestColors: bkwd = " + backwardColor);
        System.out.println("SuggestColors: ref2 = " + ref2);
        
        return new Color[] { forwardColor, backwardColor };
    }

    /**
     * @param a First hue value.
     * @param b Second hue value.
     * @return The (absolute) difference of two hue values.
     * The result is between 0.0 and 0.5 * MaxHue.
     */
    public static float hueDifference(float a, float b)
    {
        float result = Math.abs(a - b);
        if (result > MaxHue / 2)
        {
            result = MaxHue - result;
        }
        return result;
    }
}
