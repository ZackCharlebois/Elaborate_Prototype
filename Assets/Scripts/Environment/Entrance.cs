using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    [SerializeField] private int entranceNumber = 1;

    public int GetNumber()
    {
        return entranceNumber;
    }
}
