using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OrderTakingService.Lib;

namespace OrderTakingService.Models
{
    public class Order
    {
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
        public List<FixedDeal> FixedDeals { get; set; } = new List<FixedDeal>();
        public List<OnSpotDeal> OnSpotDeals { get; set; } = new List<OnSpotDeal>();
        public Customer Customer { get; set; } = new Customer();
        [XmlAttribute]
        public string Id { get; set; } = string.Empty;
        [XmlAttribute]
        public string OrderNo { get; set; } = string.Empty;
        public Waiter Waiter { get; set; } = new Waiter();
        public Table Table { get; set; } = new Table();
        [XmlAttribute]
        public string UserId { get; set; } = string.Empty;
        [XmlAttribute]
        public string OrderType { get; set; } = string.Empty;
        [XmlAttribute]
        public int Covers { get; set; } = 0;
        [XmlAttribute]
        public string Time { get; set; } = string.Empty;
        [XmlAttribute]
        public string Date { get; set; } = string.Empty;
        [XmlAttribute]
        public string DeviceId { get; set; } = string.Empty;
        [XmlAttribute]
        public string TotalTax { get; set; } = string.Empty;
        [XmlAttribute]
        public string TotalAmount { get; set; } = string.Empty;
        [XmlAttribute]
        public string SubTotal { get; set; } = string.Empty;

        public Order Create()
        {
            TotalTax = totalTax.ToString();
            TotalAmount = totalAmount.ToString();
            SubTotal = subTotal.ToString();
            return this;
        }

        public double totalTax
        {
            get
            {
                double amount = 0.00;
                foreach (var item in Items)
                {
                    amount += Convert.ToDouble(item.Amount);
                }

                foreach (var deal in FixedDeals)
                {
                    amount += Convert.ToDouble(deal.Amount);
                }

                foreach (var deal in OnSpotDeals)
                {
                    amount += Convert.ToDouble(deal.Amount);
                }
                amount *= Tax;
                return amount > 0 ? double.Parse(amount.ToString("#.##")) : 0.0;
            }
        }

        public double Tax
        {
            get
            {
                return Database.GetDoubleData("select dbo.udfGetTax()");
            }
        }

        public double totalAmount
        {
            get
            {
                double taxAmount = subTotal * Tax;
                double amount = subTotal + taxAmount;
                return amount > 0 ? double.Parse(amount.ToString("#.##")) : 0.0;
            }
        }

        public double subTotal
        {
            get
            {
                double amount = 0.00;
                foreach (var item in Items)
                {
                    amount += Convert.ToDouble(item.Price) * Convert.ToDouble(item.Quantity);
                }

                foreach (var item in FixedDeals)
                {
                    amount += Convert.ToDouble(item.Price) * Convert.ToDouble(item.Quantity);
                }

                foreach (var item in OnSpotDeals)
                {
                    amount += Convert.ToDouble(item.Price) * Convert.ToDouble(item.Quantity);
                }

                return amount > 0 ? double.Parse(amount.ToString("#.##")) : 0.0;
            }
        }

    }
}