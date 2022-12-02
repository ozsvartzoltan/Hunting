using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Game.Model;

namespace Game.Persistence
{
    public  class GameFileDataAccess : IGameDataAccess
    {
        public async Task<GameTable> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    String[] numbers = line.Split(' ');
                    int tableSize = int.Parse(numbers[0]);
                    int remainingSteps = int.Parse(numbers[1]);
                    Player player = GameTable.decidePlayer(int.Parse(numbers[2]));

                    Player[,] m = new Player[tableSize, tableSize];
                    for (int i = 0; i < tableSize; ++i)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        numbers = line.Split(' ');
                        for (int j =0; j < tableSize; ++j)
                        {
                            m[i, j] = GameTable.decidePlayer(int.Parse(numbers[j]));
                        }
                    }
                    GameTable table = new GameTable(m, player, remainingSteps);

                    return table;
                }
            }
            catch
            {
                throw new GameDataException(); 
            }
        }
        

        public async Task SaveAsync(String path, GameTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    int p;
                    if (table.GetPlayer == Player.NoPlayer) p = 0;
                    else if (table.GetPlayer == Player.Attacker) p = -1;
                    else p = 2;
                    await writer.WriteLineAsync(table.GetSize + " " + table.GetRemainingSteps + " " + p.ToString());
                    for (int i = 0; i < table.GetSize; ++i)
                    {
                        for (int j = 0; j < table.GetSize; ++j)
                        {
                            writer.Write(table.GetValue(i, j) == Player.Attacker ? "-1 " : table.GetValue(i, j) == Player.Defender ? "1 " : "0" + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new GameDataException();
            }
        }
        
    }

    [Serializable]
    public class GameDataException : Exception
    {
        public GameDataException()
        {
        }

    }
}
