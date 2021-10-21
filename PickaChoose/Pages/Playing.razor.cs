using Microsoft.AspNetCore.Components;
using PickaChoose.Data;
using System;

namespace PickaChoose.Pages
{
    public partial class Playing : ComponentBase
    {
        const int MaxHeight = 11;
        const int MaxWidth = 18;
        const int TotalPokemon = 37;

        Point picked = new(-1, -1);
        Random random = new();
        int[,] pokemon = new int[MaxHeight, MaxWidth];
        int[] pokemonCreated = new int[TotalPokemon];

        protected override void OnInitialized()
        {
            for (int i = 1; i < MaxHeight - 1; i++)
            {
                for (int j = 1; j < MaxWidth - 1; j++)
                {
                    pokemon[i, j] = GetRandomPokemon();
                }
            }
        }

        private int GetRandomPokemon()
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
                return;
            }

            if (picked.HasValue())
            {
                FindPathFromStartToEnd(picked, new Point(x, y));
                picked = new();
                return;
            }

            picked = new(x, y);
        }

        private bool IsThePokemonPicked(int x, int y)
        {
            return picked.X == x && picked.Y == y;
        }

        private void FindPathFromStartToEnd(Point start, Point end)
        {
            if (pokemon[start.X, start.Y] != pokemon[end.X, end.Y])
            {
                return;
            }

            int keepStartPokemon = pokemon[start.X, start.Y];
            int keepEndPokemon = pokemon[end.X, end.Y];
            pokemon[start.X, start.Y] = 0;
            pokemon[end.X, end.Y] = 0;

            for (int i = 0; i < MaxHeight; i++)
            {
                if (!IsExistPathX(i, Math.Min(start.Y, end.Y), Math.Max(start.Y, end.Y))
                    || !IsExistPathY(start.Y, Math.Min(start.X, i), Math.Max(start.X, i))
                    || !IsExistPathY(end.Y, Math.Min(end.X, i), Math.Max(end.X, i)))
                {
                    continue;
                }

                return;
            }

            for(int i = 0; i < MaxWidth; i++)
            {
                if (!IsExistPathY(i, Math.Min(start.X, end.X), Math.Max(start.X, end.X))
                    || !IsExistPathX(start.X, Math.Min(start.Y, i), Math.Max(start.Y, i))
                    || !IsExistPathX(end.X, Math.Min(end.Y, i), Math.Max(end.Y, i)))
                {
                    continue;
                }

                return;
            }

            pokemon[start.X, start.Y] = keepStartPokemon;
            pokemon[end.X, end.Y] = keepEndPokemon;
        }

        private bool IsExistPathX(int x, int start, int end)
        {
            if (pokemon[x, start] != 0)
            {
                return false;
            }

            if (start == end)
            {
                return true;
            }

            return IsExistPathX(x, start + 1, end);
        }

        private bool IsExistPathY(int y, int start, int end)
        {
            if (pokemon[start, y] != 0)
            {
                return false;
            }

            if (start == end)
            {
                return true;
            }

            return IsExistPathY(y, start + 1, end);
        }

        private static bool IsInMap(int x, int y)
        {
            return x > 0 && x < MaxHeight - 1 && y > 0 && y < MaxWidth - 1;
        }
    }
}
