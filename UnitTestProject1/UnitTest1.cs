using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TowersTests
{
    [TestClass]
    public class Constructor_Should
    {
        [TestMethod]

        public void TowersInitializesuccessfully_WhenInputIsvalid()
        {
            // Arrange
            int input = 5;
            int asExpected = 5;

            // Act
            Towers sa = new Towers(input);

            // Assert
            Assert.AreEqual(asExpected, sa.NumberOfDiscs);

        }

        [TestMethod]
        public void ThrowException_WhenInputIsInvalid()
        {
            // Arrange
            Towers sa;
            int input = 10;

            // Act-Assert
            Assert.ThrowsException<InvalidHeightException>(() => sa = new Towers(input));
        }
    }

    [TestClass]
    public class Move_Should
    {
        [TestMethod]
        public void NumberOfMovesincrease_WhenMoveSuccessfully()
        {
            // Arrange
            Towers sa = new Towers(5);
            int asExpected = 1;

            // Act
            sa.Move(0, 1);

            // Assert
            Assert.AreEqual(asExpected, sa.NumberOfMoves);
        }

        [TestMethod]
        public void ThrowException_WhenInputsAreInvalid()
        {
            // Arrange
            Towers sa = new Towers(5);

            // Act-Assert
            Assert.ThrowsException<InvalidMoveException>(() => sa.Move(2, 1));
        }
    }

    [TestClass]
    public class ToArray_Should
    {
        [TestMethod]
        public void ReturnsJaggedArrayCorrectly_WhenBegins()
        {
            // Arrange
            Towers sa = new Towers(5);
            int[][] towers;
            int[][] asExpected = new int[][]
                {
                    new int[]{1,2,3,4,5},
                    new int[]{ },
                    new int[]{ }
                };
            bool result = true;

            // Act
            towers = sa.ToArray();

            // Assert
            for (int i = 0; i < towers.Length; i++)
            {
                for (int j = 0; j < towers[i].Length; j++)
                {
                    if (towers[i][j] != asExpected[i][j])
                    {
                        result = false;
                    }
                }
            }
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsJaggedArrayCorrectly_WhenMoveSuccessfully()
        {
            // Arrange
            Towers sa = new Towers(5);
            int[][] towers;
            int[][] asExpected = new int[][]
                {
                    new int[]{2,3,4,5},
                    new int[]{1},
                    new int[]{ }
                };
            bool result = true;

            sa.Move(0, 1);

            // Act
            towers = sa.ToArray();

            // Assert
            for (int i = 0; i < towers.Length; i++)
            {
                for (int j = 0; j < towers[i].Length; j++)
                {
                    if (towers[i][j] != asExpected[i][j])
                    {
                        result = false;
                    }
                }
            }
            Assert.IsTrue(result);
        }
    }

}