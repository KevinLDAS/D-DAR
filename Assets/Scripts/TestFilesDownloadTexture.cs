using UnityEngine;
using UnityGoogleDrive;
using UnityEngine.UI;

public class TestFilesDownloadTexture : AdaptiveWindowGUI
{
    public SpriteRenderer SpriteRenderer;
    public Image img;

    private GoogleDriveFiles.DownloadTextureRequest request;
    private string fileId = string.Empty;

    protected override void OnWindowGUI(int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture file ID:", GUILayout.Width(85));
            fileId = GUILayout.TextField(fileId);
            if (GUILayout.Button("Download", GUILayout.Width(100))) DownloadTexture();
            GUILayout.EndHorizontal();
        }
    }

    private void DownloadTexture()
    {
        request = GoogleDriveFiles.DownloadTexture(fileId, true);
        request.Send().OnDone += RenderImage;
    }

    private void RenderImage(UnityGoogleDrive.Data.TextureFile textureFile)
    {
        img.sprite = Sprite.Create(textureFile.Texture,
            new Rect(0f, 0f, textureFile.Texture.width, textureFile.Texture.height),
            Vector2.one * .5f);
    }
}