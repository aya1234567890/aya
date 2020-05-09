using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using emarket.Models;

namespace emarket.ViewModel
{
    public class ProductViewmodel
    {
        public Product Product { get; set; }
        public IEnumerable <Category> Category { get; set; }
    }
}