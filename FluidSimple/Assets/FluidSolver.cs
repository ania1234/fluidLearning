using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSolver : MonoBehaviour
{
    public PopulateGrid grid1;
    public int iterations = 20;

    public enum GhostBehaviour
    {
        Copy,
        MirrorHorizontal,
        MirrorVertical
    }

    private void Swap(ref float[] grid1, ref float[] grid2)
    {
        var temp = grid1;
        grid1 = grid2;
        grid2 = temp;
    }

    public void Update()
    {
        DensityStep(0.001f);
    }

    public void DensityStep(float diff)
    {
        Diffuse(diff, GhostBehaviour.Copy, grid1.N, ref grid1.colors, ref grid1.temp);
        Swap(ref grid1.colors, ref grid1.temp);
        Advect(GhostBehaviour.Copy, grid1.N, ref grid1.colors, ref grid1.temp, ref grid1.velocitiesX, ref grid1.velocitiesY);
        Swap(ref grid1.colors, ref grid1.temp);
        grid1.RefreshCells();
    }

    public static int Pos(int i, int j, int N)
    {
        return i * N + j;
    }

    public void Diffuse(float diff, GhostBehaviour b, int N, ref float[] x0, ref float[] x1)
    {
        float a = Time.deltaTime * diff * grid1.N * grid1.N;
        LinSolve(b, N, a, 1 + 4 * a, ref x0, ref x1);
    }

    /// <summary>
    /// Solve with Gauss-Seidel iterations for grid1 using grid2 as an intermediate step
    /// </summary>
    /// <param name="b">Behaviour at the boundary</param>
    /// <param name="a">Rate of aquisition from neighbours</param>
    /// <param name="c">Damping rate</param>
    public void LinSolve(GhostBehaviour b, int N, float a, float c, ref float[] x0, ref float[] x1)
    {
        for (int n = 0; n < iterations; n++)
        {
            for (int i = 1; i < N - 1; i++)
            {
                for (int j = 1; j < N - 1; j++)
                {
                    x1[Pos(i, j, N)] = (x0[Pos(i, j, N)] + a * (x1[Pos(i - 1, j, N)] +
                    x1[Pos(i + 1, j, N)] + x1[Pos(i, j - 1, N)] + x1[Pos(i, j + 1, N)])) / c;
                }
            }
            SetBoundary(b, ref x1, N);
        }
    }

    public void SetBoundary(GhostBehaviour b, ref float[] x, int N)
    {
        for (int i = 1; i < N - 1; i++)
        {
            x[Pos(0, i, N)] = b == GhostBehaviour.MirrorHorizontal ? -1 * x[Pos(1, i, N)] : x[Pos(1, i, N)];
            x[Pos(N - 1, i, N)] = b == GhostBehaviour.MirrorHorizontal ? -1 * x[Pos(N - 2, i, N)] : x[Pos(N - 2, i, N)];
            x[Pos(i, 0, N)] = b == GhostBehaviour.MirrorVertical ? -1 * x[Pos(i, 1, N)] : x[Pos(i, 1, N)];
            x[Pos(i, N - 1, N)] = b == GhostBehaviour.MirrorVertical ? -1 * x[Pos(i, N - 2, N)] : x[Pos(i, N - 2, N)];
        }
        x[Pos(0, 0, N)] = 0.5f * (x[Pos(1, 0, N)] + x[Pos(0, 1, N)]);
        x[Pos(0, N - 1, N)] = 0.5f * (x[Pos(1, N - 1, N)] + x[Pos(0, N - 2, N)]);
        x[Pos(N - 1, 0, N)] = 0.5f * (x[Pos(N - 2, 0, N)] + x[Pos(N - 1, 1, N)]);
        x[Pos(N - 1, N - 1, N)] = 0.5f * (x[Pos(N - 2, N - 1, N)] + x[Pos(N - 1, N - 2, N)]);
    }

    void Advect(GhostBehaviour b, int N, ref float[] c0, ref float[] c1, ref float[] vx, ref float[] vy)
    {

        float x, y, s0, t0, s1, t1, dt0;
        dt0 = Time.deltaTime * N;
        for (int i = 1; i < N - 1; i++)
        {
            for (int j = 1; j < N - 1; j++)
            {
                x = i - dt0 * vx[Pos(i, j, N)];
                y = j - dt0 * vy[Pos(i, j, N)];
                if (x < 0.5f) x = 0.5f;
                if (x > N + 0.5f) x = N + 0.5f;
                int i0 = (int)x;
                int i1 = i0 + 1;
                if (y < 0.5f) y = 0.5f;
                if (y > N + 0.5f) y = N + 0.5f;
                int j0 = (int)y;
                int j1 = j0 + 1;
                s1 = x - i0;
                s0 = 1 - s1;
                t1 = y - j0;
                t0 = 1 - t1;
                c1[Pos(i, j, N)] = s0 * (t0 * c0[Pos(i0, j0, N)] + t1 * c0[Pos(i0, j1, N)]) + s1 * (t0 * c0[Pos(i1, j0, N)] + t1 * c0[Pos(i1, j1, N)]);
            }
        }
        SetBoundary(b, ref c1, N);
    }

    void Project(int N, ref float[] velX, ref float[] velY, ref float[] p, ref float[] div)
    {
        for (int i = 1; i < N - 1; i++)
        {
            for (int j = 1; j < N - 1; j++)
            {
                div[Pos(i, j, N)] = -0.5f * (velX[Pos(i + 1, j, N)] - velX[Pos(i - 1, j, N)] + velY[Pos(i, j + 1, N)] - velY[Pos(i, j - 1, N)]) / N;
                p[Pos(i, j, N)] = 0;
            }
        }

        SetBoundary(GhostBehaviour.Copy, ref div, N);
        SetBoundary(GhostBehaviour.Copy, ref p, N);
        LinSolve(GhostBehaviour.Copy, N, 1, 2, ref p, ref div);
        for (int i = 1; i < N - 1; i++)
        {
            for (int j = 1; j < N - 1; j++)
            {
                velX[Pos(i, j, N)] -= 0.5f * N * (p[Pos(i + 1, j, N)] - p[Pos(i - 1, j, N)]);
                velY[Pos(i, j, N)] -= 0.5f * N * (p[Pos(i, j + 1, N)] - p[Pos(i, j - 1, N)]);
            }
        }
        SetBoundary(GhostBehaviour.MirrorHorizontal, ref velX, N);
        SetBoundary(GhostBehaviour.MirrorVertical, ref velY, N);
    }


}
