using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public RawImage img;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetTexture(img));
    }

    IEnumerator GetTexture(RawImage img)
    {
        var url = $"https://pbs.twimg.com/media/FKD1MchagAMZCYC.jpg";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            img.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }
}
