using System.Collections.Generic;
using System.Text;

public class CoreDebug
{
    public static string CollectionString<T>(IEnumerable<T> items)
    {
        var output = new StringBuilder();
        var index = 0;

        output.AppendLine();

        foreach (var item in items)
        {
            output.AppendFormat("[{0}]: {1}", index, item.ToString());
            output.AppendLine();
            index++;
        }

        if (index == 0)
        {
            output.AppendLine("EMPTY");
        }

        return output.ToString();
    }
}