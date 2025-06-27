using UnityEditor;
using UnityEngine;

public class SceneLoadButton : MonoBehaviour
{
    [SerializeField] private SceneAsset sceneAsset;

    public void LoadScene()
    {
        if (sceneAsset == null)
        {
            return;
        }

        SceneManagerEx.Instance.LoadScene(sceneAsset.name);
    }
}