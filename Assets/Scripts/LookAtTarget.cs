using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LookAtTarget : MonoBehaviour
{
    private Transform Target { get; set; }

    private void Update()
    {
        transform.LookAt(Target, Vector3.up);
    }

    public void InitializeTarget(Vector3 position)
    {
        var target = new GameObject("LookAtTarget");
        Target = target.transform;
        Target.position = position;
    }

    public void DestroyTarget()
    {
        Destroy(Target.gameObject);
    }
}