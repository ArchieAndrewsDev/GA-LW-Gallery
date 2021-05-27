using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootPointData
{
    public string name;
    public Vector3 location;
    public string videoURLMedium, videoURLHigh;
    public GameObject marker;
    public string interactionURL;
    public string metaTitle;
    public string metaText;

    public List<NavPointData> navPointData;

    public string GetVideoURL(VideoQuality quality)
    {
        switch (quality)
        {
            case VideoQuality.High:
                return videoURLHigh;
            case VideoQuality.Medium:
                return videoURLMedium;
        }

        return videoURLHigh;
    }
}