using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateGrid : MonoBehaviour
{
    public int N;
    public Cell cellPrefab;

    public float[] colors;
    public float[] velocitiesX;
    public float[] velocitiesY;
    public float[] temp;
    public Cell[] cells;

    private void Start()
    {
        PopulateGridFunction();
    }

    public void PopulateGridFunction()
    {
        cells = new Cell[N * N];
        velocitiesX = new float[N * N];
        velocitiesY = new float[N * N];
        colors = new float[N * N];
        temp = new float[N * N];
        Vector3 cellSpacing = cellPrefab.sprite.bounds.size;
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                var cell = GameObject.Instantiate(cellPrefab, this.transform.position + new Vector3(cellSpacing.x * i, -cellSpacing.y * j), Quaternion.identity, this.transform);
                cell.i = i;
                cell.j = j;
                cell.grid = this;
                cells[FluidSolver.Pos(i, j, N)] = cell;
            }
        }
    }

    public void RefreshCells()
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                int pos = FluidSolver.Pos(i, j, N);
                cells[pos].color = colors[pos];
                cells[pos].velX = velocitiesX[pos];
                cells[pos].velY = velocitiesY[pos];
            }
        }
    }

}
