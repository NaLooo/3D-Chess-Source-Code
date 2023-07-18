using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{
    System.Diagnostics.Process p = new System.Diagnostics.Process();

    float timer = 0f;
    bool flag = false;

    void Start()
    {
        p.StartInfo.FileName = "C:\\Users\\MING YU\\Desktop\\stockfish\\stockfish-windows-x86-64-avx2.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        p.StandardOutput.ReadLine();

        string setupString = "position fen " + "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        p.StandardInput.WriteLine(setupString);
        p.StandardInput.Flush();

        // Process for 5 seconds
        // string processString = "go movetime 1000";

        // Process 20 deep
        string processString = "go depth 5";

        p.StandardInput.WriteLine(processString);
        p.StandardInput.Flush();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (flag) return;
        timer += Time.deltaTime;
        if (timer > 2.0f)
        {
            // var line = p.StandardOutput.ReadLine();
            // Debug.Log(line);
            // if (line.StartsWith("best")) p.Close();
            // timer = 0.5f;
            var line = p.StandardOutput.ReadLine();
            while (!line.StartsWith("best"))
            {
                line = p.StandardOutput.ReadLine();
            }
            Debug.Log(line);
            p.Close();
        }
    }

    // string GetBestMove(string forsythEdwardsNotationString)
    // {
    //     System.Diagnostics.Process p = new System.Diagnostics.Process();
    //     p.StartInfo.FileName = "C:\\Users\\MING YU\\Desktop\\stockfish\\stockfish-windows-x86-64-avx2.exe";
    //     p.StartInfo.UseShellExecute = false;
    //     p.StartInfo.RedirectStandardInput = true;
    //     p.StartInfo.RedirectStandardOutput = true;
    //     p.Start();
    //     p.StandardOutput.ReadLine();

    //     string setupString = "position fen " + forsythEdwardsNotationString;
    //     p.StandardInput.WriteLine(setupString);

    // Process for 5 seconds
    // string processString = "go movetime 1000";

    // Process 20 deep
    // string processString = "go depth 20";

    //     p.StandardInput.WriteLine(processString);
    //     while (p.StandardOutput.Peek() > -1)
    //     {
    //         Debug.Log(p.StandardOutput.ReadLine());
    //     }
    //     string bestMoveInAlgebraicNotation = p.StandardOutput.ReadLine();

    //     p.Close();

    //     return bestMoveInAlgebraicNotation;
    // }
}
