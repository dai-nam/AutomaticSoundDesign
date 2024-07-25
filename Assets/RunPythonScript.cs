using System.Diagnostics;
using UnityEngine;
using EasyButtons;
using System;
using System.Collections.Generic;
using System.Linq;

public class RunPythonScript : MonoBehaviour
{
        private string batchFilePath;
        [SerializeField] string prompt = "";

        List<string> prompts = new List<string>();

        
    void Start()
    {
        batchFilePath = System.IO.Path.Combine(Application.dataPath, "runPython.bat");

        //RunBatchFile();
    }

    [Button]
    public void SetPrompt()
    {
        string tmp = FindObjectOfType<ImageToGPT>().editedDescription;
        string[] lines = tmp.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        lines = lines.Where(s => !string.IsNullOrEmpty(s)).ToArray();
        for(int i = 0; i <lines.Length; i++)
        {
            string currentLine = lines[i];
            currentLine = currentLine.Trim();
            prompts.Add(currentLine);
            print("Prompt " + i + ": " + currentLine);
        }
    }

    [Button]
    public void TestPrompt()
    {
            prompts.Add("Dog barking");
            prompts.Add("Baby laughing");
            print("Prompt 0: " + prompts[0]);
            print("Prompt 1: " + prompts[1]);
    }


    [Button]
    public void RunBatchFile()
    {
        foreach(string currentPrompt in prompts)
        {
            prompt = currentPrompt;
            string name = prompt.Replace(" ", "");
            print("Executing Prompt: "+prompt);

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"{batchFilePath} \"{prompt}\" {name}\"";
           // print($"/c \"{batchFilePath} \"{prompt}\" {name}\"");

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
            process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
 
    }
}
