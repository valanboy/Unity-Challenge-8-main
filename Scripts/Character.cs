using UnityEngine;

public class Character : MonoBehaviour
{
    public GameObject[] characters;
    public static int selectedCharacter;
    // Start is called before the first frame update
    void Start()
        {
        foreach (GameObject character in characters)
            {
            character.SetActive(false);
            }
        characters[selectedCharacter].SetActive(true);
        }

    public void ChangeCharacter(int newCharacter)
        {
        characters[selectedCharacter].SetActive(false);
        characters[newCharacter].SetActive(true);
        selectedCharacter = newCharacter;

        }
    }
