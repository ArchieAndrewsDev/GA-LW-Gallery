using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootPointData
{
    public string name;
    public Vector3 location;
    public string videoURL;
    public GameObject marker;

    public List<NavPointData> navPointData;
}