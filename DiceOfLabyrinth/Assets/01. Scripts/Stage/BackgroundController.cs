using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public static BackgroundController Instance { get; private set; }
    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetBackground(Sprite sprite)
    {
        if (meshRenderer != null && sprite != null)
            meshRenderer.material.mainTexture = sprite.texture;
    }
}