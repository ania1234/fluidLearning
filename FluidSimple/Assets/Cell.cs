using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float _color;
    public float color { get { return _color; } set { text.text = value.ToString(); sprite.color = new Color(0, 0, value); _color = value; } }
    public float velX { get { return velocity.x; } set { velocity.x = value; } }
    public float velY { get { return velocity.y; } set { velocity.y = value; } }
    public Vector2 velocity;
    public SpriteRenderer sprite;
    public TextMesh text;

    private void Awake()
    {

    }
}
