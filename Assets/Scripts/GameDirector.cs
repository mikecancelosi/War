using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of the game, updating ui as necessary
/// </summary>
public class GameDirector : MonoBehaviour
{
    /// <summary>
    /// Current reference to the opponents deck
    /// </summary>
    public Deck OpponentDeck;
    /// <summary>
    /// Current reference to the user's deck.
    /// </summary>
    public Deck UserDeck;
    public Transform UserCard;
    public Transform OpponentCard;
    public Transform UserTieParent;
    public Transform OpponentTieParent;
    public TextMeshProUGUI OpponentDeckSizeText;
    public TextMeshProUGUI UserDeckSizeText;
    public TextMeshProUGUI WinConditionText;
    public TextMeshProUGUI BattleResultText;

    public Sprite SpadesIcon;
    public Sprite ClubsIcon;
    public Sprite DiamondsIcon;
    public Sprite HeartsIcon;


    public void Awake()
    {
        StartGame();
    }

    /// <summary>
    /// Start the game by instantiating the deck and passing out cards to user and opponent
    /// </summary>
    public void StartGame()
    {
        Deck startDeck = Deck.GetNewDeck();
        OpponentDeck = new Deck();
        UserDeck = new Deck();
        for (int i = 0; i < Deck.NUM_CARDS_IN_DECK; i++)
        {
            if (i % 2 == 0)
            {
                OpponentDeck.AddCardToBottom(startDeck.GetNextCard());
            }
            else
            {
                UserDeck.AddCardToBottom(startDeck.GetNextCard());
            }
        }
        OpponentDeckSizeText.text = OpponentDeck.GetDeckSize().ToString();
        UserDeckSizeText.text = UserDeck.GetDeckSize().ToString();

    }
    
    /// <summary>
    /// When user clicks the deck, we need to execute the next turn.
    /// </summary>
    public void OnUserDeckClick()
    {
        //Reset visuals
        UserCard.gameObject.SetActive(true);
        OpponentCard.gameObject.SetActive(true);
        UserTieParent.gameObject.SetActive(false);
        OpponentTieParent.gameObject.SetActive(false);
        BattleResultText.gameObject.SetActive(false);
        //Deal next round
        Card userCard = UserDeck.GetNextCard();
        Card opponentCard = OpponentDeck.GetNextCard();
        //Update visuals
        OpponentDeckSizeText.text = OpponentDeck.GetDeckSize().ToString();
        UserDeckSizeText.text = UserDeck.GetDeckSize().ToString();
        SetCardUI(UserCard.gameObject, userCard);
        SetCardUI(OpponentCard.gameObject, opponentCard);
        OpponentDeckSizeText.text = OpponentDeck.GetDeckSize().ToString();
        UserDeckSizeText.text = UserDeck.GetDeckSize().ToString();

        //Battle
        MatchupOutcomes outcome = userCard.Battle(opponentCard);
        if (outcome == MatchupOutcomes.Tie)
        {
            HandleTie(new List<Card>() { userCard, opponentCard });
        }
        else
        {
            BattleResultText.gameObject.SetActive(true);

            if (outcome == MatchupOutcomes.Win)
            {
                BattleResultText.text = "You won this one!";
                UserDeck.AddCardToBottom(userCard);
                UserDeck.AddCardToBottom(opponentCard);
            }
            else
            {
                BattleResultText.text = "You lost this one!";
                OpponentDeck.AddCardToBottom(userCard);
                OpponentDeck.AddCardToBottom(opponentCard);
            }

        }
        OpponentDeckSizeText.text = OpponentDeck.GetDeckSize().ToString();
        UserDeckSizeText.text = UserDeck.GetDeckSize().ToString();

        if(UserDeck.GetDeckSize() == 0)
        {
            HandleEndGame(false);
        }
        if (OpponentDeck.GetDeckSize() == 0)
        {
            HandleEndGame(true);
        }

    }

