using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using UnityEngine.UI;

public class TestFilesList : AdaptiveWindowGUI
{
    [Range(1, 1000)]
    public int ResultsPerPage = 100;

    public Image defaultImg;
    public Transform layoutParent;

    private GoogleDriveFiles.ListRequest request;
    private Dictionary<string, string> results;
    private string query = string.Empty;
    private Vector2 scrollPos;

    bool requestDone;
    WaitUntil requestFilled;

    private void Start()
    {
        requestFilled = new WaitUntil(() => requestDone = true);
        ListFiles();
    }

    Texture2D tex;
    IEnumerator GetImages()
    {
        foreach (var result in results)
        {
            GoogleDriveFiles.DownloadTextureRequest request;
            request = GoogleDriveFiles.DownloadTexture(result.Key);
            request.Send().OnDone += ImageRequested;
            yield return requestFilled;
        }
    }

    void ImageRequested(UnityGoogleDrive.Data.TextureFile textureFile)
    {
        requestDone = true;
        tex = textureFile.Texture;
    }

    protected override void OnWindowGUI(int windowId)
    {
        //if (request.IsRunning)
        //{
        //    GUILayout.Label($"Loading: {request.Progress:P2}");
        //}
        //else if (results != null)
        //{
        //    scrollPos = GUILayout.BeginScrollView(scrollPos);
        //    StartCoroutine(GetImages());
        //    //foreach (var result in results)
        //    //{
        //    //    GUILayout.Label(result.Value);
        //    //    GUILayout.BeginHorizontal();
        //    //    GUILayout.Label("ID:", GUILayout.Width(20));
        //    //    GoogleDriveFiles.DownloadTextureRequest request;
        //    //    request = GoogleDriveFiles.DownloadTexture(result.Key);
        //    //    request.Send().OnDone += 
        //    //    GUI.DrawTexture(result.)
        //    //    GUILayout.TextField(result.Key);
        //    //    GUILayout.EndHorizontal();
        //    //}
        //    //GUILayout.EndScrollView();
        //}

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("File name:", GUILayout.Width(70));
        //query = GUILayout.TextField(query);
        //if (GUILayout.Button("Search", GUILayout.Width(100))) ListFiles();
        //if (NextPageExists() && GUILayout.Button(">>", GUILayout.Width(50)))
        //    ListFiles(request.ResponseData.NextPageToken);
        //GUILayout.EndHorizontal();
    }

    private void ListFiles(string nextPageToken = null)
    {
        request = GoogleDriveFiles.List();
        request.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        request.PageSize = ResultsPerPage;
        if (!string.IsNullOrEmpty(query))
            request.Q = string.Format("name contains '{0}'", query);
        if (!string.IsNullOrEmpty(nextPageToken))
            request.PageToken = nextPageToken;
        request.Send().OnDone += BuildResults;
    }

    private void BuildResults(UnityGoogleDrive.Data.FileList fileList)
    {
        results = new Dictionary<string, string>();

        foreach (var file in fileList.Files)
        {
            var fileInfo = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
            results.Add(file.Id, fileInfo);
            if (file.MimeType.Contains("image/"))
            {
                DownloadTexture(file.Id);
            }
        }
    }

    GoogleDriveFiles.DownloadTextureRequest texReq;

    void DownloadTexture(string fileId)
    {
        texReq = GoogleDriveFiles.DownloadTexture(fileId);
        texReq.Send().OnDone += RenderImage;
    }

    void RenderImage(UnityGoogleDrive.Data.TextureFile texFile)
    {
        Image img = Instantiate(defaultImg, layoutParent);
        img.sprite = Sprite.Create(texFile.Texture,
            new Rect(0, 0, texFile.Texture.width, texFile.Texture.height),
            Vector2.one * 0.5f);
    }

    private bool NextPageExists()
    {
        return request != null &&
            request.ResponseData != null &&
            !string.IsNullOrEmpty(request.ResponseData.NextPageToken);
    }
}