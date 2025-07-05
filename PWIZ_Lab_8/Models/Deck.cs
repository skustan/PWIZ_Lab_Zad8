using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWIZ_Lab_8.Models
{
    class Deck
    {
        private List<Card> _cards;
        private Random _rng = new();

        public Deck()
        {
            _cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (int val = 1; val <= 13; val++)
                    _cards.Add(new Card(suit, val));
            }

            Shuffle();

        }

        public void Shuffle() => _cards = _cards.OrderBy(c => _rng.Next()).ToList();

        public Card Draw()
        {
            if (_cards.Count == 0) return null;
            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public int Count => _cards.Count;

        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();


    }
}
