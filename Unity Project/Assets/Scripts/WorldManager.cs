using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager _instance;

    public GameObject navMarkerPrefab;
    public GameObject userObject;
    public float navAngle = 25;
    public float minMarkerSize = 2, maxMarkerSize = 4;

    private float angleToCheck;
    private float angle;
    private List<Vector3> markerDirection = new List<Vector3>();

    [HideInInspector]
    public bool tryUse = false;
    [HideInInspector]
    public List<RootPointData> loadedData;
    [HideInInspector]
    public List<GameObject> navMarkers = new List<GameObject>();

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
        loadedData.Clear();
        LoadData._instance.StartLoading();

        navMarkers.Clear();
        for (int i = 0; i < loadedData.Count; i++)
        {
            navMarkers.Add(CreateNavMarker(loadedData[i]));
            markerDirection.Add(Vector3.zero);
        }

        if (loadedData.Count > 0)
            MoveTo(loadedData[0].name);
    }

    private void Update()
    {
        angleToCheck = navAngle * .5f;
        angle = 0;

        for (int i = 0; i < navMarkers.Count; i++)
        {
            if (!navMarkers[i].activeSelf)
                continue;

            if (CheckAngle(markerDirection[i], angleToCheck, out angle))
            {
                navMarkers[i].transform.localScale = Vector3.one * Mathf.Lerp(maxMarkerSize, minMarkerSize, angle / angleToCheck);

                if (tryUse)
                {
                    MoveTo(navMarkers[i].name);
                }
            }
            else
                navMarkers[i].transform.localScale = Vector3.one * minMarkerSize;
        }
    }

    public void MoveTo(string navPointName)
    {
        //Find the passed point and move the player there whilst grabbing the new connected nav points
        List<string> connections = new List<string>();
        for (int i = 0; i < loadedData.Count; i++)
        {
            if(loadedData[i].name == navPointName)
            {
                connections = GetConnectedNavPoints(loadedData[i]);
                userObject.transform.position = loadedData[i].location;
                break;
            }
        }

        //Enable the correct markers.
        for (int i = 0; i < loadedData.Count; i++)
        {
            if (connections.Contains(loadedData[i].name))
            {
                navMarkers[i].SetActive(true);
                markerDirection[i] = Vector3.Normalize(navMarkers[i].transform.position - userObject.transform.position);
            }
            else
            {
                navMarkers[i].SetActive(false);
                markerDirection[i] = Vector3.zero;
            }
        }
    }

    //Returns a list of all the connected nav point names
    public List<string> GetConnectedNavPoints(RootPointData rootData)
    {
        List<string> returnData = new List<string>();
        for (int i = 0; i < rootData.navPointData.Count; i++)
        {
            returnData.Add(rootData.navPointData[i].navPointName);
        }

        return returnData;
    }

    public GameObject CreateNavMarker(RootPointData rootData)
    {
        GameObject clone = Instantiate(navMarkerPrefab, rootData.location, Quaternion.identity);
        clone.name = rootData.name;

        return clone;
    }

    public bool CheckAngle(Vector3 direction, float angleToCheck, out float angle)
    {
        angle = Vector3.Angle(userObject.transform.forward, direction);
        //Debug.Log(string.Format("angle = {0}    |    nav angle = {1}", angle, angleToCheck));
        return (angle <= angleToCheck);
    }
}
