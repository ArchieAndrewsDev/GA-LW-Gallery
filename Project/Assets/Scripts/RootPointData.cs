using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RootPointData
{
    public string name;
    public Vector3 location;
    public string videoURL;

    public List<NavPointData> navPointData;
}
