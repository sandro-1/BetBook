using BetBook.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetBook.Models
{
    public class TermSheet
    {
        [JsonProperty("betId")]
        public string BetId { get; set; }

        [JsonProperty("myUsername")]
        public string MyUsername { get; set; }

        [JsonProperty("opponentsUsername")]
        public string OpponentsUsername { get; set; }

        [JsonProperty("dateTimeOffered")]
        public string DateTimeOffered { get; set; }

        [JsonProperty("dateTimeAccepted")]
        public string DateTimeAccepted { get; set; }

        [JsonProperty("dateTimeBetSettled")]
        public string DateTimeBetSettled { get; set; }

        [JsonProperty("dateTimePaid")]  
        public string DateTimePaid { get; set; }

        [JsonProperty("cashBetAmount")]
        public string CashBetAmount { get; set; }

        [JsonProperty("nonCashBet")]
        public string NonCashBet { get; set; }

        [JsonProperty("betTerms")]
        public string BetTerms { get; set; }

        [JsonProperty("dateTimeOfferExpiration")]
        public string DateTimeOfferExpiration { get; set; }

        [JsonProperty("setBetCloseDate")]
        public bool SetBetCloseDate { get; set; }

        [JsonProperty("dateTimeBetClose")]
        public string DateTimeBetClose { get; set; }

        [JsonProperty("betPhase")]
        public string BetPhase { get; set; }

        [JsonProperty("betWon")]
        public bool BetWon { get; set; }

        [JsonProperty("betPaid")]
        public bool BetPaid { get; set; }

        [JsonProperty("initiatedRequest")]
        public bool InitiatedRequest { get; set; }

        [JsonProperty("requestMode")]
        public bool RequestMode { get; set; }

        [JsonProperty("requestResponse")]
        public string RequestResponse { get; set; }
    }
}
