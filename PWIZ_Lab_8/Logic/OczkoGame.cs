using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PWIZ_Lab_8.Models;

namespace PWIZ_Lab_8.Logic
{
    class OczkoGame
    {
        private Deck _deck;
        public List<Card> PlayerHand { get; private set; }
        public List<Card> DealerHand { get; private set; }

        public OczkoGame()
        {
            _deck = new Deck();
            PlayerHand = new List<Card>();
            DealerHand = new List<Card>();
        }

        public Card PlayerDraw()
        {
            var card = _deck.Draw();
            if (card != null)
                PlayerHand.Add(card);
            return card;
        }

        public Card DealerDraw()
        {
            var card = _deck.Draw();
            if (card != null)
                DealerHand.Add(card);
            return card;
        }

        public void Restart()
        {
            _deck = new Deck();
            PlayerHand.Clear();
            DealerHand.Clear();
        }

        public int CalculatePoints(List<Card> hand)
        {
            int total = 0;
            int aces = 0;

            foreach (var card in hand)
            {
                if (card.Value > 10) total += 10;
                else if (card.Value == 1)
                {
                    total += 11;
                    aces++;
                }
                else total += card.Value;
            }

            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }

            return total;
        }

        public string CheckWinner()
        {
            int player = CalculatePoints(PlayerHand);
            int dealer = CalculatePoints(DealerHand);

            if (player > 21) return "Przegrana";
            if (dealer > 21) return "Wygrana";
            if (player > dealer) return "Wygrana";
            if (player < dealer) return "Przegrana";
            return "Remis";
        }

        public void DealerPlay()
        {
            while (CalculatePoints(DealerHand) < 17)
            {
                DealerDraw();
            }
        }
    }

}
