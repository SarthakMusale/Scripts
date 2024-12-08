using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FetchImages : MonoBehaviour
{
    const string jsonFilePath = "WeaponImages.json";
    [SerializeField] Image[] btnImgs;
    [SerializeField] GunData gunData;

    void Start() => LoadJson();

    IEnumerator FetchImageFromUrl(string url, int i)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching image: " + webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                // Create a Sprite from the texture and apply it to the UI Image
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                btnImgs[i].sprite = sprite;

                Debug.Log("UI Image applied successfully");
            }
        }
    }

    void LoadJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFilePath);

        if (File.Exists(filePath))
        {
            // Read the JSON file as a string
            string jsonContent = File.ReadAllText(filePath);
            Debug.Log("JSON file found: " + jsonContent);

            // Convert the JSON string into GunData object
            gunData = JsonUtility.FromJson<GunData>(jsonContent);

            // Test: Print URLs to the console
            foreach (string url in gunData.array)
            {
                Debug.Log(url);
            }
        }
        else
        {
            Debug.LogError("JSON file not found at path: " + filePath);
        }

        for (int i = 0; i < gunData.array.Length; i++)
        {
            StartCoroutine(FetchImageFromUrl(gunData.array[i], i));
        }
    }
}

[System.Serializable]
public class GunData
{
    public string[] array;
}