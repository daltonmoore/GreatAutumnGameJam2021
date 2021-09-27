using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Sun : MonoBehaviour
{
    public event EventHandler SunSet, SunRise;

    [SerializeField] GameObject SunStart;
    [SerializeField] GameObject SunEnd;
    [Header("Tuners")]
    [SerializeField] float sunMoveSpeed;
    [SerializeField] float nightLength;
    [SerializeField] float dayLength;
    [SerializeField] float sunStartIntensity;

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
        if(cycle == null)
        {
            cycleStartTime = Time.time;
            cycle = StartCoroutine(BeginCycle());
        }
    }

    IEnumerator BeginCycle()
    {
        transform.position = SunStart.transform.position;
        DayTime();
        while (true)
        {
            float distCovered = (Time.time - cycleStartTime) * sunMoveSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            if(transform.position != SunEnd.transform.position)
                transform.position = Vector3.Lerp(SunStart.transform.position, SunEnd.transform.position, fractionOfJourney);

            if ((Time.time - cycleStartTime) / dayLength > .80f) // 80% through day cycle so start bringing the intensity down
                light2D.intensity-=.001f;


            if (dayLength < Time.time - cycleStartTime)
                break;
            yield return new WaitForEndOfFrame();
        }
        cycle = StartCoroutine(NightCycle());
    }

    IEnumerator NightCycle()
    {
        NightTime();
        yield return new WaitForSeconds(nightLength);
        DayTime();
        cycle = null;
    }

    void DayTime()
    {
        light2D.enabled = true;
        light2D.intensity = sunStartIntensity;
        SunRise?.Invoke(this, null);
    }


    void NightTime()
    {
        light2D.enabled = false;
        SunSet?.Invoke(this, null);
    }
}
