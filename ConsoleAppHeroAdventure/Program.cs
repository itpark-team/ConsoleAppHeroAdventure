//поле - двумерный массив размера N на M
//портал - объект, пренадлежит полю
//стены - объекты, прендалежат полю, по краям поля сделать стены
//герой - объект, НЕ пренадлежит полю, рисуется поверх полю

//1) генерация поля и стен на нём
//2) генерация портала в поле (перандомировать поле если мы оказались заперты)
//3) передвижение героя по полю
//4) вход героя в портал и переход к шагу 1

using ConsoleAppHeroAdventure;

int InitRows()
{
    Random random = new Random();
    return random.Next((int)Constants.MinRows, (int)Constants.MaxRows + 1);
}

int InitCols()
{
    Random random = new Random();
    return random.Next((int)Constants.MinCols, (int)Constants.MaxCols + 1);
}

Cell[,] CreateField(int rows, int cols)
{
    return new Cell[rows, cols];
}

void InitField(Cell[,] field)
{
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            field[i, j] = Cell.Empty;
        }
    }

    for (int i = 0; i < rows; i++)
    {
        field[i, 0] = Cell.Bound;
        field[i, cols - 1] = Cell.Bound;
    }

    for (int j = 0; j < cols; j++)
    {
        field[0, j] = Cell.Bound;
        field[rows - 1, j] = Cell.Bound;
    }
}

int InitIHero()
{
    return (int)Constants.StartIHero;
}

int InitJHero()
{
    return (int)Constants.StartJHero;
}

void SetPortal(Cell[,] field, int iHero, int jHero)
{
    Random random = new Random();
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);

    int iPortal, jPortal;
    do
    {
        iPortal = random.Next(1, rows - 1);
        jPortal = random.Next(1, cols - 1);
    } while (iPortal == iHero && jPortal == jHero);

    field[iPortal, jPortal] = Cell.Portal;
}

void SetWalls(Cell[,] field, int currentWallPercent, int iHero, int jHero)
{
    Random random = new Random();
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);

    int countWalls = (int)((rows - 2) * (cols - 2) * currentWallPercent / 100.0);
    for (int i = 0; i < countWalls; i++)
    {
        int iWall, jWall;
        do
        {
            iWall = random.Next(1, rows - 1);
            jWall = random.Next(1, cols - 1);
        } while (iWall == iHero && jWall == jHero
                 || field[iWall, jWall] == Cell.Portal
                 || field[iWall, jWall] == Cell.Wall);

        field[iWall, jWall] = Cell.Wall;
    }
}

void ResetConsole()
{
    Console.Clear();
    Console.ResetColor();
}

void PrintGameInfo(int level, int wallPercent)
{
    Console.WriteLine($"Level = {level}, Wall Percent = {wallPercent}");
}

void PrintField(Cell[,] field, int iHero, int jHero)
{
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (i == iHero && j == jHero)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write((char)Constants.HeroSkin);
            }
            else
            {
                switch (field[i, j])
                {
                    case Cell.Empty:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case Cell.Wall:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case Cell.Portal:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case Cell.Bound:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }

                Console.Write((char)field[i, j]);
            }
        }

        Console.WriteLine();
    }
}

void MoveHero(Cell[,] field, ref int iHero, ref int jHero, ref bool heroInAdventure)
{
    ConsoleKey key = Console.ReadKey(false).Key;
    switch (key)
    {
        case ConsoleKey.A:
            if (field[iHero, jHero - 1] == Cell.Empty || field[iHero, jHero - 1] == Cell.Portal)
            {
                jHero--;
            }

            break;

        case ConsoleKey.W:
            if (field[iHero - 1, jHero] == Cell.Empty || field[iHero - 1, jHero] == Cell.Portal)
            {
                iHero--;
            }

            break;

        case ConsoleKey.D:
            if (field[iHero, jHero + 1] == Cell.Empty || field[iHero, jHero + 1] == Cell.Portal)
            {
                jHero++;
            }

            break;

        case ConsoleKey.S:
            if (field[iHero + 1, jHero] == Cell.Empty || field[iHero + 1, jHero] == Cell.Portal)
            {
                iHero++;
            }

            break;

        case ConsoleKey.R:
            heroInAdventure = false;
            break;
    }
}

bool IsHeroInPortal(Cell[,] field, int iHero, int jHero)
{
    return field[iHero, jHero] == Cell.Portal;
}

void GotoNextLevel(ref int currentLevel, ref int currentWallPercent, ref bool heroInAdventure)
{
    currentLevel++;
    currentWallPercent += 1;
    heroInAdventure = false;
}


//-------

int currentLevel = 1;
int currentWallPercent = (int)Constants.WallPercent;

while (true)
{
    int rows = InitRows();
    int cols = InitCols();

    Cell[,] field = CreateField(rows, cols);
    InitField(field);

    int iHero = InitIHero();
    int jHero = InitJHero();

    SetPortal(field, iHero, jHero);
    SetWalls(field, currentWallPercent, iHero, jHero);

    bool heroInAdventure = true;

    while (heroInAdventure)
    {
        ResetConsole();

        PrintGameInfo(currentLevel, currentWallPercent);
        PrintField(field, iHero, jHero);

        MoveHero(field, ref iHero, ref jHero, ref heroInAdventure);

        if (IsHeroInPortal(field, iHero, jHero))
        {
            GotoNextLevel(ref currentLevel, ref currentWallPercent, ref heroInAdventure);
        }
    }
}