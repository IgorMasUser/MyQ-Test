using CleaningRobotApp.Models;

public interface IRobot
{
    int CurrentX { get; }
    int CurrentY { get; }

    void ExecuteCommands(List<string> commands);
    string GetFinalDirection();
    int GetFinalBattery();
    List<Cell> GetVisitedCells();
    List<Cell> GetCleanedCells();
    void Initialize(string[][] map, int startX, int startY, string facingDirection, int initialBattery);
}
