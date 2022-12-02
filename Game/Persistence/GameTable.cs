using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Game.Persistence
{
    public class GameTable
    {
        //Fields
        private int _size;
        private Player[,] _fields;  // -1 attacker, 0 nothing, 1 defender
        private int _remainingSteps;
        private Player _currentPlayer;

        public int GetRemainingSteps { get { return _remainingSteps; } }
        public void MinusRemainingSteps() {
            if (_currentPlayer == Player.Attacker) --_remainingSteps;
        }

        public Player GetPlayeronxy(int x, int y) { return _fields[x, y];  }

        public int GetSize { get { return _size; } } 

        public void SetSize(int i) { _size = i; }

        public Player GetPlayer { get { return _currentPlayer; } }

        public void SetPlayer(Player p) { _currentPlayer = p; }

        public static Player decidePlayer(int x)
        {
            if (x == 0)
            {
                return Player.NoPlayer;
            }
            else if (x == -1)
            {
                return Player.Attacker;
            }
            else
            {
                return Player.Defender;
            }
        }

        public void NewGame(int size)
        {
            _size = size;
            _fields = new Player[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _fields[i, j] = Player.NoPlayer;
                }
            }
            _fields[0, 0] = _fields[0, size - 1] = _fields[size - 1, 0] = _fields[size - 1, size - 1] = Player.Attacker;
            _fields[size / 2, size / 2] = Player.Defender;
            _currentPlayer = Player.Attacker;
            _remainingSteps = size * 4;
        }

        public GameTable( Player [,]m, Player current_player, int steps)
        {
            _fields = m;
            _size = _fields.GetLength(0);
            _currentPlayer = current_player;
            _remainingSteps = steps;
        }
        public GameTable(int size)
        {
            if (size == 3 || size == 5 || size == 7)
            {
                _size = size;
                _fields = new Player[size, size];
                for (int i = 0; i < size; i++)
                {
                    for(int j = 0; j < size; j++)
                    {
                        _fields[i, j] = Player.NoPlayer;
                    }
                }
                _fields[0, 0] = _fields[0, size-1] = _fields[size-1, 0] = _fields[size-1, size-1] = Player.Attacker;
                _fields[size / 2, size / 2] = Player.Defender;
                _currentPlayer = Player.Attacker;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Invalid table size!");
            }
            _remainingSteps = size * 4;
        }

        public Player GetValue(int x, int y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            if ( y < 0 || y >= _fields.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            return _fields[x, y];
        }

        public void SetValue(int x, int y, int to_x, int to_y) //ez a jatekos
        {
            if (x < 0 || x >= _fields.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            if (y < 0 || y >= _fields.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(x), "The Y coordinate is out of range.");

            Player new_field = GetValue(to_x, to_y);    //ide szeretne lepni
            Player field = GetValue(x, y);              //innen
            if ( !(field == Player.Attacker && new_field == Player.NoPlayer) && !(field == Player.NoPlayer && new_field == Player.Attacker) &&
                 !(field == Player.Defender && new_field == Player.NoPlayer) && !(field == Player.NoPlayer && new_field == Player.Defender))
                throw new ArgumentOutOfRangeException(nameof(field), "Wrong value given.");
            if ( Math.Abs( (x-to_x)) + Math.Abs(y-to_y) == 1)
            {
                _fields[x, y] = new_field;
                _fields[to_x, to_y] = field;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid step!");
            }
        }

        /*
        private bool IsAtBorder(int x, int y)
        {
            return  (x == 0 || x == _fields.GetLength(0)-1 || y == 0 || y == _fields.GetLength(1)-1 );
        }

        private bool IsTop(int x, int y) { return x == 0; }
        private bool IsBottom(int x, int y) { return x == _fields.GetLength(0)-1; }
        private bool IsLeft(int x, int y) { return y == 0; }
        private bool IsRight(int x, int y) { return y == _fields.GetLength(1)-1; }
        */

        public int DefenderX()
        {
            for (int i = 0; i < _fields.GetLength(0); i++)
            {
                for (int j = 0; j < _fields.GetLength(1); j++)
                {
                    if (_fields[i, j] == Player.Defender) { return i; }
                }
            }
            return 0;
        }

        public int DefenderY()
        {
            for (int i = 0; i < _fields.GetLength(0); i++)
            {
                for (int j = 0; j < _fields.GetLength(1); j++)
                {
                    if (_fields[i, j] == Player.Defender) { return j; }
                }
            }
            return 0;
        }

        public Boolean DefenderCanGoLeft(int x, int y)
        {
            if (y == 0 || _fields[x, y-1] == Player.Attacker) { return false; }
            return true;
        }

        public Boolean DefenderCanGoRight(int x, int y)
        {
            if (y == _fields.GetLength(0)-1 || _fields[x, y+1] == Player.Attacker) { return false; }
            return true;
        }

        public Boolean DefenderCanGoTop(int x, int y)
        {
            if (x == 0 || _fields[x-1, y] == Player.Attacker) { return false; }
            return true;
        }

        public Boolean DefenderCanGoBottom(int x, int y)
        {
            if (x == _fields.GetLength(0) - 1 || _fields[x+1, y] == Player.Attacker) { return false; }
            return true;
        }
    }

    
}

