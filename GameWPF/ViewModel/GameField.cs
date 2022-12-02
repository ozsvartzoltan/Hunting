using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameWPF.ViewModel
{
    public class GameField : ViewModelBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Boolean IsAttacker { get { return (Player.Attacker == Player); } }
        public Boolean IsDefender { get { return (Player.Defender == Player); } }
        public Boolean IsNoPlayer { get { return (Player.NoPlayer == Player); } }
        private Player _player;
        public Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
                OnPropertyChanged();
            }
        }
        public int Number { get; set; }
        public DelegateCommand? StepCommand { get; set; }
    }
}
