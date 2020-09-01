using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetBook.Models
{
    public class UserData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("userResults")]
        public UserResults UserResults { get; set; }

        [JsonProperty("betList")]
        public List<TermSheet> BetList { get; set; }
    }
}
