//using UnityEngine;
//using TMPro;
//using System.Collections;
//using UnityEditor.SceneManagement;
//using UnityEngine.SceneManagement;
//using UnityEditor.Rendering;
//using UnityEngine.UI;

//public class IntroEventScript : MonoBehaviour
//{
//    [SerializeField] private TextMeshProUGUI textComponent;
//    [SerializeField] private GameObject dialogueBox;
//    [SerializeField] private Image backgroundSlides;
//    [SerializeField] private Animator animator;
//    //[SerializeField] private Sprite[] slideShow;



//    [SerializeField] private string[] lines;
//    [SerializeField] private float textSpeed;

//    private int index;
//    private int imageIndex = 0;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        textComponent.text = string.Empty;
//        StartCoroutine(IntroEvent());
        

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyUp(KeyCode.Escape))
//        {
//            gameObject.SetActive(false);

//            SceneManager.LoadScene("Scenes/GameScenes/BaseScene");
//        }
//    }
//    IEnumerator IntroEvent()
//    {
//        //animator.Play("FadeIn");
//        yield return new WaitForSeconds(2.4f);
//        StartDialogue();
//    }
//    void StartDialogue()
//    {
//        index = 0;
//        StartCoroutine(TypeLine());
//    }
//    IEnumerator TypeLine()
//    {
//        foreach (char c in lines[index].ToCharArray())
//        {
//            textComponent.text += c;
//            yield return new WaitForSeconds(textSpeed);
//        }
//        yield return new WaitForSeconds(2f);
//        NextLine();
//    }
//    IEnumerator ChangeSlide()
//    {              
//        textComponent.text = string.Empty;
//        imageIndex++;
//        animator.SetInteger("Slides", imageIndex);
//        yield return new WaitForSeconds(2f);
//        NextLine();
//    }
//    IEnumerator EndIntro()
//    {
//        dialogueBox.SetActive(false);
//        animator.SetBool("End",true);
//        yield return new WaitForSeconds(5f);

//    }
//    void NextLine()
//    {

//        if (index < lines.Length - 1)
//        {
//            index++;
//            if (lines[index] == "")
//            {
//                StartCoroutine(ChangeSlide());
//            }
//            else
//            {
//                textComponent.text = string.Empty;
//                StartCoroutine(TypeLine());
//            }
            
//        }
//        else
//        {
            
//            StartCoroutine(EndIntro());
//            SceneManager.LoadScene("Scenes/GameScenes/BaseScene");
//        }
//    }
//}