using CleaningRobotApp.Models;

namespace CleaningRobotApp.Services
{
    public class Robot : IRobot
    {
        private List<Cell> visitedCells;
        private List<Cell> cleanedCells;
        private int battery;
        private string[][] map;
        private int currentX;
        private int currentY;
        private string currentDirection;

        public int CurrentX
        {
           get { return currentX;}
           private set { currentX = value; }
        }

        public int CurrentY
        {
            get { return currentY;}
            private set { currentY = value; }
        }

        public Robot()
        {
            visitedCells = new List<Cell>();
            cleanedCells = new List<Cell>();
            battery = 0;
            map = null;
            currentX = 0;
            currentY = 0;
            currentDirection = "N";
        }

        public void Initialize(string[][] map, int startX, int startY, string facingDirection, int initialBattery)
        {
            this.map = map;
            currentX = startX;
            currentY = startY;
            currentDirection = facingDirection;
            battery = initialBattery;
        }

        public void ExecuteCommands(List<string> commands)
        {
            foreach (string command in commands)
            {
                if (battery <= 0)
                {
                    break;
                }

                switch (command)
                {
                    case "TL":
                        TurnLeft();
                        break;
                    case "TR":
                        TurnRight();
                        break;
                    case "A":
                        Advance();
                        break;
                    case "C":
                        if (IsCleanableSpace(currentX, currentY))
                        {
                            cleanedCells.Add(new Cell { X = currentX, Y = currentY });
                            battery -= 5;
                        }
                        break;
                }
            }
        }

        public string GetFinalDirection()
        {
            return currentDirection;
        }

        public int GetFinalBattery()
        {
            return battery;
        }

        public List<Cell> GetVisitedCells()
        {
            return visitedCells;
        }

        public List<Cell> GetCleanedCells()
        {
            return cleanedCells;
        }

        private bool IsInMapBounds(int x, int y)
        {
            return x >= 0 && x < map[0].Length && y >= 0 && y < map.Length;
        }

        private bool IsCleanableSpace(int x, int y)
        {
            return IsInMapBounds(x, y) && map[y][x] == "S";
        }

        private bool IsObstacle(int x, int y)
        {
            return !IsInMapBounds(x, y) || map[y][x] == "C";
        }

        private void TurnLeft()
        {
            switch (currentDirection)
            {
                case "N":
                    currentDirection = "W";
                    break;
                case "W":
                    currentDirection = "S";
                    break;
                case "S":
                    currentDirection = "E";
                    break;
                case "E":
                    currentDirection = "N";
                    break;
            }
            battery--;
        }

        private void TurnRight()
        {
            switch (currentDirection)
            {
                case "N":
                    currentDirection = "E";
                    break;
                case "E":
                    currentDirection = "S";
                    break;
                case "S":
                    currentDirection = "W";
                    break;
                case "W":
                    currentDirection = "N";
                    break;
            }
            battery--;
        }

        private void Advance()
        {
            int newX = currentX;
            int newY = currentY;
            switch (currentDirection)
            {
                case "N":
                    newY--;
                    break;
                case "E":
                    newX++;
                    break;
                case "S":
                    newY++;
                    break;
                case "W":
                    newX--;
                    break;
            }

            if (IsCleanableSpace(newX, newY))
            {
                currentX = newX;
                currentY = newY;
                visitedCells.Add(new Cell { X = currentX, Y = currentY });
                if (!cleanedCells.Exists(c => c.X == currentX && c.Y == currentY))
                {
                    cleanedCells.Add(new Cell { X = currentX, Y = currentY });
                    battery -= 5;
                }
                else
                {
                    battery -= 2;
                }
            }
            else if (IsObstacle(newX, newY))
            {
                BackOffStrategy();
            }
        }

        protected void BackOffStrategy()
        {
            int[] offsetX = { 0, 1, 0, -1 };
            int[] offsetY = { -1, 0, 1, 0 };
            int originalX = currentX;
            int originalY = currentY;
            string originalDirection = currentDirection;
            int backOffSequenceIndex = 0;

            while (backOffSequenceIndex < 5)
            {
                int newX = currentX + offsetX[backOffSequenceIndex];
                int newY = currentY + offsetY[backOffSequenceIndex];

                if (IsObstacle(newX, newY))
                {
                    currentX = originalX;
                    currentY = originalY;
                    currentDirection = originalDirection;
                    backOffSequenceIndex++;
                }
                else
                {
                    currentX = newX;
                    currentY = newY;
                    visitedCells.Add(new Cell { X = currentX, Y = currentY });
                    battery -= 2;

                    if (backOffSequenceIndex == 0)
                    {
                        cleanedCells.Add(new Cell { X = currentX, Y = currentY });
                        battery -= 5;
                    }
                    else
                    {
                        battery -= 1;
                    }
                    return;
                }
            }

            // If the back-off strategy fails, the robot is stuck, so finish the program.
            Console.WriteLine("Robot is stuck. Cleaning simulation completed.");
            return;
        }
    }
}

