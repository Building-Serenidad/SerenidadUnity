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

    IEnumerator Start()
    {
        while (true)
        {
            yield return StartCoroutine(FetchMessages());
            yield return new WaitForSeconds(1f); // Fetch messages every second
        }
    }

    IEnumerator FetchMessages()
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
    }

    void UpdateMessagesDisplay(string jsonData)
    {
        var parsedData = JSON.Parse(jsonData);
        string displayText = "";

        if (parsedData != null && parsedData.IsArray)
        {
            foreach (JSONNode messageNode in parsedData)
            {
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
