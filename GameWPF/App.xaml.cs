using Game.Model;
using Game.Persistence;
using GameWPF.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GameWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        private GameModel _model = null!;
        private GameViewModel _viewModel = null!;
        private MainWindow _view = null!;
        #endregion

        #region Constructors
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        #endregion

        #region Application event handlers
        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new GameModel(new GameFileDataAccess());
            _model.GameWon += new EventHandler<GameWonEventArgs>(Model_GameOver);
            _model.NewGame(5);

            _viewModel = new GameViewModel(_model);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            _viewModel.MenuGameSmall_Click += new EventHandler(ViewModel_SmallGame);
            _viewModel.MenuGameMedium_Click += new EventHandler(ViewModel_MediumGame);
            _viewModel.MenuGameLarge_Click += new EventHandler(ViewModel_LargeGame);

            _view = new MainWindow();
            _view.DataContext = _viewModel; //lehet nem kell
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();

            /*
            _model.FieldChanged += new EventHandler<FieldChangedEventArgs>(Model_FieldChanged);
            _model.GameWon += new EventHandler<GameWonEventArgs>(Model_GameWon);
            GenerateTable();
            SetUpTable();*/
        }
        #endregion

        #region View event handlers
        private void View_Closing(object? sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Beadando", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No){
                e.Cancel = true;
            }
        }
        #endregion

        #region ViewModel event handlers
        private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Loading game :)";
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.LoadGameAsync(openFileDialog.FileName);
                    }
                    catch (GameDataException)
                    {
                        MessageBox.Show("Failure loading the game!" + Environment.NewLine + "Wrong file or file format!", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                        //_menuFileSaveGame.Enabled = true;
                    }
                    //GenerateTable();
                    //SetUpTable();
                }
            }
            catch (GameDataException)
            {
                MessageBox.Show("File loading unsuccessfull!", "Beadando", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Saving game :)";
                saveFileDialog.InitialDirectory = "C:\\";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (GameDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("File saving unsuccessfull!", "Beadando", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ViewModel_ExitGame(Object? sender, System.EventArgs e)
        {
            _view.Close();
        }
        #endregion

        #region Model event handlers
        private void Model_GameOver(Object? sender, GameWonEventArgs e)
        {
            //foreach (Button button in _buttonGrid) button.Enabled = false;

            switch (e.Player)
            {
                case Player.Attacker:
                    MessageBox.Show("Congratulations for the Attacker, you won!", "Beadando", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    break;
                case Player.Defender:
                    MessageBox.Show("Congratulations for the Defender, you won!", "Beadando", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    break;
            }
            _model.NewGame(_model.TableSize);
        }

        private void ViewModel_SmallGame(object? sender, System.EventArgs e)
        {
            _model.NewGame(3);
            //_viewModel.RefreshTable();
        }
        private void ViewModel_MediumGame(object? sender, System.EventArgs e)
        {
            _model.NewGame(5);
        }
        private void ViewModel_LargeGame(object? sender, System.EventArgs e)
        {
            _model.NewGame(7);
        }
        #endregion

    }
}
