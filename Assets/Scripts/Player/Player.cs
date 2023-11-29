using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using BackEnd.Tcp;
using BackEnd;

public class Player : MonoBehaviour
{
    public enum SurfaceType
    {
        Opaque,
        Transparent
    }
    public enum BlendMode
    {
        Alpha,
        Premultiply,
        Additive,
        Multiply
    }

    private SessionId index = 0;
    private string nickName = string.Empty;
    private bool isMe = false;

    // 스테이터스
    public int hp { get; private set; } = 0;
    private const int MAX_HP = 5;
    private bool isLive = false;
    private float coolTime = 0.0f;
    private float MAX_COOLTIME = 0.3f;
    private bool isHide = false;

    // UI
    public GameObject nameObject;
    //public GameObject hpObject;
    //public VirtualStick TESTONLY_vertualStick;
    //public VirtualStick TESTONLY_attackStick;
    private List<GameObject> hpUi;
    private readonly string playerCanvas = "PlayerCanvas";

    // 애니메이터
    // private Animator anim;

    // 이동관련
    public bool isMove { get; private set; }
    public Vector3 moveVector { get; private set; }

    public bool isRotate { get; private set; }

    private float rotSpeed = 4.0f;
    private float moveSpeed = 4.0f;

    private GameObject playerModelObject;
    private Rigidbody rigidBody;

    void Start()
    {
        if (BackEndMatchManager.GetInstance() == null)
        {
            // 매칭 인스턴스가 존재하지 않을 경우 (인게임 테스트 용도)
            Initialize(SessionId.None, "testPlayer");
        }
    }

    public void Initialize(SessionId index, string nickName)
    {
        //this.isMe = isMe;
        this.index = index;
        this.nickName = nickName;

        var playerUICanvas = GameObject.FindGameObjectWithTag(playerCanvas);
        nameObject = Instantiate(nameObject, Vector3.zero, Quaternion.identity, playerUICanvas.transform);
        //hpObject = Instantiate(hpObject, Vector3.zero, Quaternion.identity, playerUICanvas.transform);

        nameObject.GetComponent<Text>().text = nickName;

        //if (this.isMe)
        //{
        //    Camera.main.GetComponent<FollowCamera>().target = this.transform;
        //}

        this.isLive = true;

        this.isMove = false;
        this.moveVector = new Vector3(0, 0, 0);
        this.isRotate = false;

        //hp
        hp = MAX_HP;
        hpUi = new List<GameObject>();
        for (int i = 0; i < 5; ++i)
        {
            //hpUi.Add(hpObject.transform.GetChild(i + 5).gameObject);
            //hpUi[i].SetActive(false);
        }

        playerModelObject = this.gameObject;
        //playerModelObject.transform.rotation = Quaternion.Euler(0, rot, 0);

        rigidBody = this.GetComponent<Rigidbody>();

        nameObject.transform.position = GetNameUIPos();
        //hpObject.transform.position = GetHeartUIPos();
        if (BackEndMatchManager.GetInstance().nowModeType == MatchModeType.TeamOnTeam)
        {
            var teamNumber = BackEndMatchManager.GetInstance().GetTeamInfo(index);
            var mySession = Backend.Match.GetMySessionId();
            var myTeam = BackEndMatchManager.GetInstance().GetTeamInfo(mySession);

            if (teamNumber == myTeam)
            {
                nameObject.GetComponent<Text>().color = new Color(0, 0, 1);
                Debug.Log("myTeam : " + index);
            }
            else
            {
                nameObject.GetComponent<Text>().color = new Color(1, 0, 0);
                Debug.Log("enemyTeam : " + index);
            }
        }
    }

    #region 이동관련 함수
    /*
     * 변화량만큼 이동
     * 특정 좌표로 이동
     */
    public void SetMoveVector(float move)
    {
        SetMoveVector(this.transform.forward * move);
    }
    public void SetMoveVector(Vector3 vector)
    {
        moveVector = vector;

        if (vector == Vector3.zero)
        {
            isMove = false;
        }
        else
        {
            isMove = true;
        }
    }

