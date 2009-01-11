package swa.util;

/**
 * Offers methods for looking into the call stack.
 * 
 * @author Stephan.Wacker@web.de
 */
public class Stack
{
    /**
     * @return The name of the calling method.
     * @see "http://nileshk.com/node/81"
     */
    public static String getCallingMethod()
    {
        return getMethodName(2);
    }
    
    /**
     * @param level The depth on the stack, relative to the current pointer.
     * @return The name of the indicated method on the stack.
     */
    private static String getMethodName(int level)
    {
        StackTraceElement e[] = Thread.currentThread().getStackTrace();

        if(e != null && e.length >= level)
        {
            StackTraceElement s = e[level];
            if(s != null)
            {
                return s.getMethodName();
            }
        }
        
        return null;
    }
}
