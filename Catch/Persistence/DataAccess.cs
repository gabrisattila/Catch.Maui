using Catch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catch.Persistence
{
    public class DataAccess
    {
        #region Fields
        private string _path;
        #endregion

        #region Ctor
        public DataAccess(string? path = null)
        {
            _path = path;
        }
        #endregion
        
        #region Methods
        public async Task<GameModel> LoadGameAsync(string path)
        {
            path = finalPath(path);
            GameModel ret;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    bool first = true;
                    string firstLine = await reader.ReadLineAsync() ?? String.Empty;
                    string[] infos = firstLine.Split(" ");
                    
                    int size = int.Parse(infos[0]);
                    int time = int.Parse(infos[1]);

                    GameTable table = new GameTable(size, new int[size, size]);
                    table.E1 = false; table.E2 = false;
                    table.FirstBomb = new int[2];
                    String[] nums;
                    String line;
                    for (int i = 0; i < size; i++)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        nums = line.Split(' ');
                        for (int j = 0; j < size; j++)
                        {
                            table.Tábla[i, j] = int.Parse(nums[j]);
                            if (first && table.Tábla[i, j] == 1)
                            {
                                table.FirstBomb[0] = i;
                                table.FirstBomb[1] = j;
                                first = false;
                            }
                            if (int.Parse(nums[j]) == 2)
                            {
                                table.E1 = true;
                                table.E1Pos = new int[2];
                                table.E1Pos[0] = i;
                                table.E1Pos[1] = j;
                            }
                            if (int.Parse(nums[j]) == 4)
                            {
                                table.E2 = true;
                                table.E2Pos = new int[2];
                                table.E2Pos[0] = i;
                                table.E2Pos[1] = j;
                            }
                            if (int.Parse(nums[j]) == 3)
                            {
                                table.P = true;
                                table.PPos = new int[2];
                                table.PPos[0] = i;
                                table.PPos[1] = j;
                            }
                        }
                    }
                    ret = new GameModel(time, table);
                    return ret;
                }
            }
            catch
            {
                throw new DataException();
            }

        }
        public async Task SaveGameAsync(string path, GameModel model)
        {
            path = finalPath(path);
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(model.BoardSize + " ");
                    writer.Write(model.Time + " ");
                    await writer.WriteLineAsync();
                    for (int i = 0; i < model.BoardSize; i++)
                    {
                        for (int j = 0; j < model.BoardSize; j++)
                        {
                            await writer.WriteAsync(model.Table.Tábla[i, j] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new DataException();
            }
        }

        public GameTable Load(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = reader.ReadLine();
                    int m = int.Parse(line);
                    int[,] t = new int[m, m];
                    GameTable table = new GameTable(m, t);
                    String[] nums;
                    for (int i = 0; i < m; i++)
                    {
                        line = reader.ReadLine();
                        nums = line.Split(' ');
                        for (int j = 0; j < m; j++)
                        {
                            table.Tábla[i, j] = int.Parse(nums[j]);
                            if (int.Parse(nums[j]) == 2)
                            {
                                table.E1 = true;
                                table.E1Pos = new int[2];
                                table.E1Pos[0] = i;
                                table.E1Pos[1] = j;
                            }
                            if (int.Parse(nums[j]) == 4)
                            {
                                table.E2 = true;
                                table.E2Pos = new int[2];
                                table.E2Pos[0] = i;
                                table.E2Pos[1] = j;
                            }
                            if (int.Parse(nums[j]) == 3)
                            {
                                table.P = true;
                                table.PPos = new int[2];
                                table.PPos[0] = i;
                                table.PPos[1] = j;
                            }
                        }
                    }
                    return table;
                }
            }
            catch
            {
                throw new DataException();
            }
        }
        private String finalPath(String PATH) => !String.IsNullOrEmpty(_path) ? Path.Combine(_path, PATH) : PATH;

        #endregion
    }
}
