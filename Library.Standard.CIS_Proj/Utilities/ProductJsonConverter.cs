using Library.GUI_App.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Standard.CIS_Proj.Utilities
{
    public class ProductJsonConverter : JsonCreationConverter<Product>
    {
        protected override Product Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");

            if (jObject["weight"] != null || jObject["Weight"] != null)
            {
                return new Library.GUI_App.Models.ProductByWeight();
            }
            else if (jObject["quantity"] != null || jObject["Quantity"] != null)
            {
                return new Library.GUI_App.Models.ProductByQuantity();
            }
            else
            {
                return new Library.GUI_App.Models.Product();
            }

            //if (jObject["deadline"] != null || jObject["Deadline"] != null)
            //{
            //    return new TaskManagement.Models.ToDo();
            //}
            //else if (jObject["attendees"] != null || jObject["Attendees"] != null)
            //{
            //    return new Appointment();
            //}
            //else
            //{
            //    return new Item();
            //}
        }
    }
}
