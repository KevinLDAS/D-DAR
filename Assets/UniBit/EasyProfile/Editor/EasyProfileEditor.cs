using UnityEditor;
using UnityEngine;
using EasyProfile;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EasyProfileEditor : MonoBehaviour
{
    [MenuItem("EasySystems/EasyProfile/Settings")]
    static void OpenSetting()
    {
        Selection.activeObject = Resources.Load("Scriptable/EasyProfileSettings");
    }

    [MenuItem("EasySystems/EasyProfile/Clear Credentials")]
    static void ClearCredentials()
    {
        PlayerPrefs.DeleteKey(Constants.LoginSaveKey);
        PlayerPrefs.DeleteKey(Constants.PasswordSaveKey);
    }

    [MenuItem("EasySystems/EasyProfile/Add Profile Manager To Scene")]
    static void AddProfileManager()
    {
        if (SceneAsset.FindObjectOfType<EasyProfileManager>() == null)
        {
            PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Managers/[EasyProfileManager]"));
        }
        if (SceneAsset.FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        MarkSceneAsDirty();
    }

    [MenuItem("EasySystems/EasyProfile/Add Simple Profile UI To Scene")]
    static void AddSimpleProfileUI()
    {
        if (SceneAsset.FindObjectOfType<SimpleSceneUI>() == null)
        {
            PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Examples/Canvas (Simple Scene UI)"));
        }
        if (SceneAsset.FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        MarkSceneAsDirty();
    }

    [MenuItem("EasySystems/EasyProfile/Setup Samples")]
    static void SetupSamples()
    {
        List<string> scenesPath = new List<string>();
        scenesPath.Add("Assets/UniBit/EasyProfile/Examples/LoginScene.unity");
        scenesPath.Add("Assets/UniBit/EasyProfile/Examples/ProfileScene.unity");
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (string _pathes in scenesPath)
        {
            editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(_pathes, true));
        }
        
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        EasyProfileSettings _setting = Resources.Load<EasyProfileSettings>("Scriptable/EasyProfileSettings");
        _setting.LoginSceneName = "LoginScene";
        _setting.ProfileSceneName = "ProfileScene";
        EditorUtility.SetDirty(_setting);
    }

    [MenuItem("EasySystems/EasyProfile/Documentation")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://archbee.io/public/TVb70-mvTMgyNONMq-Xnz/introduction");
    }

    private static void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }
}
