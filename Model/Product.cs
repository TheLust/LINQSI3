using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSI3.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Price: {Price}, Category: {Category}";
        }
    }
}
