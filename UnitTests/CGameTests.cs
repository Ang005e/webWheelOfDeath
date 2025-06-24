using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibWheelOfDeath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using static UnitTests.TypeValueDefaults;

namespace LibWheelOfDeath.Tests
{

    
    [TestClass()]
    public class CGameTests
    {

        // ToDo: CREATE A GENERIC FUNCTION TAKING IN PARAM, COMAPARING TYPE OF PARAM WITH EXPECTED VALUE FOR TYPE
        // AND RETURNING A BOOL
        UnitTests.TypeValueDefaults TypeValueDefaults = UnitTests.TypeValueDefaults.GetInstance();

        [TestMethod]
        public void CGameTest()
        {
            CGame game = new();
            Assert.IsNotNull(game);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(100000000000)]
        public void CGameTest(long id)
        {
            CGame game = new(id);
            Assert.IsNotNull(game);
        }


        // Test that game is writing to the database correctly
        [DataTestMethod]
        [DataRow(1, "This is (1) Valid Game Name!", 10, 4, 20000, 6, 8)]
        public void CreateTest(long fkDifficultyId, string gameName, int attempts, int misses, long durationMs, int minBalloons, int maxBalloons)
        {
            CGame game = BuildCGame(fkDifficultyId, gameName, attempts, misses, durationMs, minBalloons, maxBalloons);
            game.Create();
            Assert.IsTrue(game.Id > 0L, "Game was not created");
        }





        // ############ DEFAULT PARAMETER BEHAVIOUR VALIDATIONS ############ \\

        // 1: Default parameter values for each parameter MUST be specified (each datatype should have a specific default value)
        [TestMethod()]
        public void UnsetParametersAreAssignedDefaultValuesOnInitialiseTest()
        {
            CGame game = new();

            // ToDo: Replace default-value-checking with generic method -- since there's one default value each type, this
            // is well suited for generics


            // Test: ALL settable Cruds class fields MUST be assigned default values in the constructor if unspecified in initialisation.
            Assert.AreEqual(TypeValueDefaults.Long, game.Id);
            Assert.AreEqual(TypeValueDefaults.Long, game.FkDifficultyId);

            Assert.AreEqual(TypeValueDefaults.String, game.Game);

            Assert.AreEqual(TypeValueDefaults.Short, game.Attempts);
            Assert.AreEqual(TypeValueDefaults.Short, game.Misses);
            Assert.AreEqual(TypeValueDefaults.Short, game.MinBalloons);
            Assert.AreEqual(TypeValueDefaults.Short, game.MaxBalloons);
            Assert.AreEqual(TypeValueDefaults.Long, game.DurationMilliseconds);

        }


        // 2: DB write (game.Create()) must fail IF a *required* field contains a default value (unspecified)
        [DataTestMethod]
        [DataRow(TypeValueDefaults.Int, "Valid Game Name", 10, 4, 20000, 6, 8)]
        [DataRow(2, TypeValueDefaults.String, 10, 4, 20000, 6, 8)]
        [DataRow(2, "Valid Game Name", TypeValueDefaults.Short, 4, 20000, 6, 8)]
        [DataRow(2, "Valid Game Name", 10, TypeValueDefaults.Short, 20000, 6, 8)]
        [DataRow(2, "Valid Game Name", 10, 4, TypeValueDefaults.Long, 6, 8)]
        [DataRow(2, "Valid Game Name", 10, 4, 20000, TypeValueDefaults.Short, 8)]
        [DataRow(2, "Valid Game Name", 10, 4, 20000, 6, TypeValueDefaults.Short)]
        public void CreateThrowsWhenRequiredFieldIsDefaultTest(long fkDifficultyId, string gameName, int attempts, int misses, long durationMs, int minBalloons, int maxBalloons)
        {
            CGame game = BuildCGame(fkDifficultyId, gameName, attempts, misses, durationMs, minBalloons, maxBalloons);
            Assert.ThrowsException<CWheelOfDeathException>(
                game.Create,
                @$"(Database API): CRUDS class '{nameof(CGame)}' allowed a default value in a required field to be written to the Database");
        }




        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SearchTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ResetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PopulateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.Fail();
        }

        #region Helpers
        private CGame BuildCGame(long fkDifficultyId, string gameName, int attempts, int misses, long durationMs, int minBalloons, int maxBalloons)
        {
            CGame game = new()
            {
                FkDifficultyId = fkDifficultyId,
                Game = gameName,
                Attempts = (short)attempts,
                Misses = (short)misses,
                DurationMilliseconds = durationMs,
                MinBalloons = (short)minBalloons,
                MaxBalloons = (short)maxBalloons
            };

            return game;
        }
        #endregion
    }
}