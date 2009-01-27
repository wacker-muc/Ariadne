package swa.util;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

/**
 * Provides access to the resource files in a local directory or JAR file.
 */
public
class ResourceReader
{
    //--------------------- Member variables and Properties

    /** Local filenames within this folder or JAR file. */
    protected final List<String> resourceNames = new ArrayList<String>();

    /** The class whose local directory or JAR file contains the resources. */
    protected final Class<?> clientClass;

    /**
     * @return Number of resource items.
     */
    public int count()
    {
        return resourceNames.size();
    }

    //--------------------- Constructors

    /**
     * Constructor.
     * @param clientClass A class whose local directory or JAR file contains the resources.
     * @param inventoryResourceName Name of a text file resource that contains
     * a list of names of resources managed by this object.
     */
    public ResourceReader(Class<?> clientClass, String inventoryResourceName)
    {
        this.clientClass = clientClass;

        InputStream stream = clientClass.getResourceAsStream(inventoryResourceName);
        BufferedReader reader = new BufferedReader(new InputStreamReader(stream));

        try
        {
            String line;
            while((line = reader.readLine()) != null)
            {
                this.resourceNames.add(line);
            }
        }
        catch (IOException e)
        {
            // TODO do nothing
        }
    }
}
