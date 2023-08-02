using System.Collections.Generic;
using Xunit;
using CleaningRobotApp.Services;

namespace CleaningRobotApp.Tests
{
    public class RobotTests
    {
        [Fact]
        public void Robot_CanInitialize()
        {
            // Arrange
            string[][] map = new string[][]
            {
                new string[] { "S", "S", "S", "S" },
                new string[] { "S", "S", "C", "S" },
                new string[] { "S", "S", "S", "S" },
                new string[] { "S", null, "S", "S" }
            };
            int startX = 3;
            int startY = 0;
            string facingDirection = "N";
            int initialBattery = 100;

            // Act
            IRobot robot = new Robot(); 
            robot.Initialize(map, startX, startY, facingDirection, initialBattery);

            // Assert
            Assert.Equal(startX, robot.CurrentX);
            Assert.Equal(startY, robot.CurrentY);
            Assert.Equal(facingDirection, robot.GetFinalDirection());
            Assert.Equal(initialBattery, robot.GetFinalBattery());
            Assert.Empty(robot.GetVisitedCells());
            Assert.Empty(robot.GetCleanedCells());
        }

        [Fact]
        public void Robot_CanExecuteCommands()
        {
            // Arrange
            string[][] map = new string[][]
            {
                new string[] { "S", "S", "S", "S" },
                new string[] { "S", "S", "C", "S" },
                new string[] { "S", "S", "S", "S" },
                new string[] { "S", null, "S", "S" }
            };
            int startX = 3;
            int startY = 0;
            string facingDirection = "N";
            int initialBattery = 100;
            List<string> commands = new List<string> { "TL", "A", "C", "A", "C", "TR", "A", "C" };

            // Act
            IRobot robot = new Robot();
            robot.Initialize(map, startX, startY, facingDirection, initialBattery);
            robot.ExecuteCommands(commands);

            // Assert
            Assert.Equal(3, robot.GetVisitedCells().Count);
            Assert.Equal(5, robot.GetCleanedCells().Count);
        }

        [Fact]
        public void Robot_OutOfBattery()
        {
            // Arrange
            string[][] map = new string[][]
            {
                new string[] { "S", "S", "S", "S" },
                new string[] { "S", "S", "C", "S" },
                new string[] { "S", "S", "S", "S" },
                new string[] { "S", null, "S", "S" }
            };
            int startX = 3;
            int startY = 0;
            string facingDirection = "N";
            int initialBattery = 10; // Low initial battery
            List<string> commands = new List<string> { "A", "A", "A", "A", "A", "A" }; 

            // Act
            IRobot robot = new Robot();
            robot.Initialize(map, startX, startY, facingDirection, initialBattery);
            robot.ExecuteCommands(commands);

            // Assert
            Assert.Equal(3, robot.GetVisitedCells().Count);
            Assert.Equal(1, robot.GetCleanedCells().Count);
            Assert.Equal("N", robot.GetFinalDirection());
            Assert.Equal(-1, robot.GetFinalBattery());
        }
    }
}
