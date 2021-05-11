using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;

public class LoadData : MonoBehaviour
{
    public static LoadData _instance;
    public static bool dataLoaded = false;

    private string loadPath;
    private List<string> navCheckSheetPaths = new List<string>();
    private List<string> loadedXML = new List<string>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        loadPath = Application.streamingAssetsPath;
    }

    public IEnumerator GetAllNavCheckSheets()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://138.68.150.78/StreamingAssets/FindAllNavSheets.php"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            string paths = webRequest.downloadHandler.text;
            string[] navCheckNames = paths.Split(' ');

            //Use -1 to leave out the last result which is blank
            for (int i = 0; i < navCheckNames.Length - 1; i++)
            {
                navCheckSheetPaths.Add(string.Format("http://138.68.150.78/StreamingAssets/{0}", navCheckNames[i]));
            }
        }

        for (int i = 0; i < navCheckSheetPaths.Count; i++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(navCheckSheetPaths[i]))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                loadedXML.Add(webRequest.downloadHandler.text);
            }
        }

        StartLoading();
    }

    //https://answers.unity.com/questions/513582/how-to-iterate-through-every-node-in-xml.html
    public void StartLoading()
    {
        Dictionary<string, int> idList = new Dictionary<string, int>(); //We use this to cache the names and their ids for setting the nav point data id value

        int count = 0;
        foreach (string xml in loadedXML)
        {
            XmlDocument doc = new XmlDocument(); // create an empty doc
            doc.LoadXml(xml);            // load the doc, dbPath is a string

            XmlElement baseNode = doc.DocumentElement; // DocumentElement is the base/head node of all your xml document, in this case, it's Map 
            int nNodes = baseNode.ChildNodes.Count; // since it's a 'node' this means that I could access its "ChildNodes" - which is a List of "XmlNode" - since it's a list, I could get its no. elements by the Count property.

            RootPointData newRootInstance = new RootPointData();
            newRootInstance.name = baseNode.Name;
            newRootInstance.navPointData = new List<NavPointData>();

            idList.Add(newRootInstance.name, count);
            count++;

            for (int i = 0; i < nNodes; i++)
            {
                var childNode = baseNode.ChildNodes[i];

                newRootInstance.location = StringToVector3(baseNode.SelectSingleNode("Location").InnerText);
                newRootInstance.videoURLHigh = baseNode.SelectSingleNode("VideoURLHigh").InnerText;
                newRootInstance.videoURLMedium = baseNode.SelectSingleNode("VideoURLMedium").InnerText;
                newRootInstance.videoURLLow = baseNode.SelectSingleNode("VideoURLLow").InnerText;

                if (childNode.Name != "Location" && childNode.Name != "VideoURLHigh" && childNode.Name != "VideoURLMedium" && childNode.Name != "VideoURLLow")
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

            WorldManager._instance.loadedData.Add(newRootInstance);
        }

        //Set ids to the nav point data so we can access their roots faster
        for (int i = 0; i < WorldManager._instance.loadedData.Count; i++)
        {
            for (int y = 0; y < WorldManager._instance.loadedData[i].navPointData.Count; y++)
            {
                WorldManager._instance.loadedData[i].navPointData[y].id = idList[WorldManager._instance.loadedData[i].navPointData[y].navPointName];
            }
        }

        dataLoaded = true;
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

        return new Vector3(x,y,z) / 50;
    }
}
