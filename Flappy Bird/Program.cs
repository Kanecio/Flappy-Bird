using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = true;

        string[] menuOptions = { "Start Game", "Customization", "Settings", "Exit" };
        int selectedOption = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Flappy Bird!");
            Console.WriteLine("Please select an option:");

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == selectedOption)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(menuOptions[i]);

                Console.ResetColor();
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedOption = Math.Max(0, selectedOption - 1);
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedOption = Math.Min(menuOptions.Length - 1, selectedOption + 1);
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                if (selectedOption == menuOptions.Length - 1)
                {
                    Console.WriteLine("Exiting...");
                    return;
                }
                else
                {
                    Console.WriteLine("Selected: " + menuOptions[selectedOption]);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                    if (selectedOption == 0) // If "Start Game" is selected
                    {
                        StartGame(); // Start the game immediately
                        return; // Return from the menu loop to exit the menu
                    }
                    // Handle other menu options here
                }
            }
        }
    }

    static void StartGame()
    {
        Console.Clear();
        Console.WriteLine("Press Space to start");

        // Calculate the position to display the message in the middle
        int messageX = (Console.WindowWidth - "Press Space to start".Length) / 2;
        int messageY = Console.WindowHeight / 2;

        Console.SetCursorPosition(messageX, messageY);

        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);
        } while (keyInfo.Key != ConsoleKey.Spacebar);

        Bird bird = new Bird();
        List<Obstacles> obstacles = new List<Obstacles>();

        while (true)
        {
            Console.Clear();

            // Update bird
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Spacebar)
                    bird.Flap();
            }

            bird.Update();
            bird.Display();

            // Update obstacles
            if (obstacles.Count == 0 || obstacles[obstacles.Count - 1].GetRightPosition() < Console.WindowWidth / 2)
            {
                Obstacles newObstacle = new Obstacles();
                obstacles.Add(newObstacle);
            }

            foreach (Obstacles obstacle in obstacles.ToArray())
            {
                obstacle.Update();
                obstacle.Display();

                if (obstacle.IsBirdColliding(bird))
                {
                    GameOver();
                    return;
                }

                if (obstacle.GetRightPosition() < 0)
                    obstacles.Remove(obstacle);
            }

            Thread.Sleep(50);
        }
    }


    static void GameOver()
    {
        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2);
        Console.WriteLine("Game Over!");
        Console.SetCursorPosition(Console.WindowWidth / 2 - 15, Console.WindowHeight / 2 + 1);
        Console.WriteLine("Press Enter to restart or Esc to exit.");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                StartGame();
                Console.Clear();
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                return;
            }
        }
    }
}

class Bird
{
    private int position;
    private int jumpStrength;
    private int gravity;
    private int velocity;

    public Bird()
    {
        position = Console.WindowHeight / 2;
        jumpStrength = 4; // Adjust jump strength as needed
        gravity = 1; // Adjust gravity as needed
        velocity = 0;
    }

    public void Flap()
    {
        velocity = -jumpStrength;
    }

    public void Update()
    {
        velocity += gravity;
        position += velocity;

        // Keep the bird within the bounds of the console window
        position = Math.Max(0, Math.Min(Console.WindowHeight - 1, position));
    }

    public void Display()
    {
        Console.SetCursorPosition(Console.WindowWidth / 3, position);
        Console.WriteLine("^");
    }

    public int GetPosition()
    {
        return position;
    }

    public int GetHorizontalPosition()
    {
        return Console.WindowWidth / 3; // Assuming bird's horizontal position remains fixed
    }
} // <-- Added closing brace for the Bird class


class Obstacles
{
    private Random random;
    private int gapPosition;
    private int gapSize;
    private int leftPosition;
    private int rightPosition;
    private int speed;

    public Obstacles()
    {
        random = new Random();
        gapPosition = random.Next(3, Console.WindowHeight - 3); // Random gap position
        gapSize = 10; // Adjust gap size as needed
        leftPosition = Console.WindowWidth - 6; // Start near the right edge of the console
        rightPosition = Console.WindowWidth - 1; // Adjust as needed
        speed = 1; // Adjust speed of tubes
    }

    public void Update()
    {
        leftPosition -= speed;
        rightPosition -= speed;
    }

    public void Display()
    {
        leftPosition = Math.Max(0, Math.Min(Console.WindowWidth - 1, leftPosition));
        rightPosition = Math.Max(0, Math.Min(Console.WindowWidth - 1, rightPosition));

        for (int i = 0; i < Console.WindowHeight; i++)
        {
            if (leftPosition >= 0 && leftPosition < Console.WindowWidth && rightPosition >= 0 && rightPosition < Console.WindowWidth)
            {
                if (i < gapPosition - gapSize / 2 || i >= gapPosition + gapSize / 2)
                {
                    Console.SetCursorPosition(leftPosition, i);
                    Console.Write("||");
                    Console.SetCursorPosition(rightPosition, i);
                    Console.Write("||");
                }
            }
        }
    }

    public bool IsBirdColliding(Bird bird)
    {
        if (bird.GetHorizontalPosition() >= leftPosition && bird.GetHorizontalPosition() <= rightPosition)
        {
            if (bird.GetPosition() < gapPosition - gapSize / 2 || bird.GetPosition() >= gapPosition + gapSize / 2)
            {
                return true;
            }
        }
        return false;
    }

    public int GetRightPosition()
    {
        return rightPosition;
    }
}