    /// <summary>
    /// Update card ui to match the given card instance
    /// </summary>
    /// <param name="cardParent">Scene object of the card</param>
    /// <param name="card">Card Instance</param>
    public void SetCardUI(GameObject cardParent, Card card)
    {
        cardParent.SetActive(true);
        TextMeshProUGUI valueLabel = cardParent.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();
        valueLabel.text = card.Value.ToString();
        Sprite suitTexture;
        switch (card.Suit)
        {
            case CardSuit.Clubs:
                suitTexture = ClubsIcon;
                break;
            case CardSuit.Diamonds:
                suitTexture = DiamondsIcon;
                break;
            case CardSuit.Hearts:
                suitTexture = HeartsIcon;
                break;
            default:
                suitTexture = SpadesIcon;
                break;
        }


        Transform SuitParent = cardParent.transform.GetChild(0);
        for (int i = 0; i < 4; i++)
        {
            SuitParent.GetChild(i).GetComponent<Image>().sprite = suitTexture;
        }
    }

    /// <summary>
    /// A tie is handled differently, but dealing 3 cards, and doing another battle.
    /// </summary>
    /// <param name="cardsInPlay">Cards that will be given to the victor</param>
    public void HandleTie(List<Card> cardsInPlay)
    {
        UserTieParent.gameObject.SetActive(true);
        OpponentTieParent.gameObject.SetActive(true);

        int cardsToPull = Mathf.Min(UserDeck.GetDeckSize(), OpponentDeck.GetDeckSize(), 4);
        for (int i = 0; i < cardsToPull - 1; i++)
        {
            cardsInPlay.Add(UserDeck.GetNextCard());
            cardsInPlay.Add(OpponentDeck.GetNextCard());
        }
        Card opponentCard = OpponentDeck.GetNextCard();
        Card userCard = UserDeck.GetNextCard();
        cardsInPlay.Add(opponentCard);
        cardsInPlay.Add(userCard);

        OpponentDeckSizeText.text = OpponentDeck.GetDeckSize().ToString();
        UserDeckSizeText.text = UserDeck.GetDeckSize().ToString();


        Transform userCardTrans = UserTieParent.GetChild(3);
        Transform oppCardTrans = OpponentTieParent.GetChild(3);
        SetCardUI(userCardTrans.gameObject, userCard);
        SetCardUI(oppCardTrans.gameObject, opponentCard);

        MatchupOutcomes outcome = userCard.Battle(opponentCard);
        if (outcome == MatchupOutcomes.Tie)
        {
            HandleTie(cardsInPlay);
        }
        else
        {
            BattleResultText.gameObject.SetActive(true);

            if (outcome == MatchupOutcomes.Win)
            {
                BattleResultText.text = "You won this one!";
                foreach(Card c in cardsInPlay)
                {
                    UserDeck.AddCardToBottom(c);
                }
            }
            else
            {
                BattleResultText.text = "You lost this one!";
                foreach (Card c in cardsInPlay)
                {
                    OpponentDeck.AddCardToBottom(c);
                }
            }

        }
        OpponentDeckSizeText.text = OpponentDeck.GetDeckSize().ToString();
        UserDeckSizeText.text = UserDeck.GetDeckSize().ToString();

        if (UserDeck.GetDeckSize() == 0)
        {
            HandleEndGame(false);
        }
        if (OpponentDeck.GetDeckSize() == 0)
        {
            HandleEndGame(true);
        }


    }

    /// <summary>
    /// Display win/lose state.
    /// </summary>
    /// <param name="userWin">Did the user win?</param>
    public void HandleEndGame(bool userWin)
    {
        UserCard.gameObject.SetActive(false);
        OpponentCard.gameObject.SetActive(false);
        UserTieParent.gameObject.SetActive(false);
        OpponentTieParent.gameObject.SetActive(false);
        BattleResultText.gameObject.SetActive(false);
        WinConditionText.gameObject.SetActive(true);
        WinConditionText.text = userWin ? "You Won!" : "You Lost!";
    }



}
