using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetImage : MonoBehaviour
{
    public int idx;
    public bool isFrame = false;
    public string url = "";

    Renderer ren;
    Texture tex;
    Transform tr;
    GameObject frame;
    GameObject gm;

    // Start is called before the first frame update
    void Start()
    {
        ren = gameObject.GetComponent<Renderer>();
        tex = gameObject.GetComponent<Texture>();
        tr = gameObject.GetComponent<Transform>();
        frame = tr.Find("frame").gameObject;
        gm = GameObject.Find("GameManager");
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
            Debug.Log(url + " 불러오기 실패");
            this.gameObject.active = false;
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            
            if (tex.width > tex.height)
            {
                float ratio = (float)tex.height / (float)tex.width;
                tr.localScale = new Vector3(tr.localScale.x, tr.localScale.x * ratio, 1);
            } else
            {
                float ratio = (float)tex.width / (float)tex.height;
                tr.localScale = new Vector3(tr.localScale.y * ratio, tr.localScale.y, 1);
            }
            if (isFrame)
            {
                frame.GetComponent<Transform>().localScale = new Vector3(1.0f + 0.05f * tr.localScale.y, 1.0f + 0.05f * tr.localScale.x, 0.05f);

            }
            else
            {
                frame.active = false;
            }
            ren.material.SetTexture("_MainTex", tex);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        gm.GetComponent<GameManager>().info_idx = idx;
        gm.GetComponent<GameManager>().info_area = true;
    }
    private void OnTriggerExit(Collider other)
    {
        gm.GetComponent<GameManager>().info_area = false;
    }
}
