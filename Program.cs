namespace ConsoleApp2;

public class Program
{
    public static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Green;

        var gol = new GameOfLife();

        while (true)
        {
            gol.Print();
            Thread.Sleep(200);
            gol.NextGeneration();
        }
    }
}

public class GameOfLife
{
    public const int LengthX = 80;
    public const int LengthY = 330;
    private bool[,] Cells;
    private bool[,] CellsNext;
    private readonly char[] Line;

    public GameOfLife()
    {
        Cells = new bool[LengthX, LengthY];
        CellsNext = new bool[LengthX, LengthY];
        Line = new char[LengthY];

        var random = new Random();

        Iterate((x, y) => Cells[x, y] = random.Next(0, 3000) is var n && n > 2800 || n % 30 == 2);
    }

    public void NextGeneration()
    {
        Iterate((x, y) => CellsNext[x, y] = ShouldLive(x, y));
        (Cells, CellsNext) = (CellsNext, Cells);
    }

    public void Print()
    {
        Console.Clear();

        Iterate(
            (x, y) => Line[y] = Cells[x, y] ? '\u2588' : ' ',
            () => Console.WriteLine(Line));
    }

    private bool ShouldLive(int x, int y)
    {
        var isAlive = Cells[x, y];
        var neighbours = CountNeighbours(x, y);

        // Any live cell with two or three live neighbours survives.
        if (isAlive && neighbours is 2 or 3)
            return true;

        // Any dead cell with three live neighbours becomes a live cell.
        if (isAlive is false && neighbours is 3)
            return true;

        // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
        return false;
    }

    private void Iterate(Action<int, int> action, Action outerLoop = null)
    {
        for (var x = 0; x < LengthX; x++)
        {
            for (var y = 0; y < LengthY; y++)
                action(x, y);

            outerLoop?.Invoke();
        }
    }

    private int CountNeighbours(int x, int y)
    {
        var lineAbove = IsAlive(x - 1, y - 1) + IsAlive(x + 0, y - 1) + IsAlive(x + 1, y - 1);
        var sameLine = IsAlive(x - 1, y + 0) + 0 + IsAlive(x + 1, y + 0);
        var lineBelow = IsAlive(x - 1, y + 1) + IsAlive(x + 0, y + 1) + IsAlive(x + 1, y + 1);

        return lineAbove + sameLine + lineBelow;

        int IsAlive(int x, int y)
        {
            if (valid(x, LengthX) && valid(y, LengthY))
                return Cells[x, y] ? 1 : 0;

            return 0;

            static bool valid(int n, int bound) => n > -1 && n < bound;
        }
    }
}
