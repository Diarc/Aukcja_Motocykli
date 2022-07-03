using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aukcja_Motocykli.Models.ViewModels
{
    public class BikeViewModel
    {
        public Bike Bike { get; set; }
        public IEnumerable<Make> Makes { get; set; }
        public IEnumerable<Model> Models { get; set; }
        public IEnumerable<Currency> Currencies { get; set; }

        private List<Currency> Clist = new List<Currency>();

        private List<Currency> CreateList()
        {
            Clist.Add(new Currency("USD", "USD"));
            Clist.Add(new Currency("PLZ", "PLZ"));
            Clist.Add(new Currency("EUR", "EUR"));
            return Clist;
        }

        public BikeViewModel()
        {
            Currencies = CreateList();
        }
    }

    public class Currency
    {
        public String Id { get; set; }
        public String Name { get; set; }

        public Currency(String id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
