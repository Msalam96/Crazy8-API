using DeckOfCards.Data;
using System.Collections.Generic;

namespace DeckOfCards.Models
{
    public class CardDrawnResponse
    {
        public CardDrawnResponse()
        {
            Removed = new List<CardInfo>();
        }

        public string DeckId { get; set; }

        public int Remaining { get; set; }

        public List<CardInfo> Removed { get; set; }
    }
}