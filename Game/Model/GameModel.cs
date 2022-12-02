using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Persistence;


namespace Game.Model
{
    /*public enum TableSize { Small, Medium, Large }*/
    public class GameModel
    {
        //Table sizes
        private const int TableSizeSmall = 3;
        private const int TableSizeMedium = 5;
        private const int TableSizeLarge = 7;

        //Fields
        private IGameDataAccess _gameDataAccess;
        private GameTable _gameTable;
        public int RemainingSteps { get { return _gameTable.GetRemainingSteps; } }
        public int TableSize { get { return _gameTable.GetSize; } }
        public void SetTableSize(int size)
        {
            if (size == TableSizeSmall || size == TableSizeMedium || size == TableSizeLarge)
            {
                _gameTable.SetSize(size);
            }
            else
            {
                throw new GameDataException();
            }
        }
        public GameTable GameTable { get { return _gameTable; } }
        public Boolean IsGameOver { get { return _gameTable.GetRemainingSteps == 0; } }
        public Player Player { get { return _gameTable.GetPlayer; } set => _gameTable.SetPlayer(value); }
        public void SetPlayer(Player p) { _gameTable.SetPlayer(p); }

        public string GetPlayer()
        {
            if (_gameTable.GetPlayer == Player.Attacker)
            {
                return "Attacker";
            }else
            {
                return "Defender";
            }
        }

        //CONSTRUCTOR
        public GameModel(IGameDataAccess dataAccess)
        {
            _gameDataAccess = dataAccess;
            _gameTable = new GameTable(TableSizeMedium);
        }

        //PUBLIC GAME METHODS

        public void NewGame(int size)
        {
            _gameTable.NewGame(size);
            OnGameCreated();
            //TODO : ONGAMECREATED?
        }

        public void Step(int from_x, int from_y, int to_x, int to_y)
        {
            if (from_x < 0 || from_x >= _gameTable.GetSize || to_x < 0 || to_x >= _gameTable.GetSize) // ellenőrizzük a tartományt
                throw new ArgumentOutOfRangeException(nameof(from_x), "Bad column index.");
            if (from_y < 0 || from_y >= _gameTable.GetSize || to_y < 0 || to_y >= _gameTable.GetSize)
                throw new ArgumentOutOfRangeException(nameof(from_y), "Bad row index.");
            if (_gameTable.GetRemainingSteps < 0 ) // ellenőrizzük a lépésszámot
                throw new InvalidOperationException("Game is over!");
            if ( _gameTable.GetValue( from_x, from_y) != Player)
            {
                throw new InvalidOperationException("Wrong player!");
            }
            
            _gameTable.SetValue(from_x, from_y, to_x, to_y); //nem benne ellenorzesek, hanem inkabb a setben
            OnFieldChange(from_x, from_y, to_x, to_y); //

            Player = Player == Player.Attacker ? Player.Defender : Player.Attacker;
            _gameTable.MinusRemainingSteps();

            
            CheckGame();
        }

        public async Task LoadGameAsync(String path)
        {
            if (_gameDataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided!");
            }
            _gameTable = await _gameDataAccess.LoadAsync(path);
            OnGameCreated();
        }

        public async Task SaveGameAsync(String path)
        {
            if (_gameDataAccess == null) throw new InvalidOperationException("No data access is provided!");
            await _gameDataAccess.SaveAsync(path, _gameTable);
        }

        //PRIVATE EVENT METHODS

        private void CheckGame()
        {
            Player won = Player.NoPlayer;
            if (_gameTable.GetRemainingSteps == 0) won = Player.Defender;
            int x = _gameTable.DefenderX(); int y = _gameTable.DefenderY();

            if ( !_gameTable.DefenderCanGoBottom(x, y) && !_gameTable.DefenderCanGoLeft(x, y) && !_gameTable.DefenderCanGoRight(x, y) && !_gameTable.DefenderCanGoTop(x, y))
            {
                won = Player.Attacker;
            }

            if (won != Player.NoPlayer)
            {
                OnGameWon(won);
            }
        }

        //EVENTS
        public event EventHandler<GameWonEventArgs>? GameWon;
        public event EventHandler<FieldChangedEventArgs>? FieldChanged;
        public event EventHandler<FieldChangedEventArgs>? GameCreated;

        private void OnFieldChange(int x, int y, int xx, int yy)
        {
            FieldChanged?.Invoke(this, new FieldChangedEventArgs(x, y, xx, yy));
        }
        private void OnGameWon(Player p)
        {
            GameWon?.Invoke(this, new GameWonEventArgs(p));
        }
        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, new FieldChangedEventArgs(0, 0, 0, 0));
        }
    }
}
