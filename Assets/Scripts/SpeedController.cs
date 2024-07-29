using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimSpeedController : MonoBehaviour
{
    private bool isFast = false;
    public float normalSpeed = 1f;
    public float fastSpeed = 7f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFast = !isFast;
            Time.timeScale = isFast ? fastSpeed : normalSpeed;
        }
    }
}
