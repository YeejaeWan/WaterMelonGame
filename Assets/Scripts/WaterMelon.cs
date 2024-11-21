using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMelon: MonoBehaviour
{   
    

    [Header("���콺 ������� ����(���� ���� ������ �����)")]
    [Range(0,1)]
    [SerializeField] private float followMouseSensitivity;

    [Header("����Ǵ� �ӵ�")]
    [Range(0, 1)]
    [SerializeField] private float mergeSpeed = 0.5f;

    [Header("��밡 ������ ����Ǵ� �ð�")]
    [SerializeField] private float followSpeed = 0.2f;

    [Header("�ִϸ��̼� ��� �ð�")]
    [SerializeField] private float playAnimation = 0.3f;

   /* [Header("����Ʈ ������")]
    [Range(0f ,1f)]
    [SerializeField] private float effectSize = 0.5f;*/

    public ParticleSystem effect;

    private WaitForSeconds waitFollow;
    private WaitForSeconds waitAnimation;

    private const float MelonGame_Position_Y = 8f;

    public bool isDrag;
    public Rigidbody2D waterMelonRigid;


    public int level;
    private Animator animator;

    public bool isMerge;

    private CircleCollider2D circleCollider;


    private float stayTime;
    private SpriteRenderer spriteRenderer;


    private WaitForSeconds waitAttach;
    private bool isAttach;
 

    private void Awake()
    {
        waterMelonRigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();

        waitFollow = new WaitForSeconds(followSpeed);
        waitAnimation = new WaitForSeconds(playAnimation);

        spriteRenderer = GetComponent<SpriteRenderer>();
        waitAttach = new WaitForSeconds(0.2f);


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(AttachSound());
    }

    private IEnumerator AttachSound()
    {   
        if(isAttach) yield break; //�ڷ�ƾ Ż��

        isAttach = true;
        SoundManager.Instance.PlaySFX(SFX.Attach);

        yield return waitAttach;
        isAttach = false;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Finish")) {
            stayTime += Time.deltaTime;
            if(stayTime >= 2f)
            {
                spriteRenderer.color = Color.red;
            }
            if(stayTime >= 5f)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Finish"))
        {
            stayTime = 0;
            spriteRenderer.color = Color.white;
        }
    }

    // Stay : �浹�ϴ� ��
    private void OnCollisionStay2D(Collision2D collision)
    {
        //��ġ�� ����
        if (collision.collider.CompareTag("WaterMelon"))
        {
            WaterMelon other = collision.gameObject.GetComponent<WaterMelon>();

            if(level.Equals(other.level) && level < 7 && !isMerge && !other.isMerge)
            {
                //��ü ��Ģ
                float x = transform.position.x;
                float y = transform.position.y;

                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                if(y < otherY || (y.Equals(otherY) && x >otherX))
                {
                    other.Hide(transform.position);
                    LevelUp();

                }
            }

        }
    }

    private void LevelUp()
    {
        isMerge = true;

        waterMelonRigid.velocity = Vector2.zero;
        waterMelonRigid.angularVelocity = 0; //ȸ�� �ӵ�
        StartCoroutine(NextLevel());
    }

    private void PlayEffect()
    {
        effect.transform.position = transform.position;

        effect.transform.localScale = transform.localScale* 0.5f;

        effect.Play();


    }

    private IEnumerator NextLevel()
    {   
        yield return waitFollow;

        animator.SetInteger("Level", level + 1);
        PlayEffect();
        SoundManager.Instance.PlaySFX(SFX.LevelUp);

        yield return waitAnimation;
        level++;
        
        GameManager.Instance.maxLevel = Mathf.Max(level, GameManager.Instance.maxLevel);
        isMerge = false;
    }

    public void Hide(Vector3 target)
    {
        isMerge = true;

        waterMelonRigid.simulated = false;

        circleCollider.enabled = false;

        if(GameManager.Instance.isGameOver)
        {
            PlayEffect();
            
        }



        StartCoroutine(Move(target));
    }

    private IEnumerator Move(Vector3 target)
    {
        int frame = 0;
        while(frame < 20)
        {
            frame++;
            if(target != Vector3.zero)
            {
                transform.position = Vector3.Lerp(transform.position, target, mergeSpeed);

            }

            else
            {
                transform.localScale = Vector3.Lerp(transform.position, target, mergeSpeed);
            }

            yield return null;
        }
        isMerge = false;
        gameObject.SetActive(false);


        if (!GameManager.Instance.isGameOver) { 

            //���Ͽ� ������ ���� ��� ������ �޶���.
            if (level <= 2)
        {
            GameManager.Instance.score += 100;
            UIManager.Instance.tmpScore.text = GameManager.Instance.score.ToString();

        }
        else if(level > 2 && level < 5)
        {
            GameManager.Instance.score += 300;
            UIManager.Instance.tmpScore.text = GameManager.Instance.score.ToString();

        }
        else if (level == 5) 
        {
            GameManager.Instance.score += 500;
            UIManager.Instance.tmpScore.text = GameManager.Instance.score.ToString();

        }
        else if (level == 6)
        {
            GameManager.Instance.score += 700;
            UIManager.Instance.tmpScore.text = GameManager.Instance.score.ToString();

        }
        }

    }

    private void OnEnable()
    {
        //�ִϸ��̼� ��Ʈ�ѷ��� �Ķ���Ͱ� int���̹Ƿ�
        animator.SetInteger("Level", level);
    }

    private void OnDisable()
    {
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        waterMelonRigid.simulated = false;
        waterMelonRigid.velocity = Vector2.zero;
        waterMelonRigid.angularVelocity = 0;
        circleCollider.enabled = true;

    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        waterMelonRigid.simulated = true;
    }

    void Update()
    {   
        if(isDrag) {
            // ĵ���� ��ǥ -> screen point
            // ���� ���� -> world point
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //���� ���� ��輱 ����
            float borderLeft = -4.2f + transform.localScale.x * 0.5f;
            float borderRight = 4.2f - transform.localScale.x * 0.5f;



            if (mousePosition.x < borderLeft)
            {
                mousePosition.x = borderLeft;
            }

            if (mousePosition.x > borderRight)
            {
                mousePosition.x = borderRight;
            }



            mousePosition.z = 0;
            mousePosition.y = MelonGame_Position_Y;

            //���콺�� �ð����� �ΰ� �ε巴�� ��������� �ϴ� �ڵ�
            //Lerp - ���� ����
            // float t �� Ŭ���� ������ �����.
            transform.position = Vector3.Lerp(transform.position, mousePosition, followMouseSensitivity);

        }

    }
}
