﻿using AbstractClasses;


namespace BlackjackMechanics.Cards
{
    public class DeckOfCards
    {
        public List<ACard> Deck;
        private int CountDecksInOne;

        public DeckOfCards(int data)
        {
            CountDecksInOne = data;
            Deck = new List<ACard>();

            CreateCardNewDeckOfCard();
        }

        private void CreateCardNewDeckOfCard()
        {
            for (int deck = 0; deck < CountDecksInOne; deck++)
            {
                for (int suit = 0; suit < 4; suit++)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        Deck.Add(new NumberCard((CardNames)i, (CardSuits)suit));
                    }

                    for (int i = 9; i < 12; i++)
                    {
                        Deck.Add(new FaceCard((CardNames)i, (CardSuits)suit));
                    }

                    Deck.Add(new AceCard((CardSuits)suit));
                }
            }
        }

        private void RemoveCard(ACard card)
        {
            this.Deck.Remove(card);
        }

        public ACard GetOneCard(AParticipant player)
        {
            ACard card = this.Deck.First();
            if (card.CardName == CardNames.Ace)
                ((AceCard)card).CheckIsMoreThenTwentyOne(player.CardsInHand);
            RemoveCard(card);
            return card;
        }

        public void ShuffleDeck()
        {
            Random rnd = new Random();
            Deck = Deck.OrderBy(x => rnd.Next()).ToList();
        }

        public void ResetDeckOfCards()
        {
            Deck.Clear();
            CreateCardNewDeckOfCard();
        }
    }
}
