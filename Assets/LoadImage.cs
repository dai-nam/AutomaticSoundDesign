using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadImage : MonoBehaviour
{
    [SerializeField] GameObject image;
    private string imagePath = "";
    public RawImage rawImage;

    void Start()
    {
        imagePath = image.GetComponent<Image>().path;
        LoadAndDisplayImage();
    }

    void LoadAndDisplayImage()
    {
        // Load the image as a Texture2D
        byte[] imageData = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        // Set the texture to the RawImage component
        rawImage.texture = texture;
        rawImage.SetNativeSize();
    }
}
