using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static GameManager Instance { get { return instance; } }

    public WaterMelon lastWaterMelon;
    [SerializeField] private GameObject waterMelonPrefab;
    [SerializeField] private Transform waterMelonGroup;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform effectGroup;
    

    [Header("게임 오버 시, 과일이 없어지는 속도 조절")]
    [Range(0, 1)]
    [SerializeField] private float gameOverTime;
    private WaitForSeconds gameOver;

    [Range(0.5f, 2.5f)]
    [SerializeField] private float waitTime;
    private WaitForSeconds wait;


    public int maxLevel;

    public int score = 0;
    public bool isGameOver;

    public List<WaterMelon> waterMelonList = new List<WaterMelon>();

    public List<ParticleSystem> effectList = new List<ParticleSystem>();

    public int poolSize = 10;
    public int poolIndex;


    private void Awake()
    {

        if (instance == null)
            instance = this;

        Application.targetFrameRate = 60;
    }



    void Start()
    {
        wait = new WaitForSeconds(waitTime);
        gameOver = new WaitForSeconds(gameOverTime);

        for (int i = 0; i < poolSize; i++) {
            CreateWaterMelon();
        }

        //NextWaterMelon();


    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;


        //waterMelonGroup.gameObject.SetActive(false);

        StartCoroutine(ScoreByInOrder());
    }

    private IEnumerator ScoreByInOrder()
    {
        WaterMelon[] waterMelons = FindObjectsOfType<WaterMelon>();

        for (int i = 0; i < waterMelons.Length; i++)
        {
            waterMelons[i].waterMelonRigid.simulated = false;
            
        }

        for (int i = 0; i < waterMelons.Length; i++)
        {
            waterMelons[i].Hide(Vector3.zero);

            yield return gameOver;
        }

        yield return wait;

        int bestScore = Mathf.Max(score, PlayerPrefs.GetInt("BestScore"));
        PlayerPrefs.SetInt("BestScore", bestScore);

        UIManager.Instance.gameoverUI.SetActive(true);
        UIManager.Instance.tmpGameOverScore.text = "점수 : " + score.ToString();

        SoundManager.Instance.PlaySFX(SFX.GameOver);
        
    }

    private IEnumerator WaitNextWaterMelon()
    {   
        //마우스를 떼기 전까지 쉬기
        while(lastWaterMelon != null)
        {
            yield return null; //한프레임 쉬기
        }
        yield return wait;
        NextWaterMelon();
    }

    public void NextWaterMelon()
    {
        if (isGameOver) return;

        lastWaterMelon = GetWaterMelon();

        lastWaterMelon.level = Random.Range(0, maxLevel);
        lastWaterMelon.gameObject.SetActive(true);

        SoundManager.Instance.PlaySFX(SFX.Next);
        StartCoroutine(WaitNextWaterMelon());
    }

    private WaterMelon CreateWaterMelon()
    {
        ParticleSystem effect = Instantiate(effectPrefab, effectGroup).GetComponent<ParticleSystem>();
        effect.name = "Effect" + effectList.Count;
        effectList.Add(effect);

        WaterMelon waterMelon = Instantiate(waterMelonPrefab, waterMelonGroup).GetComponent<WaterMelon>();
        waterMelon.name = "WaterMelon" + waterMelonList.Count;
        waterMelon.effect = effect;

        waterMelonList.Add(waterMelon);

        return waterMelon;
    }

    private WaterMelon GetWaterMelon()
    {
        for (int i = 0; i < waterMelonList.Count; i++)
        {   

            if(!waterMelonList[i].gameObject.activeSelf)
            {
                return waterMelonList[i];
            }

            
        }
        return CreateWaterMelon();

    }

    public void TouchDown()
    {
        lastWaterMelon.Drag();    
    }

    public void TouchUp()
    {
        if (lastWaterMelon == null) return;
        lastWaterMelon.Drop();
        lastWaterMelon = null;

    }
}
