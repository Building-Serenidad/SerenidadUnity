using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON; // Ensure JSON parsing library is included

public class MessageReceiver : MonoBehaviour
{
    public TextMeshPro messagesDisplay; // Ensure it's UGUI if using Canvas
    public List<AudioClip> notificationSounds; // List of AudioClip to choose from
    public AudioSource audioSource; // AudioSource component to play the clip

    private string lastData;

    void Start()
    {
        StartCoroutine(FetchMessages());
    }

    IEnumerator FetchMessages()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://www.deet.live/getMessages");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string data = request.downloadHandler.text;

                if (data != lastData)
                {
                    lastData = data;
                    UpdateMessagesDisplay(data);
                    PlayNotificationSound();
                }
            }
            else
            {
                Debug.LogError("Error fetching messages: " + request.error);
            }

            yield return new WaitForSeconds(1f); // Fetch messages every second
        }
    }

    void UpdateMessagesDisplay(string jsonData)
    {
        var parsedData = JSON.Parse(jsonData);
        string displayText = "";

        if (parsedData != null && parsedData.IsArray)
        {
            for (int i = parsedData.Count - 1; i >= 0; i--)
            {
                JSONNode messageNode = parsedData[i];
                string sender = messageNode["sender"];
                string message = messageNode["message"];
                displayText += sender + ": " + message + "\n";
            }
        }

        messagesDisplay.text = displayText;
    }

    void PlayNotificationSound()
    {
        if (notificationSounds != null && notificationSounds.Count > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, notificationSounds.Count);
            audioSource.PlayOneShot(notificationSounds[randomIndex]);
        }
    }
}
