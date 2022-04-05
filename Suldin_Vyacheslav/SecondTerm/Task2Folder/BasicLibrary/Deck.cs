﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLibrary
{
    public class Deck
    {
        private Card[] cards = new Card[52];

        public Deck()
        {
            for (int i = 0; i < 52; i++)
            {
                cards[i] = new Card((CardSuit)(i % 4),(CardRank)(i % 13));
            }
        }
        
        public Card[] GetCards()
        {
            Card[] cardsCopy = new Card[cards.Length];
            Array.Copy(cards,cardsCopy, cards.Length);
            return cardsCopy;
        }
        public void Mix()
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            for (int i = this.cards.Length - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                Card temporaryCard = this.cards[j];
                this.cards[j] = this.cards[i];
                this.cards[i] = temporaryCard;
            }
        }
    }
}
