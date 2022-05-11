using Model;
using UnityEngine;

public class Gateway : MonoBehaviour
{
    [Range(0, -20)]
    public float accessRangeLower;
    [Range(0, 20)]
    public float accessRangeUpper;
    public KeyCode exitKey;
    private Transform _arrow;
    private IHidable _hidable;
    private GameObject _player;

    private void Start()
    {
        _arrow = transform.Find("Arrow");
        _hidable = GetComponentInParent<IHidable>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        _arrow.gameObject.SetActive((IsAccessable(_player) && _hidable.IsAccessable(_player)) || _hidable.IsOccupied());
    }

    public bool IsAccessable(GameObject gameObject)
    {
        var x = gameObject.transform.position.x;
        var lower = transform.position.x + accessRangeLower;
        var upper = transform.position.x + accessRangeUpper;
        return x >= lower && x <= upper;
    }

    public void FlipArrow()
    {
        _arrow.Rotate(new Vector3(0, 0, 180));
    }
}
