﻿using Catch.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catch.Model
{
    public class Player
    {
        /*
         VALUE: 3
         */

        #region Fields & Props

        private int[] pos;
        private bool _isAlive;
        private bool _caught;
        private bool _step;

        public int[] Pos { get { return pos; } set { pos = value; } }
        public bool Alive { get { return _isAlive; } set { _isAlive = value; } }
        public bool Get_Caught { get { return _caught; } set { _caught = value; } }
        public bool Step { get { return _step; } set { _step = value; } }

        #endregion

        #region Ctor

        public Player(int x, int y)
        {
            Pos = new int[] { x, y };
            Get_Caught = false;
            Alive = true;
            Step = false;
        }

        #endregion

        #region Methods
        public void Move(char c, GameTable t)
        {
            if (Get_Caught == false && Alive == true)
            {
                if (c == 'w')
                {
                    if (Pos[0] - 1 >= 0)
                    {
                        if (t.Tábla[Pos[0] - 1, Pos[1]] == 1 ||
                            t.Tábla[Pos[0] - 1, Pos[1]] == 6 ||
                            t.Tábla[Pos[0] - 1, Pos[1]] == 7) // Akna
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Alive = false;

                        }
                        else
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Pos[0]--;
                            t.Tábla[Pos[0], Pos[1]] = 3;
                            Step = false;
                        }
                    }
                }
                else if (c == 'a')
                {
                    if (Pos[1] - 1 >= 0)
                    {
                        if (t.Tábla[Pos[0], Pos[1] - 1] == 1 ||
                            t.Tábla[Pos[0], Pos[1] - 1] == 6 ||
                            t.Tábla[Pos[0], Pos[1] - 1] == 7)
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Alive = false;
                        }
                        else
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Pos[1]--;
                            t.Tábla[Pos[0], Pos[1]] = 3;
                            Step = false;
                        }
                    }
                }
                else if (c == 's')
                {
                    if (Pos[0] + 1 < t.M)
                    {
                        if (t.Tábla[Pos[0] + 1, Pos[1]] == 1 ||
                            t.Tábla[Pos[0] + 1, Pos[1]] == 6 ||
                            t.Tábla[Pos[0] + 1, Pos[1]] == 7)
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Alive = false;
                        }
                        else
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Pos[0]++;
                            t.Tábla[Pos[0], Pos[1]] = 3;
                            Step = false;
                        }
                    }
                }
                else if (c == 'd')
                {
                    if (Pos[1] + 1 < t.M)
                    {
                        if (t.Tábla[Pos[0], Pos[1] + 1] == 1 ||
                            t.Tábla[Pos[0], Pos[1] + 1] == 6 ||
                            t.Tábla[Pos[0], Pos[1] + 1] == 7)
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Alive = false;

                        }
                        else
                        {
                            t.Tábla[Pos[0], Pos[1]] = 0;
                            Step = true;
                            Pos[1]++;
                            t.Tábla[Pos[0], Pos[1]] = 3;
                            Step = false;
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
        #endregion
    }
}
