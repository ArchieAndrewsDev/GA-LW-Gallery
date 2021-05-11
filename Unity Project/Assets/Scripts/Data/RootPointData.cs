using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootPointData
{
    public string name;
    public Vector3 location;
    public string videoURLLow, videoURLMedium, videoURLHigh;
    public GameObject marker;

    public List<NavPointData> navPointData;

    public string GetVideoURL(VideoQuality quality)
    {
        switch (quality)
        {
            case VideoQuality.High:
                return videoURLHigh;
            case VideoQuality.Medium:
                return videoURLMedium;
            case VideoQuality.Low:
                return videoURLLow;
        }

        return videoURLHigh;
    }
}