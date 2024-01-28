using System;
using System.Collections.Generic;
using System.Threading;

enum Border
{
    MaxRight = 40,
    MaxBottom = 20
}

class SnakeGame
{
    private List<Position> snake;
    private Position food;
    private Direction direction;
    private bool isGameOver;

    public SnakeGame()
    {
        snake = new List<Position>
        {
            new Position(10, 10), 
        };
        food = GenerateFoodPosition();
        direction = Direction.Right;
        isGameOver = false;
    }

    public void RunGame()
    {
        Console.Clear();
        DrawBorder();

        Thread inputThread = new Thread(InputListener);
        inputThread.Start();

        while (!isGameOver)
        {
            MoveSnake();
            CheckCollision();
            CheckFood();
            DrawSnake();
            DrawFood();
            Thread.Sleep(100);
        }

        Console.Clear();
        Console.WriteLine("Игра закончена");
        Console.ReadKey();
    }

    private void InputListener()
    {
        while (!isGameOver)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                SetDirection(key);
            }
        }
    }

    private void SetDirection(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (direction != Direction.Down)
                    direction = Direction.Up;
                break;
            case ConsoleKey.DownArrow:
                if (direction != Direction.Up)
                    direction = Direction.Down;
                break;
            case ConsoleKey.LeftArrow:
                if (direction != Direction.Right)
                    direction = Direction.Left;
                break;
            case ConsoleKey.RightArrow:
                if (direction != Direction.Left)
                    direction = Direction.Right;
                break;
        }
    }

    private void MoveSnake()
    {
        Position head = snake.First();
        Position newHead = new Position(head.X, head.Y);

        switch (direction)
        {
            case Direction.Up:
                newHead.Y--;
                break;
            case Direction.Down:
                newHead.Y++;
                break;
            case Direction.Left:
                newHead.X--;
                break;
            case Direction.Right:
                newHead.X++;
                break;
        }

        snake.Insert(0, newHead);

        for (int i = 1; i < snake.Count; i++)
        {
            Position temp = snake[i];
            snake[i] = head;
            head = temp;
        }

        if (newHead.Equals(food))
        {
            food = GenerateFoodPosition();
        }
   
        else
        {
            snake.RemoveAt(snake.Count - 1);
        }
    }

    private void CheckCollision()
    {
        Position head = snake.First();

        if (head.X < 0 || head.X >= (int)Border.MaxRight || head.Y < 0 || head.Y >= (int)Border.MaxBottom)
        {
            isGameOver = true;
        }

        if (snake.Skip(1).Any(segment => segment.Equals(head)))
        {
            isGameOver = true;
        }
    }

    private void CheckFood()
    {
        Position head = snake.First();
        if (head.Equals(food))
        {
            snake.Add(new Position(0, 0));
        }
    }

    private Position GenerateFoodPosition()
    {
        Random random = new Random();
        int x = random.Next((int)Border.MaxRight);
        int y = random.Next((int)Border.MaxBottom);

        while (snake.Any(segment => segment.X == x && segment.Y == y))
        {
            x = random.Next((int)Border.MaxRight);
            y = random.Next((int)Border.MaxBottom);
        }

        return new Position(x, y);
    }

    private void DrawSnake()
    {
        Console.SetCursorPosition(snake.First().X, snake.First().Y);
        Console.Write("O");

        foreach (Position segment in snake.Skip(1))
        {
            Console.SetCursorPosition(segment.X, segment.Y);
            Console.Write("*");
        }
    }

    private void DrawFood()
    {
        Console.SetCursorPosition(food.X, food.Y);
        Console.Write("+");
    }

    private void DrawBorder()
    {
        for (int i = 0; i <= (int)Border.MaxBottom; i++)
        {
            Console.SetCursorPosition((int)Border.MaxRight, i);
            Console.Write("|");
        }
    }
}

class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Position other = (Position)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        SnakeGame snakeGame = new SnakeGame();
        snakeGame.RunGame();
    }
}
