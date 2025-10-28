using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            gameObject.SetActive(false);

            SceneManager.LoadScene("Scenes/GameScenes/BaseScene");
        }
    }

    void StartDialogue()
    {
        new WaitForSeconds(2f);
        index = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    { 
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(2f);
        NextLine();
    }
    void NextLine()
    {
        if (index < lines.Length -1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            new WaitForSeconds(2f);

            SceneManager.LoadScene("Scenes/GameScenes/BaseScene");
        }
    }
}
