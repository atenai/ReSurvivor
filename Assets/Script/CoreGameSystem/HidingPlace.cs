using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class HidingPlace : MonoBehaviour, IHidable
{
    [SerializeField]
    private KeyCode hidekey = KeyCode.None;

    private Gateway[] _gateways;
    private GameObject _hidedObject;
    private Dictionary<KeyCode, Gateway> _keyExitMap = new Dictionary<KeyCode, Gateway>();
    public bool IsAccessable(GameObject gameObject)
    {
        foreach(var viewable in GetAllViewables())
        {
            if (viewable.FoundAnyTarget())
            {
                return false;
            }
        }
        foreach(var gateway in _gateways)
        {
            if (gateway.IsAccessable(gameObject))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsOccupied() => _hidedObject != null;

    public Transform GetExit(KeyCode key)
    {
        return _keyExitMap[key].transform;
    }

    public void GoOut(Transform exit)
    {
        if (_hidedObject == null)
        {
            throw new InvalidOperationException();
        }
        ChangePositionX(_hidedObject.transform, exit.position.x);
        _hidedObject.SetActive(true);
        _hidedObject = null;

        FlipArrowAll();
    }

    public void Hide(GameObject gameObject)
    {
        if (!IsAccessable(gameObject))
        {
            throw new InvalidOperationException();
        }
        gameObject.SetActive(false);
        ChangePositionX(gameObject.transform, transform.position.x);
        _hidedObject = gameObject;

        FlipArrowAll();
    }

    public KeyCode HideKey()
    {
        return hidekey;
    }

    public bool ValidKey(KeyCode key)
    {
        return _keyExitMap.ContainsKey(key);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _gateways = GetComponentsInChildren<Gateway>();
        foreach (var gateway in _gateways)
        {
            _keyExitMap.Add(gateway.exitKey, gateway);
        }
    }

    private List<IViewable> GetAllViewables()
    {
        var viewables = new List<IViewable>();
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            viewables.AddRange(gameObject.GetComponentsInChildren<IViewable>());
        }
        return viewables;
    }

    // Update is called once per frame
    private void OnGUI()
    {
        if (_hidedObject != null && Event.current.type == EventType.KeyDown)
        {
            var key = Event.current.keyCode;
            if (ValidKey(key))
            {
                GoOut(GetExit(key));
            }
        }
    }

    private void ChangePositionX(Transform transform, float x)
    {
        var originPos = transform.position;
        transform.position = new Vector3(x, originPos.y, originPos.z);
    }

    private void FlipArrowAll()
    {
        foreach (var gateway in _gateways)
        {
            gateway.FlipArrow();
        }
    }
}
