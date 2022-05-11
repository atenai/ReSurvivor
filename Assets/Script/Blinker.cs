using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    public float onDuration = 0.5f;
    public float offDuration = 0.1f;

    private List<Renderer> _renderers = new List<Renderer>();

    private void Awake()
    {
        _renderers.AddRange(GetComponents<Renderer>());
        _renderers.AddRange(GetComponentsInChildren<Renderer>());
    }
    // Start is called before the first frame update
    private void Start()
    {
        _ = StartCoroutine(Blink(onDuration, offDuration, () => true));  
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private IEnumerator Blink(float on, float off, Func<bool> until)
    {
        while (until())
        {
            foreach (var r in _renderers)
            {
                r.enabled = true;
            }
            yield return new WaitForSeconds(on);
            foreach (var r in _renderers)
            {
                r.enabled = false;
            }
            yield return new WaitForSeconds(off);
        }
    }
}
