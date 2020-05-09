using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace emarket.Models
{
    public class Viewmodel
    {
        public int id_p { get; set; }
        public string name_p { get; set; }
        public Nullable<int> price { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public Nullable<int> category_id { get; set; }
        public int id_c { get; set; }
        public string name_c { get; set; }
        public Nullable<int> number_of_products { get; set; }

    }
}