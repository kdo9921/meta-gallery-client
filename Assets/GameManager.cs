using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
class Art
{
    public int idx;
    public string url;
    public bool show;
    public bool frame;
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
class Data
{
    public Art[] art;
    public LightSetting[] light;
}

public class GameManager : MonoBehaviour
{
    Data data;
    public string serverUrl = "https://gallery.darae.dev/";

    public Texture2D[] test0;
    public Texture2D[] test1;
    public Texture2D[] test2;
    private LightmapData[] lightmapData;

    Texture2D[] lightmap;

    // Start is called before the first frame update
    void Start()
    {
        lightmap = test0;
        //setupLightMaps();
        StartCoroutine(GetRequest(this.serverUrl + "data"));
    }

    // Update is called once per frame
    void Update()
    {
        
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
            //ren.material.SetColor("_EmissionColor", color);
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
                arts[idx].GetComponent<GetImage>().url = serverUrl + "images/" + data.art[i].url;
                arts[idx].GetComponent<GetImage>().isFrame = data.art[i].frame;
                arts[idx].GetComponent<GetImage>().LoadImage();
            }
        }
    }
}
