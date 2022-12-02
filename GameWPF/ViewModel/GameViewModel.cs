using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Game.Model; 

namespace GameWPF.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private GameModel _model;

        #region Properties
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand SmallTableCommand { get; private set; }
        public DelegateCommand MediumTableCommand { get; private set; }
        public DelegateCommand LargeTableCommand { get; private set; }

        public ObservableCollection<GameField> Fields { get; set; }
        public int RemainingSteps { get { return _model.RemainingSteps; } }
        public string CurrentPlayer { get { return _model.GetPlayer();  } }
        public string CurrentClick { get { return click.ToString();  } }
        private int _size;
        public int Size { get { return _model.TableSize; }
            set 
            {
                _size = value;
                OnPropertyChanged(); 
            }
        }
        public Boolean IsSmallTable { 
            get { return _model.TableSize == 3; }
            set
            {
                if(_model.TableSize == 3)
                {
                    return;
                }
                _model.SetTableSize(3);
                OnPropertyChanged(nameof(IsSmallTable));
                OnPropertyChanged(nameof(IsMediumTable));
                OnPropertyChanged(nameof(IsLargeTable));
            }
        }
        public Boolean IsMediumTable
        {
            get { return _model.TableSize == 5; }
            set
            {
                if (_model.TableSize == 5)
                {
                    return;
                }
                _model.SetTableSize(3);
                OnPropertyChanged(nameof(IsSmallTable));
                OnPropertyChanged(nameof(IsMediumTable));
                OnPropertyChanged(nameof(IsLargeTable));
            }
        }
        public Boolean IsLargeTable
        {
            get { return _model.TableSize == 7; }
            set
            {
                if (_model.TableSize == 7)
                {
                    return;
                }
                _model.SetTableSize(3);
                OnPropertyChanged(nameof(IsSmallTable));
                OnPropertyChanged(nameof(IsMediumTable));
                OnPropertyChanged(nameof(IsLargeTable));
            }
        }
        #endregion

        #region Events
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
        public event EventHandler? ExitGame;
        public event EventHandler? MenuGameSmall_Click;
        public event EventHandler? MenuGameMedium_Click;
        public event EventHandler? MenuGameLarge_Click;
        #endregion

        #region Constructors
        public GameViewModel(GameModel model)
        {
            _model = model;
            _model.FieldChanged += new EventHandler<FieldChangedEventArgs>(Model_GameAdvanced);
            _model.GameWon += new EventHandler<GameWonEventArgs>(Model_GameOver);
            _model.GameCreated += Model_GameCreated;
            //GameCreated eventargs?

            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            SmallTableCommand = new DelegateCommand(param => SmallTable());
            MediumTableCommand = new DelegateCommand(param => MediumTable());
            LargeTableCommand = new DelegateCommand(param => LargeTable());

            Fields = new ObservableCollection<GameField>();
            GenerateTable();
        }
        #endregion
        private void GenerateTable()
        {
            Fields.Clear();
            for (int i = 0; i < _model.TableSize; ++i)
            {
                for (int j = 0; j < _model.TableSize; ++j)
                {
                        Fields.Add(new GameField
                        {
                            X = i,
                            Y = j,
                            Player = Player.NoPlayer,
                            Number = (i * _model.TableSize) + j,
                            StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                        });
                    

                }
            }
            RefreshTable();
            OnPropertyChanged(nameof(Size));
        }
        #region Private methods

        public void RefreshTable()
        {
            foreach(GameField field in Fields)
            {
                field.Player = _model.GameTable.GetValue(field.X, field.Y);
            }
            OnPropertyChanged(nameof(RemainingSteps));
            //Size = _model.TableSize;  esetleg ha uj jatek van akkor kell?
        }

        private int _x, _y;
        private int click = 0;
        private GameField f = null!;
        private void StepGame(int i)
        {
            GameField field = Fields[i];
            int x = field.X;
            int y = field.Y;
            if (click == 0)
            {
                _x = x; _y = y; ++click; f = field;
            }
            else
            {
                try
                {
                    _model.Step(_x, _y, x, y);
                    field.Player = f.Player;
                    f.Player = Player.NoPlayer;
                }
                catch
                {
                }
                click = 0;
                OnPropertyChanged(nameof(RemainingSteps));
                OnPropertyChanged(nameof(CurrentPlayer));
                //RefreshTable();
            }
            OnPropertyChanged(nameof(CurrentClick));
            /*
            _toolLabelGameRemainderSteps.Text = _model.RemainingSteps.ToString();
            _toolLabelGamePlayer.Text = _model.Player.ToString();
            _toolLabelClickCount.Text = click.ToString();*/
            //RefreshTable();
        }
        #endregion

        #region Game event handlers
        private void Model_GameOver(object? sender, GameWonEventArgs e)
        {
        }

        private void Model_GameAdvanced(object? sender, FieldChangedEventArgs e)
        {
            OnPropertyChanged(nameof(RemainingSteps));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CurrentClick));
        }
        private void Model_GameCreated(object? sender, FieldChangedEventArgs e)
        {
            GenerateTable();
        }
        #endregion

        #region Event methods
        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }
        private void SmallTable()
        {
            MenuGameSmall_Click?.Invoke(this, EventArgs.Empty);
        }
        private void MediumTable()
        {
            MenuGameMedium_Click?.Invoke(this, EventArgs.Empty);
        }
        private void LargeTable()
        {
            MenuGameLarge_Click?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
