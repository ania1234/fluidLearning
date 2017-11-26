using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSolver : MonoBehaviour
{
    public PopulateGrid grid1;
    public PopulateGrid grid2;
    public int iterations = 20;

    public enum GhostBehaviour
    {
        Copy,
        MirrorHorizontal,
        MirrorVertical
    }
    public void AddSource(List<Cell> cells, float color)
    {
        foreach (var cell in cells)
        {
            cell.color = color;
        }
    }

    public void DensityStep(float diff)
    {
        Diffuse(diff, GhostBehaviour.Copy);
        Advect(GhostBehaviour.Copy);
    }
    public void Diffuse(float diff, GhostBehaviour b)
    {
        float a = Time.deltaTime * diff * grid1.width * grid1.height;
        LinSolve(b, a, 1 + 4 * a);
    }

    /// <summary>
    /// Solve with Gauss-Seidel iterations for grid1 using grid2 as an intermediate step
    /// </summary>
    /// <param name="b">Behaviour at the boundary</param>
    /// <param name="a">Rate of aquisition from neighbours</param>
    /// <param name="c">Damping rate</param>
    public void LinSolve(GhostBehaviour b, float a, float c)
    {

        for (int n = 0; n < iterations; n++)
        {
            for (int i = 1; i < grid1.width - 1; i++)
            {
                for (int j = 1; j < grid1.height - 1; j++)
                {
                    grid2.Cell(i, j).color = (grid1.Cell(i, j).color + a * (grid2.Cell(i - 1, j).color +
                    grid2.Cell(i + 1, j).color + grid2.Cell(i, j - 1).color + grid2.Cell(i, j + 1).color)) / c;
                }
            }
            SetBoundary(b, grid2, (i, j) => grid2.Cell(i, j).color, (i, j, f) => grid2.Cell(i, j).color = f);
        }


        for (int i = 0; i < grid1.width; i++)
        {
            for (int j = 0; j < grid1.height; j++)
            {
                grid1.Cell(i, j).color = grid2.Cell(i, j).color;
            }
        }
    }

    public void SetBoundary(GhostBehaviour b, PopulateGrid grid, Func<int, int, float> cell, Action<int, int, float> setCell)
    {
        for (int i = 1; i < grid.width - 1; i++)
        {
            setCell(0, i, b == GhostBehaviour.MirrorHorizontal ? -1 * cell(1, i) : cell(1, i));
            setCell(grid.width - 1, i, b == GhostBehaviour.MirrorHorizontal ? -1 * cell(grid.width - 2, i) : cell(grid.width - 2, i));
            setCell(i, 0, b == GhostBehaviour.MirrorVertical ? -1 * cell(i, 1) : cell(i, 1));
            setCell(i, grid.height - 1, b == GhostBehaviour.MirrorVertical ? -1 * cell(i, grid.height - 2) : cell(i, grid.height - 2));
        }
        setCell(0, 0, 0.5f * (cell(1, 0) + cell(0, 1)));
        setCell(0, grid.height - 1, 0.5f * (cell(1, grid.height - 1) + cell(0, grid.height - 2)));
        setCell(grid.width - 1, 0, 0.5f * (cell(grid.width - 2, 0) + cell(grid.width - 1, 1)));
        setCell(grid.width - 1, grid.height - 1, 0.5f * (cell(grid.width - 2, grid.height - 1) + cell(grid.width - 1, grid.height - 2)));
    }

    void Advect(GhostBehaviour b)
    {

        float x, y, s0, t0, s1, t1, dt0;
        dt0 = Time.deltaTime * grid1.width;
        for (int i = 1; i < grid1.width - 1; i++)
        {
            for (int j = 1; j < grid1.height - 1; j++)
            {
                x = i - dt0 * grid1.Cell(i, j).velocity.x;
                y = j - dt0 * grid1.Cell(i, j).velocity.y;
                if (x < 0.5f) x = 0.5f;
                if (x > grid1.width + 0.5f) x = grid1.width + 0.5f;
                int i0 = (int)x;
                int i1 = i0 + 1;
                if (y < 0.5f) y = 0.5f;
                if (y > grid1.height + 0.5f) y = grid1.height + 0.5f;
                int j0 = (int)y;
                int j1 = j0 + 1;
                s1 = x - i0;
                s0 = 1 - s1;
                t1 = y - j0;
                t0 = 1 - t1;
                grid2.Cell(i, j).color = s0 * (t0 * grid1.Cell(i0, j0).color + t1 * grid1.Cell(i0, j1).color) + s1 * (t0 * grid1.Cell(i1, j0).color + t1 * grid1.Cell(i1, j1).color);
            }
        }
        SetBoundary(b, grid2, (i, j) => grid2.Cell(i, j).color, (i, j, f) => grid2.Cell(i, j).color = f);
        for (int i = 0; i < grid1.width; i++)
        {
            for (int j = 0; j < grid1.height; j++)
            {
                grid1.Cell(i, j).color = grid2.Cell(i, j).color;
            }
        }
    }
}
