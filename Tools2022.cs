﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;


public class Tools2022 : MonoBehaviour
{
    private static Tools2022 _instance;
    public static Tools2022 Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<Tools2022>();
            }
            return _instance;
        }
    }

    private string _input = "";
    public string Input { get { return _input; } }

    public IEnumerator GetInput(int day)
    { 
        yield return GetInput(day.ToString());
    }

    private bool _isProcessing = false;
    public bool IsProcessing { get { return _isProcessing; } }

    private void Awake()
    {
        _isProcessing = false;
    }

    public IEnumerator GetInput(string day)
    {
        _isProcessing = true;
        _input = "";
        string uri = "https://adventofcode.com/2022/day/" + day.ToString() + "/input";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Cookie", "session=" + EasyAccessValues.CookieSession);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                _input = webRequest.downloadHandler.text.TrimEnd('\n');
            }
        }
        _isProcessing = false;
    }

    public IEnumerator GetTestInput(string uri)
    {
        _isProcessing = true;
        _input = "";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                Debug.LogError("Test input : Error: " + webRequest.error);
            else
            {
                _input = webRequest.downloadHandler.text.TrimEnd('\n');
                Debug.Log("Test input received: " + webRequest.downloadHandler.text);
            }
        }
        _isProcessing = false;
    }

    public IEnumerator GetLeaderoard()
    {
        _input = "";
        string uri = "https://adventofcode.com/2022/leaderboard/private/view/" + EasyAccessValues.LeaderboardId + ".json";


        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Cookie", "session=" + EasyAccessValues.CookieSession);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                _input = webRequest.downloadHandler.text.TrimEnd('\n');

                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}