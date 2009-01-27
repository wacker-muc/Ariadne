package swa.util;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FilenameFilter;
import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.URL;
import java.util.Collection;
import java.util.List;
import java.util.Vector;
import java.util.regex.Pattern;
import java.util.regex.PatternSyntaxException;

/**
 * Provides auxiliary methods related to directories on a file system.
 *
 * @author Stephan.Wacker@web.de
 */
public final
class Directory
{
    /**
     * @param directoryPath The fully qualified path of a directory.
     * @param filePattern A filename pattern like <code>"*.txt"</code>.
     * @param withSubdirectories When true, the search includes all subdirectories, as well.
     * @return A list of fully qualified file name paths matching the given pattern.
     * @see <a href="http://snippets.dzone.com/posts/show/1875">"http://snippets.dzone.com/posts/show/1875"</a>
     */
    public static List<String> Find(String directoryPath, String filePattern, boolean withSubdirectories)
    {
        // [start] Convert the filePattern to a regular expression pattern.

        StringBuffer sb = new StringBuffer('^');
        for (int i = 0; i < filePattern.length(); i++) {
            char c = filePattern.charAt(i);
            switch(c) {
            case '.':  sb.append("\\."); break;
            case '*':  sb.append(".*"); break;
            case '?':  sb.append('.'); break;
            default:  sb.append(c); break;
            }
        }
        sb.append('$');

        final Pattern nameRE;
        try
        {
            nameRE = Pattern.compile(sb.toString());
        }
        catch (PatternSyntaxException ex)
        {
            System.err.println("Error: RE " + sb.toString() +
                    " didn't compile: " + ex);
            return new Vector<String>();
        }

        // [end]

        FilenameFilter filter = new FilenameFilter()
        {
            @Override
            public boolean accept(File dir, String name)
            {
                return nameRE.matcher(name).matches();
            }

        };

        // Create a reference to the given directory.
        File di = new File(directoryPath);

        // Create an array representing the files in the given directory.
        Collection<File> fis = listFiles(di, filter, withSubdirectories);

        List<String> result = new Vector<String>(fis.size());

        for (File fi : fis)
        {
            result.add(fi.getAbsolutePath());
        }

        return result;
    }

    /**
     * @param directory The directory in which the files are searched.
     * @param filter A {@link FilenameFilter} that decides whether a file is accepted or not
     * or null to accept all files.
     * @param recurse When true, subdirectories are searched, as well.
     * @return A collection of files that match the filter criteria.
     */
    private static Collection<File> listFiles(File directory, FilenameFilter filter, boolean recurse)
    {
        // List of files / directories
        Vector<File> files = new Vector<File>();

        // Get files / directories in the directory
        File[] entries = directory.listFiles();

        // Go over entries
        for (File entry : entries)
        {
            // If there is no filter or the filter accepts the
            // file / directory, add it to the list
            if (filter == null || filter.accept(directory, entry.getName()))
            {
                files.add(entry);
            }

            // If the file is a directory and the recurse flag
            // is set, recurse into the directory
            if (recurse && entry.isDirectory())
            {
                files.addAll(listFiles(entry, filter, recurse));
            }
        }

        // Return collection of files
        return files;
    }

    /**
     * @return The full path to the application directory.
     * @throws IOException When the interpretation of a class URL fails.
     * @throws FileNotFoundException When the class file cannot be determined.
     * @see <a href="http://illegalargumentexception.blogspot.com/2008/04/java-finding-application-directory.html">"http://illegalargumentexception.blogspot.com/2008/04/java-finding-application-directory.html"</a>
     **/
    public static String ApplicationDirectory() throws IOException, FileNotFoundException
    {
        Class<Directory> c = Directory.class;
        String className = c.getName();
        String resourceName = className.replace('.', '/') + ".class";
        ClassLoader classLoader = c.getClassLoader();
        if(classLoader == null)
        {
            classLoader = ClassLoader.getSystemClassLoader();
        }
        URL url = classLoader.getResource(resourceName);

        File classFile = null;
        String szUrl = url.toString();
        if(szUrl.startsWith("jar:file:"))
        {
            try
            {
                szUrl = szUrl.substring("jar:".length(), szUrl.lastIndexOf("!"));
                URI uri = new URI(szUrl);
                classFile = new File(uri);
            }
            catch(URISyntaxException e)
            {
                throw new IOException(e.toString());
            }
        }
        else if(szUrl.startsWith("file:"))
        {
            try
            {
                szUrl = szUrl.substring(0, szUrl.length() - resourceName.length());
                URI uri = new URI(szUrl);
                classFile = new File(uri);
            }
            catch(URISyntaxException e)
            {
                throw new IOException(e.toString());
            }
        }

        if (classFile == null)
        {
            throw new FileNotFoundException(szUrl);
        }

        return classFile.getAbsolutePath();
    }
}
