//поле - двумерный массив размера N на M
//портал - объект, пренадлежит полю
//стены - объекты, прендалежат полю, по краям поля сделать стены
//герой - объект, НЕ пренадлежит полю, рисуется поверх полю

//1) генерация поля и стен на нём
//2) генерация портала в поле (перандомировать поле если мы оказались заперты)
//3) передвижение героя по полю
//4) вход героя в портал и переход к шагу 1

using System.Security.Cryptography;
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

void PrintGameObjects(Cell[,] field, int iHero, int jHero, int[,] dogs)
{
    ResetConsole();
    
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
            else if (IsStateOnDog(dogs, i, j))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write((char)Constants.DogSkin);
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

int[,] CreateDogs(int countDogs)
{
    int[,] dogs = new int[countDogs, 2];

    for (int i = 0; i < dogs.GetLength(0); i++)
    {
        for (int j = 0; j < dogs.GetLength(1); j++)
        {
            dogs[i, j] = -1;
        }
    }

    return dogs;
}

bool IsStateOnDog(int[,] dogs, int iObject, int jObject)
{
    int countDogs = dogs.GetLength(0);

    for (int i = 0; i < countDogs; i++)
    {
        if (dogs[i, 0] == iObject && dogs[i, 1] == jObject)
        {
            return true;
        }
    }

    return false;
}

void SetDogs(int[,] dogs, Cell[,] field, int iHero, int jHero)
{
    Random random = new Random();
    int rows = field.GetLength(0);
    int cols = field.GetLength(1);

    int countDogs = dogs.GetLength(0);

    for (int i = 0; i < countDogs; i++)
    {
        int iDog, jDog;
        do
        {
            iDog = random.Next(1, rows - 1);
            jDog = random.Next(1, cols - 1);
        } while (iDog == iHero && jDog == jHero
                 || field[iDog, jDog] == Cell.Portal
                 || field[iDog, jDog] == Cell.Wall
                 || IsStateOnDog(dogs, iDog, jDog));

        dogs[i, 0] = iDog;
        dogs[i, 1] = jDog;
    }
}

void MoveDogs(int[,] dogs, Cell[,] field)
{
    Random random = new Random();
    int countDogs = dogs.GetLength(0);

    for (int i = 0; i < countDogs; i++)
    {
        int direction = random.Next(1, 4 + 1);

        int iDog = dogs[i, 0];
        int jDog = dogs[i, 1];

        switch (direction)
        {
            case (int)Constants.DirectionUp:
                if (field[iDog - 1, jDog] == Cell.Empty)
                {
                    iDog--;
                }

                break;
            case (int)Constants.DirectionDown:
                if (field[iDog + 1, jDog] == Cell.Empty)
                {
                    iDog++;
                }

                break;
            case (int)Constants.DirectionLeft:
                if (field[iDog, jDog - 1] == Cell.Empty)
                {
                    jDog--;
                }

                break;
            case (int)Constants.DirectionRight:
                if (field[iDog, jDog + 1] == Cell.Empty)
                {
                    jDog++;
                }

                break;
        }

        dogs[i, 0] = iDog;
        dogs[i, 1] = jDog;
    }
}

void GameOver(ref bool heroInAdventure, ref bool runGame)
{
    heroInAdventure = false;
    runGame = false;
    Console.WriteLine("Игра окончена!");
}
//-------

int currentLevel = 1;
int currentWallPercent = (int)Constants.WallPercent;
bool runGame = true;

while (runGame)
{
    int rows = InitRows();
    int cols = InitCols();

    Cell[,] field = CreateField(rows, cols);
    InitField(field);

    int iHero = InitIHero();
    int jHero = InitJHero();

    SetPortal(field, iHero, jHero);
    SetWalls(field, currentWallPercent, iHero, jHero);

    int[,] dogs = CreateDogs(10);
    SetDogs(dogs, field, iHero, jHero);
    
    //PrintGameInfo(currentLevel, currentWallPercent);
    PrintGameObjects(field, iHero, jHero, dogs);
    
    bool heroInAdventure = true;
    while (heroInAdventure)
    {
        MoveHero(field, ref iHero, ref jHero, ref heroInAdventure);
        MoveDogs(dogs, field);

        //PrintGameInfo(currentLevel, currentWallPercent);
        PrintGameObjects(field, iHero, jHero, dogs);

        if (IsHeroInPortal(field, iHero, jHero))
        {
            GotoNextLevel(ref currentLevel, ref currentWallPercent, ref heroInAdventure);
        }

        if (IsStateOnDog(dogs, iHero, jHero))
        {
            GameOver(ref heroInAdventure, ref runGame);
        }
    }
}