using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extension
{
    public static bool Contain(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
}
