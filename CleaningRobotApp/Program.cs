using CleaningRobotApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CleaningRobotApp
{
    public class Program
    {
        private readonly IRobot robot;

        public Program(IRobot robot)
        {
            this.robot = robot;
        }

        public void Run(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"Input file not found: {inputFilePath}");
                return;
            }

            string jsonContent = File.ReadAllText(inputFilePath);
            var input = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

            if (input == null)
            {
                Console.WriteLine("Input data is null or invalid JSON.");
            }
            else
            {
                if (!input.ContainsKey("map") || !input.ContainsKey("start") || !input.ContainsKey("battery") || !input.ContainsKey("commands"))
                {
                    Console.WriteLine("Input file is missing required fields.");
                }

                string[][] map = JsonConvert.DeserializeObject<string[][]>(input["map"].ToString());
                int startX = input["start"]["X"];
                int startY = input["start"]["Y"];
                string facingDirection = input["start"]["facing"];
                int initialBattery = (int)input["battery"];
                List<string> commands = JsonConvert.DeserializeObject<List<string>>(input["commands"].ToString());

                // Initialize the robot using the provided parameters
                robot.Initialize(map, startX, startY, facingDirection, initialBattery);
                robot.ExecuteCommands(commands);

                var result = new Dictionary<string, dynamic>
            {
                { "visited", robot.GetVisitedCells() },
                { "cleaned", robot.GetCleanedCells() },
                { "final", new { X = robot.CurrentX, Y = robot.CurrentY, facing = robot.GetFinalDirection() } },
                { "battery", robot.GetFinalBattery() }
            };

                string outputJson = JsonConvert.SerializeObject(result, Formatting.Indented);
                File.WriteAllText(outputFilePath, outputJson);

                Console.WriteLine("Cleaning simulation completed. Output saved to result.json");
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: cleaning_robot <source.json> <result.json>");
                return;
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            var serviceProvider = new ServiceCollection()
                .AddTransient<IRobot, Robot>()
                .BuildServiceProvider();


            var robot = serviceProvider.GetService<IRobot>();

            if (robot == null)
            {
                Console.WriteLine("Failed to initialize the robot. Check the IoC container registration.");
                return;
            }

            var program = new Program(robot);
            program.Run(inputFilePath, outputFilePath);
        }
    }
}
