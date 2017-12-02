using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float _color;
    public float color { get { return _color; } set { text.text = value.ToString(); sprite.color = new Color(0, 0, value); _color = value; } }
    public float velX { get { return velocity.x; } set { velocity.x = value; } }
    public float velY { get { return velocity.y; } set { velocity.y = value; } }
    public int i;
    public int j;
    public PopulateGrid grid;
    public Vector2 velocity;
    public SpriteRenderer sprite;
    public TextMesh text;

    private static Material lineMaterial;

    public void UpdateColor()
    {
        int pos = FluidSolver.Pos(i, j, grid.N);
        grid.colors[pos] = color;
    }

    public void UpdateVelocity()
    {
        int pos = FluidSolver.Pos(i, j, grid.N);
        grid.velocitiesX[pos] = velX;
        grid.velocitiesY[pos] = velY;
    }

    private static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void OnRenderObject()
    {
        if (ColorPen.instance != null && ColorPen.instance.penType == ColorPen.PenType.VelocityAddition)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);

            // Vertex colors change from red to green
            GL.Color(Color.red);
            //GL.Color(new Color(1, 0, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(velocity.x, velocity.y, 0);

            GL.End();
            GL.PopMatrix();
        }
    }
}