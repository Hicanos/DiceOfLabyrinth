using UnityEngine;

public class SceneBackButton : MonoBehaviour
{
    public void GoBackScene()
    {
        SceneManagerEx.Instance.LoadPreviousScene();
    }
}