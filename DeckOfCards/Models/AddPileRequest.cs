using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckOfCards.Models
{
    public class AddPileRequest
    {
        public AddPileRequest()
        {
            CardCodes = new List<string>();
        }
        public string Op { get; set; }
        public string Path { get; set; }
        public List<string> CardCodes { get; set; }
    }
}