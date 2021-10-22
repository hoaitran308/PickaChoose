using Microsoft.AspNetCore.Components;
using PickaChoose.Data;
using System;
using System.Collections.Generic;

namespace PickaChoose.Pages
{
    public partial class Playing : ComponentBase
    {
        const int MaxTime = 600;
        const int MaxHeight = 9;
        const int MaxWidth = 16;
        const int TotalPokemon = 36;

        Random random;
        Point picked;
        bool isFirstPick;
        int[,] pokemon;
        int[] pokemonCreated;

        System.Timers.Timer timer;
        int countdownTime;
        int countdownPokemon;

        bool isShowHint;
        KeyValuePair<Point, Point> hintPokemon;

        protected override void OnInitialized()
        {
            ResetGame();
        }

        private void GeneratePokemon()
        {
            int totalPokemon = countdownPokemon;

            while (totalPokemon-- > 0)
            {
                Point point = GetRandomPoint();
                pokemon[point.X, point.Y] = GetRandomPokemon();
            }
        }

        private int GetRandomPokemon()
        {
            int pokemonId;

            do
            {
                pokemonId = random.Next(1, TotalPokemon + 1);
            }
            while (pokemonCreated[pokemonId] == 0);

            pokemonCreated[pokemonId]--;

            return pokemonId;
        }

        private Point GetRandomPoint()
        {
            Point point = new();

            do
            {
                point.X = random.Next(1, MaxHeight + 1);
                point.Y = random.Next(1, MaxWidth + 1);
            }
            while (pokemon[point.X, point.Y] != 0);

            return point;
        }

        private void PickThePokemon(int x, int y)
        {
            if (isFirstPick)
            {
                ChangeStateCountdownTimer();
                isFirstPick = false;
            }

            if (picked.HasValue())
            {
                if (FindPath(picked, new Point(x, y)))
                {
                    countdownPokemon -= 2;
                    ResetHintPokemon();
                }

                if (countdownPokemon == 0)
                {
                    ChangeStateCountdownTimer();
                }

                if (!hintPokemon.Key.HasValue())
                {
                    SwapPokemon();
                }

                picked = new();
                return;
            }

            picked = new(x, y);
        }

        private bool IsThePokemonPicked(int x, int y)
        {
            return picked.IsEqual(x, y);
        }

        private bool FindPath(Point start, Point end)
        {
            if (pokemon[start.X, start.Y] != pokemon[end.X, end.Y] || start.IsEqual(end) || pokemon[start.X, start.Y] == 0)
            {
                return false;
            }

            int keepStartPokemon = pokemon[start.X, start.Y];
            int keepEndPokemon = pokemon[end.X, end.Y];
            pokemon[start.X, start.Y] = 0;
            pokemon[end.X, end.Y] = 0;

            for (int i = 0; i <= MaxHeight + 1; i++)
            {
                if (IsExistPathX(i, Math.Min(start.Y, end.Y), Math.Max(start.Y, end.Y))
                    && IsExistPathY(start.Y, Math.Min(start.X, i), Math.Max(start.X, i))
                    && IsExistPathY(end.Y, Math.Min(end.X, i), Math.Max(end.X, i)))
                {
                    return true;
                }
            }

            for (int i = 0; i <= MaxWidth + 1; i++)
            {
                if (IsExistPathY(i, Math.Min(start.X, end.X), Math.Max(start.X, end.X))
                    && IsExistPathX(start.X, Math.Min(start.Y, i), Math.Max(start.Y, i))
                    && IsExistPathX(end.X, Math.Min(end.Y, i), Math.Max(end.Y, i)))
                {
                    return true;
                }
            }

            pokemon[start.X, start.Y] = keepStartPokemon;
            pokemon[end.X, end.Y] = keepEndPokemon;

            return false;
        }

        private bool IsExistPathX(int x, int start, int end)
        {
            return pokemon[x, start] == 0 && (start == end || IsExistPathX(x, start + 1, end));
        }

        private bool IsExistPathY(int y, int start, int end)
        {
            return pokemon[start, y] == 0 && (start == end || IsExistPathY(y, start + 1, end));
        }

        private void ChangeStateCountdownTimer()
        {
            if (timer is null)
            {
                timer = new System.Timers.Timer(1000);
                timer.Elapsed += (sender, args) =>
                {
                    countdownTime--;
                    if (countdownTime <= 0)
                    {
                        ChangeStateCountdownTimer();
                    }
                    InvokeAsync(StateHasChanged);
                };
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            else
            {
                ResetGame();
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        private KeyValuePair<Point, Point> GetHintPokemon()
        {
            for (int i = 1; i <= MaxHeight; i++)
            {
                for (int j = 1; j <= MaxWidth; j++)
                {
                    for (int k = i; k <= MaxHeight; k++)
                    {
                        for (int l = 1; l <= MaxWidth; l++)
                        {
                            int keepStartPokemon = pokemon[i, j];
                            int keepEndPokemon = pokemon[k, l];
                            if (FindPath(new Point(i, j), new Point(k, l)))
                            {
                                pokemon[i, j] = keepStartPokemon;
                                pokemon[k, l] = keepEndPokemon;
                                return new(new(i, j), new(k, l));
                            }
                        }
                    }
                }
            }

            return new(new(), new());
        }

        private void CountAndResetPokemon()
        {
            countdownPokemon = 0;
            for (int i = 1; i <= MaxHeight; i++)
            {
                for (int j = 1; j <= MaxWidth; j++)
                {
                    if (pokemon[i, j] != 0)
                    {
                        pokemonCreated[pokemon[i, j]]++;
                        pokemon[i, j] = 0;
                        countdownPokemon++;
                    }
                }
            }
        }

        private void SwapPokemon()
        {
            CountAndResetPokemon();
            GeneratePokemon();
            ResetHintPokemon();
        }

        private void OrderHintPokemon()
        {
            isShowHint = true;
        }

        private void ResetHintPokemon()
        {
            hintPokemon = GetHintPokemon();
            isShowHint = false;
        }

        private void ResetGame()
        {
            if (timer is not null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            random = new();

            countdownTime = MaxTime;
            countdownPokemon = MaxHeight * MaxWidth;

            isFirstPick = true;
            picked = new();

            pokemon = new int[MaxHeight + 2, MaxWidth + 2];
            pokemonCreated = new int[TotalPokemon + 1];

            for (int i = 1; i <= TotalPokemon; i++)
            {
                pokemonCreated[i] = MaxHeight * MaxWidth / TotalPokemon;
            }

            GeneratePokemon();

            ResetHintPokemon();
        }
    }
}
