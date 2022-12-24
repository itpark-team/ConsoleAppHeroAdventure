//поле - двумерный массив размера N на M
//портал - объект, пренадлежит полю
//стены - объекты, прендалежат полю, по краям поля сделать стены
//герой - объект, НЕ пренадлежит полю, рисуется поверх полю

//1) генерация поля и стен на нём
//2) генерация портала в поле (перандомировать поле если мы оказались заперты)
//3) передвижение героя по полю
//4) вход героя в портал и переход к шагу 1

using ConsoleAppHeroAdventure;

Random random = new Random();

int currentLevel = 1;
int rows, cols;
Cell[,] field;

int iHero, jHero;

bool heroInAdventure;

int currentWallPercent = (int)Constants.WallPercent;


// int[,] dogs = new int[,]
// {
//     { 0, 0 },
//     { 0, 0 }
// };
//
// int iDog, jDog;

while (true)
{
    rows = random.Next((int)Constants.MinRows, (int)Constants.MaxRows + 1);
    cols = random.Next((int)Constants.MinCols, (int)Constants.MaxCols + 1);

    field = new Cell[rows, cols];

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

    iHero = (int)Constants.StartIHero;
    jHero = (int)Constants.StartJHero;

    int iPortal, jPortal;
    do
    {
        iPortal = random.Next(1, rows - 1);
        jPortal = random.Next(1, cols - 1);
    } while (iPortal == iHero && jPortal == jHero);

    field[iPortal, jPortal] = Cell.Portal;

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

    heroInAdventure = true;
    while (heroInAdventure)
    {
        Console.Clear();

        Console.ResetColor();

        Console.WriteLine($"Current Level = {currentLevel}");
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

        if (field[iHero, jHero] == Cell.Portal)
        {
            currentLevel++;
            currentWallPercent += 5;
            heroInAdventure = false;
        }
    }
}