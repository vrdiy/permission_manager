/*******************************************************************
* Name: Anthony Verdi
* Date: 10/26/24
* 
* The MiniLogger class is a simple logger class.
*/
public class MiniLogger
{
    private static List<string> log = new() { };
    private string Prefix;
    public MiniLogger(string prefix)
    {
        Prefix = prefix;
    }
    public void LogError(string message)
    {
        log.Add($"ERROR,{Prefix},{message}");
    }
    public void LogWarning(string message)
    {
        log.Add($"WARNING,{Prefix},{message}");
    }
    public void LogInfo(string message)
    {
        log.Add($"INFO,{Prefix},{message}");
    }
    public void ExportLog()
    {
        var base_dir = AppDomain.CurrentDomain.BaseDirectory;
        var log_dir = Path.Combine(base_dir, "logs");
        Directory.CreateDirectory(log_dir);
        var res = Path.Combine(log_dir, "log.txt");
        File.WriteAllLines(res, log);
    }
}