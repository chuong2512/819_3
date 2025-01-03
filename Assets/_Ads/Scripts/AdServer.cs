using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class AdServer : MonoBehaviour
{
    [SerializeField] [TextArea(2, 6)] private string jsonFileURL;

    [HideInInspector] public static bool adUnitsLoaded = false;

    // wrapper for json data:
    public struct AdUnits
    {
        public string AppID;
        public string BannerID;
        public string InterstitialID;
        public string RewardID;
    }

    void Start()
    {
        if (!adUnitsLoaded)
        {
            StopAllCoroutines();
            StartCoroutine(GetData(jsonFileURL));
        }
    }

    IEnumerator GetData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        // if (!request.isNetworkError && !request.isHttpError) // obsolete
        if(request.result == UnityWebRequest.Result.Success)
        {
            AdUnits adUnits = JsonUtility.FromJson<AdUnits>(request.downloadHandler.text);

            ServerAdUnits.AppId = adUnits.AppID;
            ServerAdUnits.BannerId = adUnits.BannerID;
            ServerAdUnits.InterstitialId = adUnits.InterstitialID;   
            ServerAdUnits.RewardId = adUnits.RewardID;

            adUnitsLoaded = true;

            Debug.Log("<color=green>[AdmobServer] Ad units loaded successfully from server.</color>");
        }
        else
        {
            Debug.LogError("[AdmobServer] Error: " + request.error);
        }

        request.Dispose();
    }


}
