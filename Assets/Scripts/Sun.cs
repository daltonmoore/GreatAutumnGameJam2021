using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Sun : MonoBehaviour
{
    public event EventHandler OnSunSet;

    [SerializeField] GameObject SunStart;
    [SerializeField] GameObject SunEnd;
    [Header("Tuners")]
    [SerializeField] bool onlyNight;
    [SerializeField] float cycleSpeed;
    [SerializeField] float nightLength;

    Light2D light2D;
    Coroutine cycle;
    float cycleStartTime = 0;
    float journeyLength;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        journeyLength = Vector3.Distance(SunStart.transform.position, SunEnd.transform.position);
    }

    private void Update()
    {
        if(!onlyNight && cycle == null)
        {
            cycleStartTime = Time.time;
            cycle = StartCoroutine(BeginCycle());
        }
        else if(onlyNight && cycle == null)
        {
            cycleStartTime = Time.time;
            cycle = StartCoroutine(NightOnlyCycle());
        }
    }

    IEnumerator BeginCycle()
    {
        transform.position = SunStart.transform.position;
        while (true)
        {
            float distCovered = (Time.time - cycleStartTime) * cycleSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(SunStart.transform.position, SunEnd.transform.position, fractionOfJourney);
            if (transform.position == SunEnd.transform.position)
                break;
            yield return new WaitForEndOfFrame();
        }
        NightTime();
        
        light2D.enabled = true;
        cycle = null;
    }

    IEnumerator NightOnlyCycle()
    {
        NightTime();
        yield return new WaitForSeconds(nightLength);
    }


    void NightTime()
    {
        light2D.enabled = false;
        OnSunSet?.Invoke(this, null);
    }
}
