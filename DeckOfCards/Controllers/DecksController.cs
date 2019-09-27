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
    }
}