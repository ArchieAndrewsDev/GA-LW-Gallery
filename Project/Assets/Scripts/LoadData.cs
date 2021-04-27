using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class LoadData : MonoBehaviour
{
    public static LoadData _instance;

    public List<RootPointData> loadedData;

    private string loadPath;

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

        loadPath = Application.streamingAssetsPath;

        StartLoading();
    }

    //https://answers.unity.com/questions/513582/how-to-iterate-through-every-node-in-xml.html
    private void StartLoading()
    {
        string[] filePaths = Directory.GetFiles(loadPath + "/", "*.xml");

        foreach (string path in filePaths)
        {
            var doc = new XmlDocument(); // create an empty doc
            doc.Load(path);            // load the doc, dbPath is a string

            var baseNode = doc.DocumentElement; // DocumentElement is the base/head node of all your xml document, in this case, it's Map 
            int nNodes = baseNode.ChildNodes.Count; // since it's a 'node' this means that I could access its "ChildNodes" - which is a List of "XmlNode" - since it's a list, I could get its no. elements by the Count property.

            RootPointData newRootInstance = new RootPointData();
            newRootInstance.name = baseNode.Name;
            newRootInstance.navPointData = new List<NavPointData>();

            for (int i = 0; i < nNodes; i++)
            {
                var childNode = baseNode.ChildNodes[i];
                //Debug.Log(childNode.Name); // should print layer_01, layer_02, etc
               // Debug.Log(baseNode.SelectSingleNode("Location").InnerText); // in the first loop, it will print "first", in the 2nd will print "second", etc
                // Debug.Log(childNode.SelectSingleNode("GID").InnerText); // in the first loop, will print "1"

                newRootInstance.location = StringToVector3(baseNode.SelectSingleNode("Location").InnerText);
                newRootInstance.videoURL = baseNode.SelectSingleNode("VideoURL").InnerText;

                if (childNode.Name != "Location" && childNode.Name != "VideoURL")
                {
                    NavPointData newNavPointInstance = new NavPointData();
                    newNavPointInstance.navPointName = childNode.Name;

                    foreach (XmlNode node in childNode.ChildNodes)
                    {
                        if (node.Name == "Direction")
                        {
                            newNavPointInstance.direction = StringToVector3(node.InnerText);
                        }

                        if (node.Name == "TransitionVideoURL")
                        {
                            newNavPointInstance.transitionVideoUrl = node.InnerText;
                        }
                    }

                    newRootInstance.navPointData.Add(newNavPointInstance);
                }
            }

            loadedData.Add(newRootInstance);
        }
    }

    private Vector3 StringToVector3(string stringToSplit)
    {
        string[] seperatedValues = stringToSplit.Split(' ');

        for (int i = 0; i < seperatedValues.Length; i++)
        {
            seperatedValues[i] = seperatedValues[i].Substring(2, seperatedValues[i].Length - 2);
        }


        float x = 0;
        float.TryParse(seperatedValues[1], out x);

        float y = 0;
        float.TryParse(seperatedValues[2], out y);

        float z = 0;
        float.TryParse(seperatedValues[0], out z);

        return new Vector3(x,y,z);
    }
}
