using System;

namespace DAL.DB.Model
{
    public class Card
    {
        public int Id { get; set; } // Identifiant de la carte

        public PaymentMethod PaymentMethod { get; set; }
       
    }
}
