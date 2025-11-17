using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    List<Pokemon> pokemons;
    PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                foreach (Transform child in memberSlots[i].transform)
                    child.gameObject.SetActive(true);

                memberSlots[i].SetData(pokemons[i]);
            }
            else
            {
                memberSlots[i].SetEmptyImage();
                foreach (Transform child in memberSlots[i].transform)
                    child.gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose a Pokemon";
    }


    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemons.Count; i++) 
        {
            if (i == selectedMember) //when member is selected, highlight image
            {
                if (pokemons[i].HP > 0)
                {
                    memberSlots[i].SetBGImage(true, false); // selected and not fainted
                }
                else
                {
                    memberSlots[i].SetBGImage(true, true); //selected and fainted
                }
            } // for unselected members
            else
            {
                if (pokemons[i].HP > 0)
                {
                    memberSlots[i].SetBGImage(false, false); //unselected, not fainted
                }
                else
                {
                    memberSlots[i].SetBGImage(false,true); //unselected, fainted
                }
            }
        }
    }
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
