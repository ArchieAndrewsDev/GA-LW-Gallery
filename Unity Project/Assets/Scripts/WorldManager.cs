﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WorldManager : MonoBehaviour
{
    public static WorldManager _instance;

    public GameObject navMarkerPrefab;
    public GameObject userObject;
    public GameObject videoSpherePrefab;
    public int videoPlayerPoolCount = 3;
    public float navAngle = 25;
    public float minMarkerSize = 2;
    public Fade fade;
    public VideoQuality defaultVideoQuality = VideoQuality.High;

    private List<VideoPlayer> videoPlayerPool = new List<VideoPlayer>();
    private VideoPlayer activeVideoPlayer;
    private int activeId = -1, readyNavPointId = -1;
    private List<int> connectedNavPointIds = new List<int>();
    private Vector3 scaleVelocity;
    private float volumeCache = 1;

    public List<RootPointData> loadedData = new List<RootPointData>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        SetUpVideoPlayerPool();
    }

    private void Start()
    {
        StartCoroutine(BeginWorldCreation(new WaitUntil(() => LoadData.dataLoaded == true)));
    }

    private void SetUpVideoPlayerPool()
    {
        for (int i = 0; i < videoPlayerPoolCount; i++)
        {
            AddVideoPlayerToPool();
        }
    }

    private void AddVideoPlayerToPool()
    {
        GameObject clone = Instantiate(videoSpherePrefab);
        clone.SetActive(false);
        VideoPlayer vp = clone.GetComponent<VideoPlayer>();
        videoPlayerPool.Add(vp);
        vp.SetDirectAudioVolume(0, volumeCache);
    }

    private IEnumerator BeginWorldCreation(WaitUntil wait)
    {
        loadedData.Clear();
        StartCoroutine(LoadData._instance.GetAllNavCheckSheets());

        yield return wait;

        for (int i = 0; i < loadedData.Count; i++)
        {
            CreateNavMarker(loadedData[i]);
        }

        MoveTo(0);
    }

    public void TryMoveToNavPoint(GameObject marker)
    {
        for (int i = 0; i < loadedData.Count; i++)
        {
            Debug.Log(string.Format("{0} == {1}", loadedData[i].marker.name, marker.transform.parent.name));
            if (loadedData[i].marker.name == marker.transform.parent.name)
            {
                TryMoveToNavPoint(i);
                return;
            }
        }
    }

    public void ChangeQualitySettings(VideoQuality quality)
    {
        defaultVideoQuality = quality;
        TryMoveToNavPoint(activeId);
    }

    private void MoveTo(int id)
    {
        if (id >= loadedData.Count)
        {
            fade.FadeIn();
            return;
        }

        //Store the active id
        activeId = id;

        //Remove the conncted ids from the last move
        connectedNavPointIds.Clear();

        foreach (NavPointData navData in loadedData[id].navPointData)
        {
            connectedNavPointIds.Add(navData.id);
        }

        //Set the active sate of all nav markers by connection
        for (int i = 0; i < loadedData.Count; i++)
        {
            loadedData[i].marker.SetActive(connectedNavPointIds.Contains(i));
        }

        VideoPlayer nextVideoPlayer = GetNewVideoPlayer();
        nextVideoPlayer.url = loadedData[id].GetVideoURL(defaultVideoQuality);
        nextVideoPlayer.transform.position = loadedData[id].location;        //Move the new video sphere to the new pos
        nextVideoPlayer.prepareCompleted += ClearOldVideoPlayer;
        nextVideoPlayer.Play();

        UpdateMarkerSize();

        UIManager._instance.ShowPrompt(PromptType.URL, (loadedData[id].interactionURL != ""));

        if(loadedData[id].metaText != "" || loadedData[id].metaTitle != "")
        {
            UIManager._instance.SetMetaData(loadedData[id].metaTitle, loadedData[id].metaText);
        }
        else
        {
            UIManager._instance.EnableMetaData(false);
        }
    }

    private void ClearOldVideoPlayer(VideoPlayer vp)
    {
        if(activeVideoPlayer != null)
        {
            activeVideoPlayer.Stop();
            activeVideoPlayer.gameObject.SetActive(false);
            activeVideoPlayer.prepareCompleted -= ClearOldVideoPlayer;
        }

        activeVideoPlayer = vp;
        //Move the user to the new point
        userObject.transform.position = vp.transform.position;
        fade.FadeIn();
    }

    private VideoPlayer GetNewVideoPlayer()
    {
        for (int i = 0; i < videoPlayerPool.Count; i++)
        {
            if (videoPlayerPool[i].gameObject.activeSelf == false)
            {
                videoPlayerPool[i].gameObject.SetActive(true);
                return videoPlayerPool[i];
            }
        }

        AddVideoPlayerToPool();
        return GetNewVideoPlayer();
    }

    private void UpdateMarkerSize()
    {
        float size = minMarkerSize;
        foreach (NavPointData navData in loadedData[activeId].navPointData)
        {
            size = .1f * Vector3.Distance(loadedData[navData.id].marker.transform.position, loadedData[activeId].marker.transform.position);
            size = Mathf.Clamp(size, minMarkerSize, Mathf.Infinity);
            loadedData[navData.id].marker.transform.localScale = Vector3.one * size;
        }
    }

    public GameObject CreateNavMarker(RootPointData rootData)
    {
        GameObject clone = Instantiate(navMarkerPrefab, rootData.location, Quaternion.identity);
        clone.name = rootData.name;
        rootData.marker = clone;
        return clone;
    }

    public bool CheckAngle(Vector3 direction, float angleToCheck, out float angle)
    {
        angle = Vector3.Angle(userObject.transform.forward, direction);
        //Debug.Log(string.Format("angle = {0}    |    nav angle = {1}", angle, angleToCheck));
        return (angle <= angleToCheck);
    }

    //This is mainly called from UserControl.cs
    public void TryMoveToNavPoint(int id)
    {
        if (id == -1)
            return;

        fade.FadeOut();
        fade.OnFadeOut.AddListener(() => MoveTo(id));
    }

    public void SetVideoPlayerVolume(float volume)
    {
        //Cache the volume so that we can set it for players added to the pool
        volumeCache = volume;

        for (int i = 0; i < videoPlayerPool.Count; i++)
        {
            videoPlayerPool[i].SetDirectAudioVolume(0, volume);
        }
    }

    public void RunActionOnActiveNavPoint()
    {
        if (loadedData[activeId].interactionURL == "")
            return;

        Application.OpenURL(loadedData[activeId].interactionURL);
    }
}
