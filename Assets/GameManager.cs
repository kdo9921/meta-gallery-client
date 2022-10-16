using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
class Art
{
    public int idx;
    public string url;
    public bool show;
}

[System.Serializable]
class Data
{
    public Art[] art;
}

public class GameManager : MonoBehaviour
{
    Data data;
    public string serverUrl = "https://gallery.darae.dev/";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest(this.serverUrl + "data"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("JSON 데이터 불러오기 실패");
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                data = JsonUtility.FromJson<Data>(request.downloadHandler.text);
                setImage();
            }
        }
    }
    private void setImage()
    {
        GameObject[] arts = GameObject.FindGameObjectsWithTag("Art");
        System.Array.Sort<GameObject>(arts, (x, y) => string.Compare(x.name, y.name));
        for (int i = 0; i < arts.Length; i++)
        {
            if (data.art[i].show)
            {
                arts[i].GetComponent<GetImage>().url = serverUrl + "images/" + data.art[i].url;
                arts[i].GetComponent<GetImage>().LoadImage();
            } else
            {
                arts[i].active = false;
            }
        }
    }
}
