using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON; // Make sure to include the SimpleJSON library or another JSON parsing library

public class FetchData : MonoBehaviour
{
    private float timeBetweenRequests = 1.0f; // Time in seconds between requests
    private string url = "https://whispering-tundra-60957-a9fec9593e7f.herokuapp.com/getNewInteractions";

    void Start()
    {
        StartCoroutine(FetchEverySecond());
    }

    private IEnumerator FetchEverySecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenRequests);
            StartCoroutine(FetchDataFromServer());
        }
    }

    private IEnumerator FetchDataFromServer()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                HandleResponse(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Web request failed: " + webRequest.error);
            }
        }
    }

    private void HandleResponse(string json)
    {
        var data = JSON.Parse(json);
        if (data.Count > 0)
        {
            string sceneType = data[0]["type"];
            SceneManager.LoadScene(sceneType);
        }
        //else
        //{
        //    Debug.Log("Received empty list, no action taken.");
        //}
    }
}
