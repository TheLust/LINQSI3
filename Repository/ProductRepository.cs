using LINQSI3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSI3.Repository
{
    internal class ProductRepository : Repository<Product>
    {
        public ProductRepository(string connection) : base(connection)
        {
        }
    }
}
