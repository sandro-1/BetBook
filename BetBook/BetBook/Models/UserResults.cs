using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetBook.Models
{
    public class UserResults
    {
        [JsonProperty("betsWon")]
        public int BetsWon { get; set; }

        [JsonProperty("betsLost")]
        public int BetsLost { get; set; }

        [JsonProperty("cashWon")]
        public double CashWon { get; set; }

        [JsonProperty("cashLost")]
        public double CashLost { get; set; }

        [JsonProperty("cashWonCollected")]
        public double CashWonCollected { get; set; }

        [JsonProperty("cashLostPaid")]
        public double CashLostPaid { get; set; }

        [JsonProperty("collectionRatio")]
        public double CollectionRatio { get; set; }

        [JsonProperty("credibilityRatio")]
        public double CredibilityRatio { get; set; }


    }
}
