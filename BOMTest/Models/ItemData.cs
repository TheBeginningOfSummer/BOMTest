using System;
using System.Collections.Generic;
using System.Text;

namespace BOMTest.Models
{
    public class ItemData
    {
        public string Name { get; set; }
        public int Amount { get; set; }

        public ItemData(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
