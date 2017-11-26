using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateGrid : MonoBehaviour
{
    public int width;
    public int height;
    public Cell cellPrefab;

    private Cell[,] cells;

    private void Start()
    {
        PopulateGridFunction();
    }

    public void PopulateGridFunction()
    {
        cells = new Cell[width, height];
        Vector3 cellSpacing = cellPrefab.sprite.bounds.size;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = GameObject.Instantiate(cellPrefab, this.transform.position + new Vector3(cellSpacing.x * i, -cellSpacing.y * j), Quaternion.identity, this.transform);
                cells[i, j] = cell;
            }
        }
    }

    public Cell Cell(int i, int j)
    {
        return cells[i, j];
    }

}
