using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPen : MonoBehaviour
{

    public float radius;
    public float deltaPosMultiplier;
    public Color color;
    public PenType penType;
    public static ColorPen instance;

    public enum PenType
    {
        Coloring,
        VelocityAddition
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;


    }
}
