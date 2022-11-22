using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
class Art
{
    public int idx;
    public string url;
    public bool show;
    public bool frame;
    public bool explan;
    public string title;
    public string artist;
    public string info;
}
[System.Serializable]
class LightSetting
{
    public int idx;
    public string color;
    public float range;
    public float angle;
}
[System.Serializable]
class BGMSetting
{
    public int idx;
    public string url;
    public bool play;
    public string title;
}
[System.Serializable]
class Data
{
    public Art[] art;
    public LightSetting[] light;
    public BGMSetting[] bgm;
}

public class GameManager : MonoBehaviour
{
    Data data;
    public string serverUrl = "https://gallery.darae.dev/";

    public GameObject explanUI;
    public TMP_Text title;
    public TMP_Text artist;
    public TMP_Text info;

    public int info_idx;
    public bool info_area;

    /*
    public Texture2D[] test0;
    public Texture2D[] test1;
    public Texture2D[] test2;
    private LightmapData[] lightmapData;
    Texture2D[] lightmap;
    */

    // Start is called before the first frame update
    void Start()
    {
        //lightmap = test0;
        //setupLightMaps(); //저사양 기기용으로 빌드할때 쓸것
        explanUI.active = false;
        StartCoroutine(GetRequest(this.serverUrl + "data"));
    }

    // Update is called once per frame
    void Update()
    {
        if (info_area)
        {
            if (data.art[info_idx].explan)
            {
                if (Input.GetKeyDown(KeyCode.E)) {
                    if (explanUI.active)
                    {
                        explanUI.active = false;
                    } else
                    {
                        title.text = data.art[info_idx].title;
                        artist.text = "작가 : " + data.art[info_idx].artist;
                        info.text = data.art[info_idx].info;
                        explanUI.active = true;
                    }
                }
            }
        } else
        {
            explanUI.active = false;
        }
    }

    public void setLight()
    {
        GameObject[] bulb = GameObject.FindGameObjectsWithTag("bulb");
        for (int i = 0; i < bulb.Length; i++)
        {
            GameObject circle = bulb[i].transform.GetChild(0).gameObject;
            Light light = bulb[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Light>();
            Color color;
            ColorUtility.TryParseHtmlString(data.light[0].color + "FF", out color);
            Renderer ren = circle.GetComponent<Renderer>();
            ren.material.SetColor("_Color", color);
            ren.material.SetColor("_EmissionColor", color);
            light.color = color;
            light.range = data.light[0].range;
            light.spotAngle = data.light[0].angle;
            var postProcessVolume = GameObject.FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
            Bloom bloom = postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.Bloom>();
            var colorParameter = new UnityEngine.Rendering.PostProcessing.ColorParameter();
            colorParameter.value = color;
            bloom.color.Override(colorParameter);
        }

    }
    /*
    public void setupLightMaps()
    {
        lightmapData = new LightmapData[lightmap.Length / 2];

        for (int i = 0; i < lightmapData.Length; i++)
        {
            lightmapData[i] = new LightmapData();
            lightmapData[i].lightmapDir = lightmap[i*2];
            lightmapData[i].lightmapColor = lightmap[i*2+1];
        }
        LightmapSettings.lightmaps = lightmapData;
    }
    */

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
                setLight();
                setImage();
                StartCoroutine(setBGM());
            }
        }
    }
    private void setImage()
    {
        GameObject[] arts = GameObject.FindGameObjectsWithTag("Art");
        System.Array.Sort<GameObject>(arts, (x, y) => string.Compare(x.name, y.name));
        for (int i = 0; i < arts.Length; i++)
        {
            arts[i].active = false;
        }
        for (int i = 0; i < data.art.Length; i++)
        {
            if (data.art[i].show)
            {
                int idx = data.art[i].idx - 1;
                arts[idx].active = true;
                arts[idx].GetComponent<GetImage>().idx = i;
                arts[idx].GetComponent<GetImage>().url = serverUrl + "images/" + data.art[i].url;
                arts[idx].GetComponent<GetImage>().isFrame = data.art[i].frame;
                arts[idx].GetComponent<GetImage>().LoadImage();
            }
        }
    }

    IEnumerator setBGM()
    {
        yield return new WaitForSeconds(1.5f);
        if (data.bgm.Length != 0)
        {
            if (data.bgm[0].play)
            {
                AudioSource audio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
                
                UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(serverUrl + "bgm/" + data.bgm[0].url, AudioType.MPEG);
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("재생");
                    audio.clip = DownloadHandlerAudioClip.GetContent(www);
                    audio.Play();
                }
            }
        }
        
    }

}
