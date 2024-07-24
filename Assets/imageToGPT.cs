using UnityEngine;
using System.Diagnostics;
using System.Collections;
using EasyButtons;
using System;
using System.Collections.Generic;

public class ImageToGPT : MonoBehaviour
{
    private string pythonExecutable = @"Assets\ImgGPT\venv\Scripts\python.exe";
    private string scriptPath = @"Assets\ImgGPT\image_query.py";
    [SerializeField] GameObject image;
    string imagePath = "";

    public string desc1 = "";
    [TextAreaAttribute(1, 10)] public string prompt = @"Here are my instructions:
        I give you an image. Describe what is in the image. 
        Describe each thing one at a time in separate short sentences.
        Only describe a maximum of the 2 most important things in the image.
        Name the item, what the item is doing, and where applicable, what sound it is making.
        Only include sentences where the item is making a sound.
        If the image indicates what sound they are making, include it in the description.
        If the image doesn't indicate the sound, guess a typical fitting sound and include it in the description.
        The description must be as short as possible (maximum of 4 words).
        Don't include the context of the items and the rest of the image, just describe the items and the sound they are making. 
        Give it back in a format where each sentence is separated by a line break. 
        Don't include counting numbers before the sentences and a period at the end of the sentences.";


    void Start()
    {
        imagePath = image.GetComponent<Image>().path;
    }

    [Button]
    public void CallScript()
    {
        StartCoroutine(CallPython());
    }

      public void CallScript(string imgPath)
    {
        imagePath = imgPath;
        StartCoroutine(CallPython());
    }

    IEnumerator CallPython()
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = pythonExecutable;
        psi.Arguments = $"\"{scriptPath}\" \"{imagePath}\" \"{prompt}\"";
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.CreateNoWindow = true;

        string output = "";
        using (Process process = Process.Start(psi))
        {
            output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();  // Wait for the process to exit

            if (!string.IsNullOrEmpty(errors))
            {
                UnityEngine.Debug.LogError("Python Error: " + errors);
            }
        }

        if (!string.IsNullOrEmpty(output))
        {
            try
            {
                RootObject jsonData = JsonUtility.FromJson<RootObject>(output);
                if (jsonData.choices != null && jsonData.choices.Length > 0)
                {
                    UnityEngine.Debug.Log("Content: " + jsonData.choices[0].message.content);
                    desc1 = jsonData.choices[0].message.content;
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No content found in JSON response.");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Failed to parse JSON: " + e.Message);
            }
        }

        yield return null;
    }
}


[System.Serializable]
public class RootObject
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}
