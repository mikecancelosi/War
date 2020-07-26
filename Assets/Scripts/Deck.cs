using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deck handles instantiation and keeps track of a set of cards
/// </summary>
public class Deck
{

    public static int NUM_CARDS_IN_DECK => Enum.GetNames(typeof(CardValue)).Length * NUM_SUITS;
    public static int NUM_SUITS => Enum.GetNames(typeof(CardSuit)).Length;

    /// <summary>
    /// Local reference to the cards in this deck
    /// </summary>
    private List<Card> cards;    

    public  Deck()
    {
        cards = new List<Card>();
    }

    /// <summary>
    /// Create a fresh deck of 52 unique cards
    /// </summary>
    /// <returns>Fresh deck of 52 shuffled cards</returns>
    public static Deck GetNewDeck()
    {
        Deck deck = new Deck();
        deck.cards = new List<Card>();
        for (int i = 0; i < NUM_CARDS_IN_DECK; i++)
        {
            Array valueEnumValues = Enum.GetValues(typeof(CardValue));
            Array suitEnumValues = Enum.GetValues(typeof(CardSuit));
            int valueIndex = i / NUM_SUITS;
            int suitIndex = i % NUM_SUITS;
            CardValue value = (CardValue)valueEnumValues.GetValue(valueIndex);
            CardSuit suit = (CardSuit)suitEnumValues.GetValue(suitIndex);

            deck.cards.Add(new Card(value,suit));
        }
        deck.Shuffle();
        return deck;
    }

    /// <summary>
    /// Using the Fisher-Yates Shuffle algorithm, shuffles the current deck instance
    /// </summary>
    public void Shuffle()
    {
        System.Random random = new System.Random();
        for (int i = 0; i < (NUM_CARDS_IN_DECK - 1); i++)
        {
            int r = i + random.Next(NUM_CARDS_IN_DECK - i);
            Card t = cards[r];
            cards[r] = cards[i];
            cards[i] = t;
        }
    }

    /// <summary>
    /// Operates like a pop. Returns the top cards, removes from deck 
    /// </summary>
    /// <returns>Next card on top of the deck</returns>
    public Card GetNextCard()
    {
        Card card = new Card(cards[0].Value,cards[0].Suit);
        cards.RemoveAt(0);
        return card;
    }

    /// <summary>
    /// Adds given card to the bottom of the deck
    /// </summary>
    /// <param name="card">Card to add to the bottom of the deck</param>
    public void AddCardToBottom(Card card)
    {
        cards.Add(card);
    }

    /// <summary>
    /// Gets current size of the deck instance
    /// </summary>
    /// <returns>Amount of cards in the deck instance</returns>
    public int GetDeckSize()
    {
        return cards.Count;
    }
       
}
/// <summary>
/// Class used to represent a card in a deck
/// </summary>
public class Card
{
    /// <summary>
    /// This card's value. E.g "Ace","Two"
    /// </summary>
    public CardValue Value;
    /// <summary>
    /// Suit of the card. E.g "Diamond"
    /// </summary>
    public CardSuit Suit;

    public Card(CardValue value,CardSuit suit)
    {
        Value = value;
        Suit = suit;
    }    

    /// <summary>
    /// Given an opponent, return outcome in faceup
    /// </summary>
    /// <param name="opponent">Card to compare to</param>
    /// <returns>Outcome of the battle</returns>
    public MatchupOutcomes Battle(Card opponent)
    {
        int thisCardValue = (int)Value;
        int opponentCardValue = (int)opponent.Value;

        if(thisCardValue == opponentCardValue)
        {
            return MatchupOutcomes.Tie;
        }
        else
        {
            return thisCardValue > opponentCardValue ? MatchupOutcomes.Win : MatchupOutcomes.Lose;
        }

    }   
}

/// <summary>
/// Used to represent outcomes of any matchup
/// </summary>
public enum MatchupOutcomes
{
    Win,
    Lose,
    Tie
}

/// <summary>
/// All suits that can be used in WAR
/// </summary>
public enum CardSuit
{
    Hearts,
    Diamonds,
    Spades,
    Clubs
}

/// <summary>
/// All values that can be used in WAR
/// </summary>
public enum CardValue
{
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}