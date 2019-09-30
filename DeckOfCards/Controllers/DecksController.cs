using DeckOfCards.Data;
using DeckOfCards.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DeckOfCards.Controllers
{
    [RoutePrefix("api/decks")]
    public class DecksController : ApiController
    {
        private IDeckRepository _repository;

        public DecksController(IDeckRepository repository)
        {
            _repository = repository;
        }

        async public Task<ShortDeckInfo> Post(DeckCreate creation)
        {
            int creationCount = creation.Count.HasValue ? creation.Count.Value : 1;
            Deck deck = await _repository.CreateNewShuffledDeckAsync(creationCount);
            ShortDeckInfo deckInfo = new ShortDeckInfo
            {
                DeckId = deck.DeckId,
                Remaining = deck.Cards.Where(x => !x.Drawn).Count()
            };
            return deckInfo;
        }

        [Route("{deckId}/cards")]
        async public Task<CardDrawnResponse> Delete(string deckId, CardDrawRequest request)
        {
            int drawCount = request.Count.HasValue ? request.Count.Value : 1;
            Deck deck = await _repository.DrawCardsAsync(deckId, drawCount);
            List<CardInfo> cards = deck.Cards
              .Where(x => x.Drawn)
              .Reverse()
              .Take(drawCount)
              .Reverse()
              .Select(x => new CardInfo { Suit = x.Suit, Value = x.Value, Code = x.Code })
              .ToList();
            return new CardDrawnResponse
            {
                DeckId = deckId,
                Remaining = deck.Cards.Where(x => !x.Drawn).Count(),
                Removed = cards,
            };
        }

        [Route("{deckId}/piles/{pileName}")]
        async public Task<AddCardResponse> Patch(string deckId, string pileName, AddPileRequest request)
        {
            Deck deck = await _repository.GetDeck(deckId);
            Pile pile = await _repository.GetPile(deckId, pileName);
            //[7S, QH] 
            List<string> cardCodes = request.CardCodes;
            List<Card> cards = new List<Card>();
            //List<Card> card = new List<Card>();

            foreach(var cardCode in cardCodes)
            {
                Card card = await _repository.GetCards(deckId, cardCode);
                cards.Add(card);
            }

            pile = await _repository.AddToPile(deckId, pileName, request.CardCodes);
            deck = await _repository.GetDeck(deckId);

            Dictionary<string, ShortPileInfo> piles = new Dictionary<string, ShortPileInfo>();
            
            foreach (var _pile in deck.Piles)
            {
                ShortPileInfo info = new ShortPileInfo();
                info.Remaining = _pile.Cards.Count;
                piles.Add(_pile.Name, info);
            }

            return new AddCardResponse
            {
                DeckId = deckId,
                Remaining = deck.Cards.Where(x => !x.Drawn).Count(),
                Piles = piles
            };
        }

        [Route("deckId/pile/pileName/shuffle")]
        async public Task<bool> Post(string deckId, string pileName)
        {
            return await _repository.Shuffle(deckId, pileName);
        }

    }
}