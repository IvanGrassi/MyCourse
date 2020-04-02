
class Apple
{
    public string Color { get; set; }
    public int Weight { get; set; }     //in grammi

}

List<Apple> apples = new List<Apple> {
    new Apple {Color = "Red", Weight = 180},
    new Apple {Color = "Green", Weight = 195},
    new Apple {Color = "Red", Weight = 190},
    new Apple {Color = "Green", Weight = 185}
};

//lambda expression che accetta un solo parametro di tipo Apple e restituisce un bool

//Func<Apple, bool> takeRedApples = apple => apple.Color == "Red"; //non si usa solitamente
//apples.Where(takeRedApples);
//qui ci sarà l'elenco comprendente solo i risultati accettati
//IEnumerable<Apple> redApples = apples.Where(takeRedApples);

IEnumerable<Apple> redApples = apples.Where(apple => apple.Color == "Red");
Console.WriteLine();

//di tutte le mele, vai a prendere quelle di colore rosso, e di quelle stampami il peso
IEnumerable<int> weightOfRedApples = apples
                            .Where(apple => apple.Color == "Red")
                            .Select(apple => apple.Weight);

double count = weightOfRedApples.Count();
double average = weightOfRedApples.Average();
int firstElement = weightOfRedApples.First();
int lastElement = weightOfRedApples.Last();

Console.WriteLine("Total elements: " + count);
Console.WriteLine("First element: " + firstElement);
Console.WriteLine("Last element: " + lastElement);
Console.WriteLine("Average: " + average);

//---------------------------------------------------------------
//Qual'é il peso minimo?
IEnumerable<int> minimumWeight = apples.
Select(apple => apples.Min(apple => apple.Weight));

int min = minimumWeight.Min();
Console.WriteLine("Minimum weight: " + min);

//---------------------------------------------------------------
//Di che colore é la mela che pesa 190 grammi?

IEnumerable<string> countApples = apples
.Where(apple => apple.Color == "Red")
.Select(apple => apple.Color);

int counts = countApples.Count();
Console.WriteLine("Totale mele rosse" + counts);

//----------------------------------------------------------------
//Quanto pesano in totale le mele verdi?
IEnumerable<int> sumGreenApples = apples
.Where(apple => apple.Color == "Green")
.Select(apple => apple.Weight);

int totalWeight = sumGreenApples.Sum();
Console.WriteLine("Peso totale: " + totalWeight);