    public void Move()
    {
        Move(moveVector);
    }
    public void Move(Vector3 var)
    {
        if (!isLive)
        {
            return;
        }
        // 회전
        if (var.Equals(Vector3.zero))
        {
            isRotate = false;
        }
        else
        {
            if (Quaternion.Angle(playerModelObject.transform.rotation, Quaternion.LookRotation(var)) > Quaternion.kEpsilon)
            {
                isRotate = true;
            }
            else
            {
                isRotate = false;
            }
        }

        //playerModelObject.transform.rotation = Quaternion.LookRotation(var);

        // 이동
        var pos = gameObject.transform.position + playerModelObject.transform.forward * moveSpeed * Time.deltaTime;
        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, pos, Time.deltaTime * smoothVal);
        SetPosition(pos);
    }

    public void Rotate()
    {
        if (moveVector.Equals(Vector3.zero))
        {
            isRotate = false;
            return;
        }
        if (Quaternion.Angle(playerModelObject.transform.rotation, Quaternion.LookRotation(moveVector)) < Quaternion.kEpsilon)
        {
            isRotate = false;
            return;
        }
        playerModelObject.transform.rotation = Quaternion.Lerp(playerModelObject.transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotSpeed);
    }

    public void SetPosition(Vector3 pos)
    {
        if (!isLive)
        {
            return;
        }
        gameObject.transform.position = pos;
    }

    // isStatic이 true이면 해당 위치로 바로 이동
    public void SetPosition(float x, float y, float z)
    {
        if (!isLive)
        {
            return;
        }
        Vector3 pos = new Vector3(x, y, z);
        SetPosition(pos);
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public Vector3 GetRotation()
    {
        //return gameObject.transform.rotation;
        return gameObject.transform.rotation.eulerAngles;
    }
    #endregion

    public void Attack()
    {
        if (!isLive)
        {
            return;
        }
        if (coolTime > 0.0f)
        {
            return;
        }
        coolTime = MAX_COOLTIME;
        Vector3 pos = this.transform.position + (this.transform.forward * 2);
        //BulletManager.Instance.ShootBullet(pos, this.transform.forward);
    }

    public void Attack(Vector3 target)
    {
        if (!isLive)
        {
            return;
        }
        if (coolTime > 0.0f)
        {
            return;
        }
        //StartAnimation(AnimIndex.stop);
        coolTime = MAX_COOLTIME;
        target.y = this.transform.position.y;
        Vector3 dir = Vector3.Normalize(target - this.transform.position);
        Vector3 pos = this.transform.position + (dir * 2);
        //BulletManager.Instance.ShootBullet(pos, dir);
    }

    public bool GetIsLive()
    {
        return isLive;
    }

    public void Damaged()
    {
        hpUi[MAX_HP - hp].SetActive(true);
        hp -= 1;
    }

    public void SetHP(int hp)
    {
        this.hp = hp;
        for (int i = hp; i < 5; ++i)
        {
            hpUi[MAX_HP - 1 - i].SetActive(true);
        }
    }

    private void PlayerDie()
    {
        isLive = false;
        nameObject.SetActive(false);
        //hpObject.SetActive(false);
    }

    Vector3 GetNameUIPos()
    {
        return this.transform.position + (Vector3.forward * 1.6f) + (Vector3.up * 2.5f);
    }

    Vector3 GetHeartUIPos()
    {
        return this.transform.position + (Vector3.forward * 0.8f) + (Vector3.up * 2);
    }

    void Update()
    {
        //if (BackEndMatchManager.GetInstance() == null)
        //{
        //    // 매칭 인스턴스가 존재하지 않는 경우 (인게임 테스트 용도)
        //    Vector3 tmp = new Vector3(TESTONLY_vertualStick.GetHorizontalValue(), 0, TESTONLY_vertualStick.GetVerticalValue());
        //    tmp = Vector3.Normalize(tmp);
        //    SetMoveVector(tmp);
        //    Move();


        //    if (TESTONLY_attackStick.isInputEnable)
        //    {
        //        Vector3 tmp2 = new Vector3(TESTONLY_attackStick.GetHorizontalValue(), 0, TESTONLY_attackStick.GetVerticalValue());
        //        if (!tmp2.Equals(Vector3.zero))
        //        {
        //            tmp2 += GetPosition();
        //            Attack(tmp2);
        //        }
        //    }
        //}

        if (!isLive)
        {
            return;
        }

        if (isMove)
        {
            Move();
        }

        if (isRotate)
        {
            Rotate();
        }

        if (coolTime > 0.0f)
        {
            coolTime -= Time.deltaTime;
        }

        if (transform.position.y < -10.0f)
        {
            PlayerDie();
            WorldManager.instance.dieEvent(index);
        }

        if (hp <= 0)
        {
            PlayerDie();
            WorldManager.instance.dieEvent(index);
        }

        if (nameObject.activeSelf)
        {
            nameObject.transform.position = GetNameUIPos();
            //hpObject.transform.position = GetHeartUIPos();
        }
    }

    public SessionId GetIndex()
    {
        return index;
    }

    public bool IsMe()
    {
        return isMe;
    }

    public string GetNickName()
    {
        return nickName;
    }

    void OnTriggerEnter(Collider collider)
    {
        // 플레이어 투명화
        if (collider.gameObject.CompareTag("Bush"))
        {
            if (isHide)
            {
                return;
            }
            isHide = true;

            var standardShaderMaterial = playerModelObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
            standardShaderMaterial.SetFloat("_Surface", (float)SurfaceType.Transparent);
            standardShaderMaterial.SetFloat("_Blend", (float)BlendMode.Alpha);

            standardShaderMaterial.SetOverrideTag("RenderType", "Transparent");
            standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            standardShaderMaterial.SetInt("_ZWrite", 0);
            standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            standardShaderMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            standardShaderMaterial.SetShaderPassEnabled("ShadowCaster", false);

            if (isMe)
            {
                standardShaderMaterial.color = new Color32(255, 255, 255, 100);
            }
            else
            {
                standardShaderMaterial.color = new Color32(255, 255, 255, 0);
                nameObject.SetActive(false);
                //hpObject.SetActive(false);
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (isHide)
        {
            return;
        }
        isHide = true;

        var standardShaderMaterial = playerModelObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
        standardShaderMaterial.SetFloat("_Surface", (float)SurfaceType.Transparent);
        standardShaderMaterial.SetFloat("_Blend", (float)BlendMode.Alpha);

        standardShaderMaterial.SetOverrideTag("RenderType", "Transparent");
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        standardShaderMaterial.SetInt("_ZWrite", 0);
        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        standardShaderMaterial.SetShaderPassEnabled("ShadowCaster", false);

        if (isMe)
        {
            standardShaderMaterial.color = new Color32(255, 255, 255, 100);
        }
        else
        {
            standardShaderMaterial.color = new Color32(255, 255, 255, 0);
            nameObject.SetActive(false);
            //hpObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // 플레이어 투명화 해제
        if (collider.gameObject.CompareTag("Bush"))
        {
            isHide = false;
            var standardShaderMaterial = playerModelObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
            standardShaderMaterial.SetFloat("_Surface", (float)SurfaceType.Opaque);
            standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            standardShaderMaterial.SetInt("_ZWrite", 1);
            standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
            standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            standardShaderMaterial.renderQueue = 2450;

            standardShaderMaterial.color = new Color32(255, 255, 255, 255);

            nameObject.SetActive(true);
            //hpObject.SetActive(true);
        }
    }
}