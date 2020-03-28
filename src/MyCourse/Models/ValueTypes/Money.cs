using MyCourse.Models.Enums;

namespace MyCourse.Models.ValueTypes
{
    public class Money
    {
        public Money() : this(Currency.EUR, 0.00m)
        {

        }

        //la valuta scelta di default é l'euro (EUR)

        public Money(Currency currency, decimal amount)
        {
            Amount = amount;
            Currency = currency;
        }

        private decimal amount = 0;

        public decimal Amount       //valore di vendita del corso - per tutti gli importi usare il decimal
        {
            get
            {
                return amount;
            }
            set
            {
                if (value < 0)      //evita l'esistenza di valori negativi
                {
                    throw new System.InvalidOperationException("The amount cannot be negative");
                }
                amount = value;
            }
        }

        public Currency Currency    //rappresenta la valuta utilizzata, definita in una classe enum
        {
            get; set;
        }

        public override bool Equals(object obj)     //confronto fra due tipi di oggetti Money differenti
        {
            var money = obj as Money;
            return money != null && Amount == money.Amount && Currency == money.Currency;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Currency} {Amount:#.00}";
        }
    }
}