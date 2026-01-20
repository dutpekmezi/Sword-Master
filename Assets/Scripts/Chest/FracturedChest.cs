using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FracturedChest : MonoBehaviour
{
    public Transform[] cellTransforms;
    public void Init()
    {
        int i = 0;

        foreach (Transform transform in transform.GetComponentsInChildren<Transform>())
        {
            if (i < cellTransforms.Length) transform.position = cellTransforms[i].position;
            i++;
        }
    }
}
