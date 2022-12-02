using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Persistence
{
    public interface IGameDataAccess
    {
        Task<GameTable> LoadAsync(String path);
        Task SaveAsync(String path, GameTable table);
    }
}

public enum Player
{
    NoPlayer,
    Attacker,
    Defender
}

