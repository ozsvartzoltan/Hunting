using Game.Model;
using Game.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace GameTest
{
    [TestClass]
    public class UnitTest1
    {
        private Mock<IGameDataAccess> _mock = null!;
        private GameModel _model = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IGameDataAccess>();
            //_mock.Setup(mock => mock.LoadAsync(It.IsAny<String>())).Returns(() => Enumerable.Repeat(Player.NoPlayer, 9).ToArray());
            _mock.Setup(mock => mock.LoadAsync("test.txt"));
            _model = new GameModel(_mock.Object);
        }

        [TestMethod]
        public void GameSizeTest()
        {
            _model.NewGame(5);
            Assert.AreEqual(5, _model.TableSize);
        }

        [TestMethod]
        public void GameRemainingStepsLeft()
        {
            _model.NewGame(5);
            Assert.AreEqual(20, _model.RemainingSteps);
        }

        [TestMethod]
        public void GameCurrentPlayer()
        {
            _model.NewGame(5);
            Assert.AreEqual(Player.Attacker, _model.Player);
        }

        [TestMethod]
        public void GameConstructorTestNoPlayer()
        {
            _model.NewGame(5);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ( !((i == 0 && j == 0) || (i == 4 && (j == 0) || (i == 0 && j == 4) || (i == 4 && j == 4) || (i == 2 && j == 2) )))
                    {
                        Assert.AreEqual(Player.NoPlayer, _model.GameTable.GetValue(i, j));
                    }
                }
            }
            
        }

        [TestMethod]
        public void GameConstructorTestPlayers()
        {
            _model.NewGame(5);
            Assert.AreEqual(Player.Defender, _model.GameTable.GetValue(2, 2));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(0, 0));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(0, 4));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(4, 0));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(4, 4));
        }

        [TestMethod]
        public void GameTestStep()
        {
            _model.NewGame(5);
            _model.Step(0, 0, 0, 1);
            Assert.AreEqual(20, _model.RemainingSteps);
            Assert.AreEqual(Player.Defender, _model.Player);

            /* EGY LÉPÉS A TÁMADÓVAL */
            _model.Step(2, 2, 2, 3);
            Assert.AreEqual(19, _model.RemainingSteps);
            Assert.AreEqual(Player.Attacker, _model.Player);
        }

        [TestMethod]
        public void GameSizeTest7()
        {
            _model.NewGame(7);
            Assert.AreEqual(7, _model.TableSize);
        }

        [TestMethod]
        public void GameRemainingStepsLeft7()
        {
            _model.NewGame(7);
            Assert.AreEqual(28, _model.RemainingSteps);
        }

        [TestMethod]
        public void GameCurrentPlayer7()
        {
            _model.NewGame(7);
            Assert.AreEqual(Player.Attacker, _model.Player);
        }

        [TestMethod]
        public void GameConstructorTestNoPlayer7()
        {
            _model.NewGame(7);
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (!((i == 0 && j == 0) || (i == 6 && (j == 0) || (i == 0 && j == 6) || (i == 6 && j == 6) || (i == 3 && j == 3))))
                    {
                        Assert.AreEqual(Player.NoPlayer, _model.GameTable.GetValue(i, j));
                    }
                }
            }

        }

        [TestMethod]
        public void GameConstructorTestPlayers7()
        {
            _model.NewGame(7);
            Assert.AreEqual(Player.Defender, _model.GameTable.GetValue(3, 3));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(0, 0));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(0, 6));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(6, 0));
            Assert.AreEqual(Player.Attacker, _model.GameTable.GetValue(6, 6));
        }

        [TestMethod]
        public void GameTestStep7()
        {
            _model.NewGame(7);
            _model.Step(0, 0, 0, 1);
            Assert.AreEqual(28, _model.RemainingSteps);
            Assert.AreEqual(Player.Defender, _model.Player);

            /* EGY LÉPÉS A TÁMADÓVAL */
            _model.Step(3, 3, 2, 3);
            Assert.AreEqual(27, _model.RemainingSteps);
            Assert.AreEqual(Player.Attacker, _model.Player);
        }

        [TestMethod]
        public async void GameSaveTest()
        {
            Player currentPlayyer = _model.Player;
            int remainingSteps = _model.RemainingSteps;
            Player[,] matrix = new Player[5, 5]; 
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    matrix[i, j] = _model.GameTable.GetValue(i, j);
                }
            }

            await _model.SaveGameAsync(String.Empty);
            Assert.AreEqual(20, _model.RemainingSteps);
            Assert.AreEqual(Player.Attacker, _model.Player);
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {
                    Assert.AreEqual(matrix[i, j], _model.GameTable.GetValue(i, j));
                }
            }
        }
    }
}