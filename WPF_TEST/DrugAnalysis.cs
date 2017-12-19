using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TEST
{
    public class DrugAnalysis
    {
        public int ID { get; set; }
        public string DrugName { get; set; }
        public int AmountToBuy { get; set; }
        public int AmountToHave { get; set; }
        public DrugAnalysis() { }
        public DrugAnalysis(int id, string name, int amount, int amountToHave) {
            ID = id;
            DrugName = name;
            AmountToBuy = amount;
            AmountToHave = amountToHave;
        }
    }
}
