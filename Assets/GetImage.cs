using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetImage : MonoBehaviour
{
    public string url = "";

    Renderer ren;
    Texture tex;
    Transform tr;


    // Start is called before the first frame update
    void Start()
    {
        ren = gameObject.GetComponent<Renderer>();
        tex = gameObject.GetComponent<Texture>();
        tr = gameObject.GetComponent<Transform>();
    }

    public void LoadImage()
    {
        try
        {
            StartCoroutine("GetTexture");
        }
        catch
        {
            Debug.Log(url + " 이미지 불러오기 실");
        }
    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            float ratio = (float)tex.height / (float)tex.width;
            tr.localScale = new Vector3(tr.localScale.x, tr.localScale.x * ratio, 1);

            ren.material.SetTexture("_MainTex", tex);
        }
    }
}
