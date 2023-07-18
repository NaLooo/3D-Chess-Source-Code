using System.Diagnostics;

public class StockFish
{
    string mode;
    Process p;
    public StockFish(string address, string mode)
    {
        this.mode = mode;
        p = new Process();
        p.StartInfo.FileName = address;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        p.StandardOutput.ReadLine();
    }

    public void Input(string fen)
    {
        p.StandardInput.WriteLine("position fen " + fen);
        p.StandardInput.Flush();
        p.StandardInput.WriteLine(mode);
        p.StandardInput.Flush();
    }

    public string GetMove()
    {
        var line = p.StandardOutput.ReadLine();
        while (!line.StartsWith("best"))
        {
            line = p.StandardOutput.ReadLine();
        }

        return line.Split(' ')[1];
    }
}