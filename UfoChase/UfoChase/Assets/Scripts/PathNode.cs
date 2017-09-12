using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// part of the path for the ufo
public class PathNode : MonoBehaviour {
    [SerializeField]
    private float waitTime = 0.0f;
    public float WaitTime { get { return waitTime; } }
}
