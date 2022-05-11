using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSwitcher : MonoBehaviour
{
    public Material[] materials;

    private Renderer _renderer;

    public void SelectByIndex(int x)
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        _renderer.material = materials[x];
    }
}
