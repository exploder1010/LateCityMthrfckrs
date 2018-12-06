using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class LevelBlock : MonoBehaviour {
    [SerializeField]
    [Tooltip("Multiple of original blocksize")]
    float blockSize;
    [SerializeField]
    [Tooltip("Can modify Pitch in Editor")]
    bool pitch;

    public float BlockSize { get { return blockSize * 30; } }
    public bool Pitch { get { return pitch; } }
}
#endif