using System.Threading.Tasks;

namespace DeckOfCards.Data
{
    public interface IDeckRepository
    {
        Task<Deck> CreateNewShuffledDeckAsync(int deckCount);
        Task<Deck> DrawCardsAsync(string deckId, int numberToDraw);
        Task<Deck> GetDeck(string deckId);
        Task<Pile> GetPile(string deckId, string pileName);
        Task<Card> GetCards(string deckId, string value);
    }
}