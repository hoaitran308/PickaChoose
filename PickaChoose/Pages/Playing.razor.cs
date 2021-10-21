using Microsoft.AspNetCore.Components;
using PickaChoose.Data;
using System;
using System.Collections.Generic;

namespace PickaChoose.Pages
{
    public partial class Playing : ComponentBase
    {
        Point[] roadMap = {
            new Point(-1, 0),
            new Point(0, 1),
            new Point(1, 0),
            new Point(0, -1)
        };

        Point picked = new(-1, -1);

        const int Height = 11;
        const int Width = 18;
        const int TotalPokemon = 37;

        Random random = new();
        int[,] pokemon = new int[Height, Width];
        int[] pokemonCreated = new int[TotalPokemon];

        protected override void OnInitialized()
        {
            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    pokemon[i, j] = GetPokemonId();
                }
            }
        }

        private int GetPokemonId()
        {
            int pokemonId;

            do
            {
                pokemonId = random.Next(1, TotalPokemon);
            }
            while (pokemonCreated[pokemonId] == 4);

            pokemonCreated[pokemonId]++;

            return pokemonId;
        }

        private void PickThePokemon(int x, int y)
        {
            if (picked.X == x && picked.Y == y)
            {
                picked = new();
            }

            if (picked.HasValue())
            {
                IsExistPath(picked, new Point(x, y));
                picked = new();
                return;
            }

            picked.X = x;
            picked.Y = y;
        }

        private bool IsThePokemonPicked(int x, int y)
        {
            return picked.X == x && picked.Y == y;
        }

        private bool IsExistPath(Point start, Point end)
        {
            if (pokemon[start.X, start.Y] != pokemon[end.X, end.Y])
            {
                return false;
            }

            int keepStartPokemon = pokemon[start.X, start.Y];
            int keepEndPokemon = pokemon[end.X, end.Y];
            pokemon[start.X, start.Y] = 0;
            pokemon[end.X, end.Y] = 0;

            for (int i = 0; i < Height; i++)
            {
                if (!IsExistPathX(i, Math.Min(start.Y, end.Y), Math.Max(start.Y, end.Y))
                    || !IsExistPathY(start.Y, Math.Min(start.X, i), Math.Max(start.X, i))
                    || !IsExistPathY(end.Y, Math.Min(end.X, i), Math.Max(end.X, i)))
                {
                    continue;
                }

                return true;
            }

            for(int i = 0; i < Width; i++)
            {
                if (!IsExistPathY(i, Math.Min(start.X, end.X), Math.Max(start.X, end.X))
                    || !IsExistPathX(start.X, Math.Min(start.Y, i), Math.Max(start.Y, i))
                    || !IsExistPathX(end.X, Math.Min(end.Y, i), Math.Max(end.Y, i)))
                {
                    continue;
                }

                return true;
            }

            pokemon[start.X, start.Y] = keepStartPokemon;
            pokemon[end.X, end.Y] = keepEndPokemon;

            return false;
        }

        private bool IsExistPathX(int x, int start, int end)
        {
            if (start == end)
            {
                return true;
            }

            if (pokemon[x, start] != 0)
            {
                return false;
            }

            return IsExistPathX(x, start + 1, end);
        }

        private bool IsExistPathY(int y, int start, int end)
        {
            if (start == end)
            {
                return true;
            }

            if (pokemon[start, y] != 0)
            {
                return false;
            }

            return IsExistPathY(y, start + 1, end);
        }

        private void HandleCorrectPick(Queue<Point> points)
        {
            while (points.Count > 0)
            {
                Point point = points.Dequeue();
                pokemon[point.X, point.Y] = 0;
            }
        }

        private static bool IsInMap(int x, int y)
        {
            return x > 0 && x < Height - 1 && y > 0 && y < Width - 1;
        }
    }
}
