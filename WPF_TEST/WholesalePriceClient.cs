using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TEST
{
    public class WholesalePriceClient
    {
        public int WholesalePriceID;
        public int Minimal_amount_of_product;
        public string Price;
        public int DrugID;

        public WholesalePriceClient() { }
        public WholesalePriceClient(int id, int amount, string price, int drugID) {
            WholesalePriceID = id;
            Minimal_amount_of_product = amount;
            Price = price;
            DrugID = drugID;
        }
    }
}
