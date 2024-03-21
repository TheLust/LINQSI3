using LINQSI3.Model;
using LINQSI3.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSI3
{
    internal class Program
    {
        private static ProductRepository productRepository = 
            new ProductRepository("Data Source=CEDINTL886;Initial Catalog=Market;Integrated Security=True");

        static void Main(string[] args)
        {
            Product product = new Product()
            {
                Id = 10,
                Name = "IDK",
                Price = 10,
                Category = "Nush"
            };
            productRepository.Update(product);

            Console.WriteLine(productRepository.FindAll().Count);
            Console.WriteLine(productRepository.FindById(1));
            Console.ReadKey();
        }
    }
}
