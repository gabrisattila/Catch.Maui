using Catch.Model.CatchEventArgs;
using Catch.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catch.Model
{
    public class GameModel
    {
        #region Fields & Props

        private List<int> POSSIBLE_SIZES = new List<int>() { 11, 15, 21 };
        private GameTable _table;
        private int _boardSize;
        private Enemy _enemy1;
        private Enemy _enemy2;
        private Player _player;
        private Boolean van_e1, van_e2, van_p;
        private int _gameTime;
        private DataAccess Da;

        public GameTable Table { get { return _table; } set { _table = value; } }
        public int BoardSize { get { return _boardSize; } set { _boardSize = value; } }
        public Enemy ENEMY1 { get { return _enemy1; } set { _enemy1 = value; } }
        public Enemy ENEMY2 { get { return _enemy2; } set { _enemy2 = value; } }
        public Player PLAYER { get { return _player; } set { _player = value; } }
        public bool E1 { get { return van_e1; } set { van_e1 = value; } }
        public bool E2 { get { return van_e2; } set { van_e2 = value; } }
        public bool P { get { return van_p; } set { van_p = value; } }
        public int Time { get { return _gameTime; } }
        public bool IsGameOver
        {
            get { return (!ENEMY1.Alive && !ENEMY2.Alive) || !PLAYER.Alive || PLAYER.Get_Caught; }
        }
        #endregion

        #region Events & Handlers

        public event EventHandler<GameOverEventArgs> GameOver = null!;

        public void OnGameOver(Ends end)
        {
            GameOver?.Invoke(this, new GameOverEventArgs(end));
        }

        #endregion

        #region Ctors
        public GameModel(int boardSize)
        {
            if (!POSSIBLE_SIZES.Contains(boardSize))
            {
                throw new ArgumentException();
            }
            _boardSize = boardSize;
            _table = new GameTable(_boardSize, new int[_boardSize, _boardSize]);

            FillWithBombs(_boardSize*_boardSize);

            _table.setP(false);
            _table.setE1(false);
            _table.setE2(false);
            PLAYER = new Player(_table.PPos[0], _table.PPos[1]);
            P = true;
            ENEMY1 = new Enemy(_table.E1Pos[0], _table.E1Pos[1], PLAYER);
            E1 = true;
            ENEMY2 = new Enemy(_table.E2Pos[0], _table.E2Pos[1], PLAYER);
            E2 = true;
            _gameTime = 0;
        }
        public GameModel(int time, GameTable table) 
        {
            _table = table;
            _boardSize = _table.M;

            PLAYER = new Player(_table.PPos[0], _table.PPos[1]);
            if (_table.E1)
            {
                ENEMY1 = new Enemy(_table.E1Pos[0], _table.E1Pos[1], PLAYER);
                ENEMY1.Alive = true;
            }
            else
            {
                ENEMY1 = new Enemy(_table.FirstBomb[0], _table.FirstBomb[1], PLAYER);
                ENEMY1.Alive = false;
            }
            if (_table.E2)
            {
                ENEMY2 = new Enemy(_table.E2Pos[0], _table.E2Pos[1], PLAYER);
                ENEMY2.Alive = true;
            }
            else
            {
                ENEMY2 = new Enemy(_table.FirstBomb[0], _table.FirstBomb[1], PLAYER);
                ENEMY2.Alive = false;
            }

            _gameTime = time;
        }

        public GameModel(DataAccess da)
        {
            Da = da;
            int[,] x = new int[2, 2];
            _table = new GameTable(1, x);
            Da = da;
            van_e1 = false;
            van_e2 = false;
            van_p = false;
            _gameTime = 0;
        }

        #endregion

        #region Methods
        private void FillWithBombs(int boardSize)
        {
            Random random = new Random();
            bool first = true;
            _table.FirstBomb = new int[2];
            foreach (int _ in Enumerable.Range(1, boardSize/3))
            {
                bool bumm = false;
                while (!bumm)
                {
                    int lehetseges_x = random.Next(1, boardSize - 1);
                    int lehetseges_y = random.Next(1, boardSize - 1);
                    if (Cont(lehetseges_x, lehetseges_y) && _table.Tábla[lehetseges_x, lehetseges_y] != 1)
                    {
                        if (first)
                        {
                            _table.Tábla[lehetseges_x, lehetseges_y] = 1;
                            _table.FirstBomb[0] = lehetseges_x; _table.FirstBomb[1] = lehetseges_y;
                            first = false;
                        }
                        else
                        {
                            _table.Tábla[lehetseges_x, lehetseges_y] = 1;
                            _table.FirstBomb[0] = lehetseges_x; _table.FirstBomb[1] = lehetseges_y;
                        }
                        bumm = true;
                    }
                }
            }
        }

        private bool Cont(int x, int y)
        {
            var boardRange = Enumerable.Range(1, BoardSize-2);
            return boardRange.Contains(x) && boardRange.Contains(y);
        }
        public void MovePlayer(int i, int j)
        {
            char c;
            int tav;
            bool t = Math.Abs(PLAYER.Pos[0] - i) >= Math.Abs(PLAYER.Pos[1] - j);
            if (PLAYER.Pos[0] == i && PLAYER.Pos[1] > j)
            {
                c = 'a';
            }
            else if (PLAYER.Pos[0] == i && PLAYER.Pos[1] < j)
            {
                c = 'd';
            }
            else if (PLAYER.Pos[1] == j && PLAYER.Pos[0] > i)
            {
                c = 'w';
            }
            else if (PLAYER.Pos[1] == j && PLAYER.Pos[0] < i)
            {
                c = 's';
            }
            else
            {
                if (t)
                {
                    if (PLAYER.Pos[0] - i < 0)
                    {
                        c = 's';
                    }
                    else
                    {
                        c = 'w';
                    }
                }
                else
                {
                    if (PLAYER.Pos[1] - j < 0)
                    {
                        c = 'd';
                    }
                    else
                    {
                        c = 'a';
                    }
                }
            }
            PLAYER.Move(c, Table);
            if (PLAYER.Get_Caught)
            {
                OnGameOver(Ends.Player_Get_Caught);
            }
            if (PLAYER.Alive == false)
            {
                OnGameOver(Ends.Player_Exploded);
            }
            if (ENEMY1.Alive == false && ENEMY2.Alive == false)
            {
                OnGameOver(Ends.Enemies_Exploded);
            }
        }

        public void Enemy1Move(int sc)
        {
            if (sc % 0.5 == 0)
            {
                ENEMY1.Move(Table, ENEMY1.
                            next(ENEMY1.
                            merreKéne(ENEMY1.Pos, PLAYER.Pos)), 0);
                if (PLAYER.Get_Caught)
                {
                    OnGameOver(Ends.Player_Get_Caught);
                }
                if (PLAYER.Alive == false)
                {
                    OnGameOver(Ends.Player_Exploded);
                }
                if (ENEMY1.Alive == false && ENEMY2.Alive == false)
                {
                    OnGameOver(Ends.Enemies_Exploded);
                }
            }
        }

        public void Enemy2Move(int sc)
        {
            if (sc % 0.5 == 0)
            {
                ENEMY2.Move(Table, ENEMY2.
                            next(ENEMY2.
                            merreKéne(ENEMY2.Pos, PLAYER.Pos)), 1);
                if (PLAYER.Get_Caught)
                {
                    OnGameOver(Ends.Player_Get_Caught);
                }
                if (PLAYER.Alive == false)
                {
                    OnGameOver(Ends.Player_Exploded);
                }
                if (ENEMY1.Alive == false && ENEMY2.Alive == false)
                {
                    OnGameOver(Ends.Enemies_Exploded);
                }
            }
        }

        public void AdvanceTime()
        {
            _gameTime++;
        }

        public GameTable Loaddef(string path)
        {
            GameTable table = Da.Load(path);
            table.setE1(table.E1);
            table.setE2(table.E2);
            table.setP(table.P);
            PLAYER = new Player(table.PPos[0], table.PPos[1]);
            P = true;
            ENEMY1 = new Enemy(table.E1Pos[0], table.E1Pos[1], PLAYER);
            E1 = true;
            ENEMY2 = new Enemy(table.E2Pos[0], table.E2Pos[1], PLAYER);
            E2 = true;

            this.Table = table;
            return table;
        }

        #endregion
    }
}
