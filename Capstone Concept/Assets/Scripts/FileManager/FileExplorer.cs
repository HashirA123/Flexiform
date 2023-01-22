using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif
using UnityEngine.Networking;
using System.IO;
using System.Text;
using Dummiesman;
using TMPro;
using System;

public class FileExplorer: MonoBehaviour
{
    private RawImage rawImage;
    private Transform handModel;
    //public GameObject text;

    public void OpenFileBrowser()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "Image files | *.obj";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            StartCoroutine(LoadFile1(path));
        });
#endif
    }

    IEnumerator LoadImage(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        {
            yield return uwr.SendWebRequest();

            if(uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var uwrTexture = DownloadHandlerTexture.GetContent(uwr);
                rawImage.texture = uwrTexture;
            }
        }
    }

    IEnumerator LoadFile1(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var textStream = new MemoryStream(Encoding.UTF8.GetBytes(uwr.downloadHandler.text));
                var loadedObject = new OBJLoader().Load(textStream);
                loadedObject.name = "hand";
                loadedObject.transform.position = Vector3.zero;
                handModel = loadedObject.transform.GetChild(0);
                
                //verts[0] += new Vector3(0, 0.5f, 0);
                //mesh.vertices = verts;
                //mesh.RecalculateBounds();
            }
        }
    }

    public void EditVertex(Slider slider)
    {
        var mesh = handModel.GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        verts[4].y = slider.value;
        mesh.vertices = verts;
        mesh.RecalculateBounds();
    }
}
