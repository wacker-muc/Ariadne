package swa.ariadne.settings;

import java.util.HashMap;

import swa.util.IParameterFile;
import swa.util.ParameterFile;

/**
 * Short introduction sentence. More description afterwards.
 */
public final class RegisteredOptions
{
    //--------------------- Constants

    private static final String PARAM_FILE = "~/.ariadne";

    public static final String OPT_SHOW_DETAILS_BOX = "show details box";
    public static final String OPT_BLINKING = "paint blinking end square";
    public static final String OPT_EFFICIENT_SOLVERS = "use efficient solvers";
    public static final String OPT_STEPS_PER_SECOND = "steps per second";
    public static final String OPT_IMAGE_NUMBER = "image number";
    public static final String OPT_IMAGE_MIN_SIZE = "image minimum size";
    public static final String OPT_IMAGE_MAX_SIZE = "image maximum size";
    public static final String OPT_IMAGE_FOLDER = "image folder";
    public static final String OPT_IMAGE_SUBTRACT_BACKGROUND = "subtract uniform image background color";
    public static final String OPT_BACKGROUND_IMAGES = "display background images";
    public static final String OPT_BACKGROUND_IMAGE_FOLDER = "background image folder";
    public static final String OPT_OUTLINE_SHAPES = "add outline shapes";
    public static final String OPT_IRREGULAR_MAZES = "build irregular mazes";
    public static final String OPT_PAINT_ALL_WALLS = "paint all maze walls";
    public static final String OPT_MULTIPLE_MAZES = "create multiple mazes";
    public static final String OPT_LOG_SOLVER_STATISTICS = "log solver statistics";

    // Note: The screen saver options dialog will recreate the registry entry and store only the OPT_ values.
    //       All other values will be lost.
    public static final String SAVE_IMAGE_PATHS = "immediate image paths";

    //--------------------- Methods for reading typed parameters.

    /**
     * @param name The name of the requested parameter.
     * @return A boolean value from the application's registered parameters.
     * <br> If no value has been registered, an application defined default value is returned.
     */
    public static boolean GetBoolSetting(String name)
    {
        HashMap<String, Boolean> defaults = new HashMap<String, Boolean>();
        defaults.put(OPT_BACKGROUND_IMAGES, false);
        defaults.put(OPT_PAINT_ALL_WALLS, false);
        defaults.put(OPT_LOG_SOLVER_STATISTICS, false);
        defaults.put(OPT_SHOW_DETAILS_BOX, true);
        defaults.put(OPT_BLINKING, true);
        defaults.put(OPT_EFFICIENT_SOLVERS, true);
        defaults.put(OPT_IMAGE_SUBTRACT_BACKGROUND, true);
        defaults.put(OPT_OUTLINE_SHAPES, true);
        defaults.put(OPT_IRREGULAR_MAZES, true);
        defaults.put(OPT_MULTIPLE_MAZES, true);

        boolean defaultValue = (defaults.containsKey(name) ? defaults.get(name) : false);
        return GetBoolSetting(name, defaultValue);
    }

    /**
     * @param name The name of the requested parameter.
     * @param defaultValue A default value.
     * @return A boolean value from the application's registered parameters.
     * <br> If no value has been registered, the given default value is returned.
     */
    private static boolean GetBoolSetting(String name, boolean defaultValue)
    {
        int value = (defaultValue == false ? 0 : 1);

        value = GetIntSetting(name, value);

        return (value != 0);
    }

    /**
     * @param name The name of the requested parameter.
     * @return An integer value from the application's registered parameters.
     * <br> If no value has been registered, an application defined default value is returned.
     */
    public static int GetIntSetting(String name)
    {
        HashMap<String, Integer> defaults = new HashMap<String, Integer>();
        defaults.put(OPT_STEPS_PER_SECOND, 200);
        defaults.put(OPT_IMAGE_NUMBER, 0);
        defaults.put(OPT_IMAGE_MIN_SIZE, 300);
        defaults.put(OPT_IMAGE_MAX_SIZE, 400);

        int defaultValue = (defaults.containsKey(name) ? defaults.get(name) : 0);
        return GetIntSetting(name, defaultValue);
    }

    /**
     * @param name The name of the requested parameter.
     * @param defaultValue A default value.
     * @return An integer value from the application's registered parameters.
     * <br> If no value has been registered, the given default value is returned.
     */
    private static int GetIntSetting(String name, int defaultValue)
    {
        int value = defaultValue;

        String sValue = GetStringSetting(name, null);
        if (sValue != null)
        {
            value = Integer.parseInt(sValue); // TODO: exception handling
        }

        return value;
    }

    /**
     * @param name The name of the requested parameter.
     * @return A string value from the application's registered parameters.
     * <br> If no value has been registered, an empty String is returned.
     */
    public static String GetStringSetting(String name)
    {
        return GetStringSetting(name, "");
    }

    /**
     * @param name The name of the requested parameter.
     * @param defaultValue A default value.
     * @return A string value from the application's registered parameters.
     * <br> If no value has been registered, the given default value is returned.
     */
    private static String GetStringSetting(String name, String defaultValue)
    {
        String value = defaultValue;

        IParameterFile params = getParameterFile();
        if (params != null)
        {
            value = params.get(name);
        }

        return value;
    }

    /**
     * @return The {@link IParameterFile} of this application.
     */
    public static IParameterFile getParameterFile()
    {
        return new ParameterFile(PARAM_FILE);
    }
}
