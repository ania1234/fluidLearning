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
    public SpriteRenderer sprite;
    private Vector2 prevMousePosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public enum PenType
    {
        Coloring,
        VelocityAddition
    }

    private void Update()
    {
        this.sprite.size = new Vector2(radius * 2, radius * 2);
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        pos.z = 0;
        this.transform.position = pos;

        if (Input.GetMouseButton(0))
        {
            var overlaped = Physics2D.OverlapCircleAll(new Vector2(pos.x, pos.y), radius);

            foreach (var obj in overlaped)
            {
                Cell c = obj.GetComponent<Cell>();
                if (c != null)
                {
                    switch (penType)
                    {
                        case PenType.Coloring:
                            c.color = this.color.b;
                            c.UpdateColor();
                            break;

                        case PenType.VelocityAddition:
                            c.velocity += (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - prevMousePosition) * deltaPosMultiplier;
                            c.UpdateVelocity();
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        prevMousePosition = Input.mousePosition;
    }
}