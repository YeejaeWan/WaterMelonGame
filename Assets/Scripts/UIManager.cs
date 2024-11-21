using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    [SerializeField] public TextMeshProUGUI tmpScore;
    [SerializeField] public TextMeshProUGUI tmpBestScore;

    [Header("게임오버")]
    public GameObject gameoverUI;
    public TextMeshProUGUI tmpGameOverScore;
    [SerializeField] private Button buttonRestart;
    private WaitForSeconds waitButtonClick;

    [Header("게임시작")]
    [SerializeField] private GameObject gameStartUI;
    [SerializeField] private GameObject borderLine;
    [SerializeField] private GameObject bottom;
    [SerializeField] private Button buttonStart;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        waitButtonClick = new WaitForSeconds(0.5f);

        buttonRestart.onClick.AddListener(()=> 
        { 
            StartCoroutine(Restart());       
        });

        buttonStart.onClick.AddListener(GameStart);

        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", 0);
        } 

        tmpBestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }
    
    private void GameStart()
    {
        borderLine.SetActive(true);
        bottom.SetActive(true);
        tmpScore.gameObject.SetActive(true);
        tmpBestScore.gameObject.SetActive(true);

        gameStartUI.SetActive(false);

        SoundManager.Instance.PlaySFX(SFX.Button);
        GameManager.Instance.NextWaterMelon();
    }


    /*void LateUpdate()
    {
        tmpScore.text = GameManager.Instance.score.ToString();    
    }

   */

    private IEnumerator Restart()
    {
        SoundManager.Instance.PlaySFX(SFX.Button);
        yield return waitButtonClick;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }    
}
