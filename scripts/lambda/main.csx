
//query che usa un parametro di tipo DateTime e che ritorna un risultato di tipo bool
Func<DateTime, bool> canDrive = dob =>
{        //dob vale 25/12/2000
    return dob.AddYears(18) <= DateTime.Today;
};

//invocazione variabile canDrive
DateTime dob = new DateTime(2000, 12, 25);

//raccolgo il valore restituito in una variabile
bool result = canDrive(dob); //in questo modo richiamo la funzione scritta sopra

Console.WriteLine(result);  //true poiché una persona nata il 25/12/2000 é maggiorenne (>= 18)


//------------------------------------------------------------------

Action<DateTime> printDate = date => Console.WriteLine(date);

//invocazione lambda
DateTime date = DateTime.Today;
printDate(date);

//-----------------ESERCIZI------------------------------------------
//una lambda che prende due parametri stringa (nome, cognome) e restituisce la loro concatenazione
Func<string, string, string> concatFirstAndLastName = (nome, cognome) =>
{
    string concat = nome + " " + cognome;
    return concat;
};

string nome = "Ivan"; string cognome = "Grassi";

string concat = concatFirstAndLastName(nome, cognome);
Console.WriteLine(concat);

//-----------------------------------------------------------------
//una lambda che prende tre parametri interi (3 numeri) e restituisce il maggiore dei tre
Func<int, int, int, int> getMaximum = (uno, due, tre) =>
{
    var numbers = new List<int> { uno, due, tre };
    int maxNumber = numbers.Max();
    return maxNumber;
};

int uno = 1; int due = 2; int tre = 3;
int maxnum = getMaximum(uno, due, tre);
Console.WriteLine(maxnum);
//---------------------------------------------------------------
//una lambda che prende due parametri DateTime e non restituisce nulla, ma stampa la minore delle due date in console
Action<DateTime, DateTime> printLowerDate = (date1, date2) => Console.WriteLine(date1);

DateTime date1 = new DateTime(2000, 12, 12);
DateTime date2 = new DateTime(2000, 12, 25);
if (date1 < date2)
{
    DateTime minDate = date1;
    printDate(minDate);
}
else
{
    DateTime minDate = date2;
    printDate(minDate);
}

