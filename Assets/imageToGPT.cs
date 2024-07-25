using UnityEngine;
using System.Diagnostics;
using System.Collections;
using EasyButtons;
using System;
using System.IO;
using System.Collections.Generic;

public class ImageToGPT : MonoBehaviour
{
    private string pythonExecutable = @"Assets\ImgGPT\venv\Scripts\python.exe";
    private string imgToTxtScript = @"Assets\ImgGPT\image_query.py";
    private string editGptResponseScript =  @"Assets\ImgGPT\chatGPT_query.py";


    [SerializeField] GameObject image;
    string imagePath = "";

    public string imgDescription = "";
    public string editedDescription = "";

    [SerializeField] bool shouldEditGptResponse = false;

    /*
    [TextAreaAttribute(1, 20)] public string prompt = @"Here are my instructions:
        I give you an image. Describe what is in the image. 
        Describe each thing one at a time in separate short sentences.
        Only describe a maximum of the 2 most important things in the image. 
        The items need to be distinct, so don't describe the same item twice.
        Name the item, what the item is doing, and where applicable, what sound it is making.
        Only include sentences where the item is making a sound. Don't give me a description of items that don't make a sound.
        Use general words for the items, don't be too specific.
        If the image indicates what sound they are making, include it in the description.
        If the image doesn't indicate the sound, guess a typical fitting sound and include it in the description.
        The description must be as short as possible. Never use more than 4 words.
        Don't include the context of the items and the rest of the image, just describe the items and the sound they are making. 
        Give it back in a format where each sentence is separated by a line break. 
        Don't include counting numbers before the sentences and a period at the end of the sentences.";
*/
   private string imgToTxtPrompt = @"I give you an image. Describe what is in the image. 
        Describe each object and their actions one at a time in separate very short sentences.
        Name the object. Only include objects which actions are probably making a sound.
        Only describe a maximum of the 3 most striking objects in the image. For each object 1 sentence. Never describe the same object and category of objects twice.
        Don't include the context of the objects and the rest of the image.";


    void Start()
    {
        imagePath = image.GetComponent<Image>().path;
    }

    [Button]
    public void CallImgToTxt()
    {
        StartCoroutine(ImgToTxt(imgToTxtPrompt));
    }

 
    //Calls the python script image_query.py
    IEnumerator ImgToTxt(string prompt)
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = pythonExecutable;
        psi.Arguments = $"\"{imgToTxtScript}\" \"{imagePath}\" \"{prompt}\"";
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
                    imgDescription = jsonData.choices[0].message.content;
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


    private string editPrompt = @"You get a description of objects from an image. Your task is to filter out sentences or rewrite them to fit my needs.
        If there are sentences which describe the same object or category of objects, just take the first one.
        Only use general basic words for objects and actions, don't be specific about visual properties like color, types etc. But don't alter the meaning.
        Each sentence must be as short as possible. Never use more than 4 words for a sentence.
        Separate each sentence by a line break. 
        Don't include counting numbers before the sentences and a period at the end of the sentences.
        Here is the original description: ";


    [Button]
    public void CallEditGptResponse()
    {
        if(!shouldEditGptResponse)
        {
            print("Edit GPT Response ist set to false. No Editing done.");
            editedDescription = imgDescription;
            return;
        }

        StartCoroutine(EditGptResponse(editPrompt+imgDescription));
    }

    IEnumerator EditGptResponse(string prompt)
    {
       {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = pythonExecutable;
        psi.Arguments = $"\"{editGptResponseScript}\" \"{prompt}\"";
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
            print(output);
            try
            {
                RootObject jsonData = JsonUtility.FromJson<RootObject>(output);
                if (jsonData.choices != null && jsonData.choices.Length > 0)
                {
                    UnityEngine.Debug.Log("Content: " + jsonData.choices[0].message.content);
                    editedDescription = jsonData.choices[0].message.content;
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
