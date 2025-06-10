namespace HelloWorld.Utilities;
public static class EscapeSingleQuote
{
    public static string Escape(string input)
    {
        return input.Replace("'", "''");
    }
}