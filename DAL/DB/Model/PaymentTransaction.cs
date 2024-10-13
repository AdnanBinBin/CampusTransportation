using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DB.Model
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public PaymentMethod Method { get; set; }
        public bool IsRefund { get; set; } = false;
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        PrepaidBalance
    }
}
