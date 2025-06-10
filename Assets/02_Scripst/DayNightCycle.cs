using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f,24f)]public float hour = 12;
    public Transform sun;

    private float sunX;
    public float durationDayMinutes=1;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotationSun();
    }

    void RotationSun() 
    {
        sunX = 15 * hour;

        sun.localEulerAngles = new Vector3(sunX, 0, 0);
    }
}
