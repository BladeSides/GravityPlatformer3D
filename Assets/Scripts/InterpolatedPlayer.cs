using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatedPlayer : MonoBehaviour
{
    public Transform Player;
    private void Update()
    {
        transform.position = LerpSmooth(transform.position, Player.position, Time.deltaTime, 0.1f, 0.01f);
        transform.rotation = SlerpSmooth(transform.rotation, Player.rotation, Time.deltaTime, 0.5f, 0.01f);
    }

    public static float LerpSmooth(float a, float b, float dt, float h)
    {
        return b + (a - b) * Mathf.Pow(2, -dt / h);
    }

    public static float LerpSmooth(float a, float b, float dt, float t, float precision)
    {
        return b + (a - b) * Mathf.Pow(2, -dt / GetHalfLife(t, precision));
    }

    public static Vector3 LerpSmooth(Vector3 a, Vector3 b, float dt, float h)
    {
        return b + (a - b) * Mathf.Pow(2, -dt / h);
    }

    public static Vector3 LerpSmooth(Vector3 a, Vector3 b, float dt, float t, float precision)
    {
        return b + (a - b) * Mathf.Pow(2, -dt / GetHalfLife(t, precision));
    }

    public static Quaternion SlerpSmooth(Quaternion a, Quaternion b, float dt, float h)
    {
        return (Quaternion.Slerp(a, b, 1 - Mathf.Pow(2, -dt / h)));
    }

    public static Quaternion SlerpSmooth(Quaternion a, Quaternion b, float dt, float t, float precision)
    {
        return (Quaternion.Slerp(a, b, 1 - Mathf.Pow(2, -dt / GetHalfLife(t, precision))));
    }

    public static Quaternion LerpSmooth(Quaternion a, Quaternion b, float dt, float h)
    {
        return (Quaternion.Lerp(a, b, 1 - Mathf.Pow(2, -dt / h)));
    }

    public static Quaternion LerpSmooth(Quaternion a, Quaternion b, float dt, float t, float precision)
    {
        return (Quaternion.Lerp(a, b, 1 - Mathf.Pow(2, -dt / GetHalfLife(t, precision))));
    }

    public static float GetHalfLife(float t, float precision)
    {
        return -t / Mathf.Log(precision, 2);
    }
}
