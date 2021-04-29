using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WorldManager : MonoBehaviour
{
    public static WorldManager _instance;

    public GameObject navMarkerPrefab;
    public GameObject userObject;
    public GameObject videoSphere;
    public VideoPlayer videoPlayer;
    public float navAngle = 25;
    public float minMarkerSize = 2, maxMarkerSize = 4;

    private int activeId = -1, readyNavPointId = -1;
    private List<int> connectedNavPointIds = new List<int>();
    private Vector3 scaleVelocity;

    [HideInInspector]
    public List<RootPointData> loadedData = new List<RootPointData>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        BeginWorldCreation();
    }

    private void BeginWorldCreation()
    {
        loadedData.Clear();
        LoadData._instance.StartLoading();

        for (int i = 0; i < loadedData.Count; i++)
        {
            CreateNavMarker(loadedData[i]);
        }

        MoveTo(0);
    }

    private void MoveTo(int id)
    {
        if (id >= loadedData.Count || activeId == id)
            return;

        //Store the active id
        activeId = id;

        //Remove the conncted ids from the last move
        connectedNavPointIds.Clear();

        //Move the user to the new point and get all the connected ids
        userObject.transform.position = loadedData[id].location;
        foreach (NavPointData navData in loadedData[id].navPointData)
        {
            connectedNavPointIds.Add(navData.id);
        }

        //Set the active sate of all nav markers by connection
        for (int i = 0; i < loadedData.Count; i++)
        {
            loadedData[i].marker.SetActive(connectedNavPointIds.Contains(i));
        }

        //Move the video sphere to the new pos
        videoSphere.transform.position = loadedData[id].location;
        videoPlayer.url = loadedData[id].videoURL;
        videoPlayer.Play();
    }

    private void Update()
    {
        CheckForReadyNavPoint();
    }

    private void CheckForReadyNavPoint()
    {
        float returnAngle = 0;
        readyNavPointId = -1;
        foreach (NavPointData navData in loadedData[activeId].navPointData)
        {
            if (CheckAngle(navData.direction, navAngle, out returnAngle))
            {
                loadedData[navData.id].marker.transform.localScale = Vector3.one * Mathf.Lerp(maxMarkerSize, minMarkerSize, returnAngle / navAngle);
                readyNavPointId = navData.id;
            }
            else
                loadedData[navData.id].marker.transform.localScale = Vector3.one * minMarkerSize;
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
    public void TryMoveToReadyNavPoint()
    {
        if (readyNavPointId == -1)
            return;

        MoveTo(readyNavPointId);
    }
}
