using System;

namespace Naanayam
{
    public class Transaction
    {
        public uint ID { get; set; }

        public uint Account { get; set; }

        public DateTime Timestamp { get; set; }

        public TransactionType Type { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }
    }
}