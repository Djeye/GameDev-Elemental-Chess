using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElemMotion : MonoBehaviour
{
    private const float DESK_OFFSET = 0.05f; // ������ �� ������������ ����������, ����� ����� ������ �� �������
    [SerializeField] private ElemParameters elemParameters; // ����������� ����� ��������, ���������� �������

    [Header("Changable Parameters")]
    [SerializeField] float _speed; // �������� � ������������ ��������
    [SerializeField] float _maxHp; // �������� � ������������ ��������
    [SerializeField] float _attackDamage, _attackFreq; // ���� �� �����, � �������� �����
    [SerializeField] private string _element; // ������
    [SerializeField] private float manaCost; // ��������� ����� 
    [SerializeField] private bool _isGhost = false; // ������, ���� ���� - �������

    // ���������, ���������� �� ElemParameters
    private Parameters parameters; // �������� ���������
    private ManaBar manaBar; // ������� ����
    private Canvas _fieldCanvas; // ��������� ��������� ������� �������� � ���������
    LayerMask groundLayerMask; // ����, �� �������� ����� ����������� Drag'n'Drop
    private Image _fieldSquare; // ���������, ������� ��� �������, ������������ ����������� ��������� �����
    private Image _hpBar; // �������  ��������, � ��� �� ��������� ��� � ��������� ����

    private Vector3 startPos, roundedPosition; // ��������� �������, � ������� ������ ������ (�����������)
    private float minZ = -0.5f, maxZ = 4.5f, minX = -0.5f, maxX = 7.5f; // ������� ������� ����

    private bool enteredInGame = false, posibleToPlace = false, enterTower = false; // ������ �� � ����, �������� �� ���������, ����� �� � �����
    private float lerpSpeed = 3f, defaultSpeed = 0.35f; // �������� ������������ ��������, �������������� �������� ��������
    private float hp, startSpeed;  // ������� ��������, ��������� ��������
    private float earthSlowTime; // ����� ���������� �� �����

    private Animator animator; // ��������� ��������� ����� ������ �������� 
    private CapsuleCollider capsCol; // ��������� ����������
    private GameObject splash, earthSlow; // ���������� �������, � �������� �������� ���������������� 

    // ���������� ��� �������������� ������������� �� ������ Start
    private void Awake()
    {
        // ��� ��������� �������� ���������, �� �������� �������� ��������� ��������� ��� ������
        if (_isGhost) elemParameters = transform.parent.GetComponentInParent<ElemParameters>();
    }

    void Start()
    {
        GetMainElemsParameters(); // �������� �������� ��������� �� ������� � ������ �����������
        if (_isGhost) // ���� �������
        {
            InitializeGhost(); // �������������� ��������� ��� ��������
        }
        else
        {
            InitializeElem(); // �������������� ��������� ��� �����
        }
    }

    private void GetMainElemsParameters()
    {
        animator = GetComponentInChildren<Animator>(); // �������� ��������� ���������, ��� ����� ��������
        hp = _maxHp; // ��� ������ ��������� �������� ����� �������������
        startSpeed = _speed; // ��������� ��������� ��������, ������ ��� �� ����� ��� ����� ��������

        parameters = elemParameters.parameters; // �������� �������� ���������
        manaBar = elemParameters.manaBar; // �������� ������� ����
        _fieldCanvas = elemParameters._fieldCanvas; // �������� ������ ��� ��������� ������� ��������
        groundLayerMask = elemParameters.groundLayerMask; // �������� ���� ��� ����������� ������� �������
        _fieldSquare = elemParameters._fieldSquare; // �������� ������ �������� ��� �����������
        _hpBar = elemParameters._hpBar; // �������� ������ ������� ��������

        capsCol = GetComponent<CapsuleCollider>(); // �������� ��������� ����������
    }

    private void InitializeElem()
    {
        tag = "InactiveElem";
        name = name.Replace("(Clone)", "").Trim(); // ������ ��� ������ �����, ����� �� ���� ����������� (Clone) � ��������
        startPos = transform.position; // ��������� ��������� �������

        _fieldSquare = Instantiate(_fieldSquare, Vector3.zero, Quaternion.identity, _fieldCanvas.transform); // ������� �������
        _fieldSquare.transform.localRotation = Quaternion.identity; // ������������ ��� � ��������� �������
        _fieldSquare.enabled = false; // ������ ���������
    }

    private void InitializeGhost()
    {
        enteredInGame = true; // �� ����� ������ � ����
        SpawnHpBar(); // ������� ������� ��������
    }

    private void FixedUpdate()
    {
        if (enteredInGame) // ���� ���� ������ � ����, � �� �� �����
        {
            if (!enterTower) // ���� �� ������� ������ � �����
            {
                if (!Field.IsCellOccupied(transform.position) || isGhost()) // ���� ������ �� ������ ���� ���� - �������
                {
                    float fieldSpeed = -1 * defaultSpeed * _speed * Time.fixedDeltaTime;
                    Vector3 changePosition = new Vector3(fieldSpeed, 0, 0); // ������� ����������, ������� ������ ���� ������
                    transform.position += changePosition; // ������ ������� �� ��� ����������
                    _hpBar.transform.localPosition += changePosition; // ���������� ������� �������� ������ � ������
                }

                ApplyEarthSlow(); // ��������� ������ ����������
            }

            // ������� ������� �� ��� Z == ����� �����
            int roundedZ = Mathf.RoundToInt(transform.position.z);
            Field.field[roundedZ].Add(transform); // ��������� ������� � ���� ����������� ��� ���� ����� ����. ���������� ����������

            // ������ ��������� ������� ��������
            _hpBar.fillAmount = Mathf.Lerp(_hpBar.fillAmount, hp / _maxHp, lerpSpeed * Time.fixedDeltaTime); // ��������� ������� �����
            _hpBar.color = Color.Lerp(parameters.hpRingRed, parameters.hpRingGreen, hp / _maxHp); // ������ ���� ������������


            // ������� ��� ������ �����: �������� ������ ����, ��� �� ����� � ����� == ��������� �����,
            // ��� ��� ���� ��������� �����, ��� �� ����� ����� �����, ��� ��������� ����� ����
            if (hp <= 0 || transform.position.x < 0 || !Field.aliveTowers[roundedZ] || Field.endOfGame) // || enterTower 
            {
                animator.SetTrigger("isDead"); // ������� �������� ������
                capsCol.enabled = false;
                enteredInGame = false;
                Destroy(_hpBar.gameObject);
                Field.field[roundedZ].Remove(transform); // ������� ���������� �� ���� ����� �� ���� - ��� ������ ��� � ����
                if (isGhost()) manaBar.SpendMana(-manaCost); // ��������������� ���� ��� ������ ��������
            }
        }
    }

    // � ������ Update ��������� ����� ����������, ���� ���������� - ������� ������, ���������� ��������
    private void ApplyEarthSlow()
    {
        if (earthSlowTime > 0)
        {
            // ������������ ����� ���������� �� 0 �� ��������������, ��������� �� ��������
            earthSlowTime = Mathf.Clamp(earthSlowTime - Time.fixedDeltaTime, 0, parameters._earthSlowDuration);
            if (earthSlowTime == 0f)
            {
                _speed = startSpeed; // ���������� ��������
                if (earthSlow) Destroy(earthSlow.gameObject); // ���� ���� ������ ���������� - �������
            }
        }
    }

    // TODO ���������� �������� ����� ��� ��� ����������, �������� ��������� ����� �� ���� -> ����������� ��� ������, � �� ���������
    private void ApplyWoodRoot()
    {

    }

    // �������� ��� ������� ���� (Drag'n'Drop)
    private void OnMouseDrag()
    {
        if (!enteredInGame && !Field.endOfGame && manaBar.IsEnoughMana(manaCost)) // ���� ��� �� ������ � ����, � ���� �� ��������� � ���������� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ������� ��� ��� Drag'n'Drop 
            RaycastHit hit;
            float onDeskY = startPos.y + DESK_OFFSET;
            if (Physics.Raycast(ray, out hit, 30f, groundLayerMask)) // ���� ��� �������� ���� ��� ����� � �������� 30 �.�.
            {
                // ���������� ����� ������ ����, ��������� ��������� ������ +- ������
                transform.position = new Vector3(Mathf.Clamp(hit.point.x, maxX - 0.7f, maxX + 0.7f), onDeskY,
                                                 Mathf.Clamp(hit.point.z, minZ + 0.3f, maxZ - 0.3f));
            }

            int roundedX = Mathf.RoundToInt(transform.position.x); // ���������� �������� ���������, 
            int roundedZ = Mathf.RoundToInt(transform.position.z); // ��� ���������� ��������� �������� 
            roundedPosition = new Vector3(roundedX, onDeskY, roundedZ); // � ������������ ����������� �����


            manaBar.ShowPreview(manaCost); // ������������� ������� ���� ����� ������ ���� ��� ����������

            if (isObjectInField()) // ���� ���� ��������� ������ ����
            {
                // ����������� ��������� �� ����� - �� ����� ���� �����, � ���� ���� �������, ���� ������ �� ������
                posibleToPlace = Field.aliveTowers[roundedZ] && (isGhost() || !Field.IsCellOccupied(roundedPosition));
                _fieldSquare.color = posibleToPlace ? parameters.squareGreen : parameters.squareRed; // ������ ���� �������� � ����������� �� ����� �������
                _fieldSquare.enabled = true; // ������ ������� �������
                _fieldSquare.transform.localPosition = new Vector3(roundedX, roundedZ, 0); // ���������� ������� ������ � ������, �� �� �������
            }
            else
            {
                posibleToPlace = false; // ���������� ��������� ����� ��� ����
                _fieldSquare.enabled = false; // ������ ������� ���������
            }
        }
    }

    // �������� ��� ���������� ����
    private void OnMouseUp()
    {
        if (!enteredInGame)
        {
            if (posibleToPlace) // ���� �� ������ � ���� � ���� ����������� ���������
            {
                enteredInGame = true; // ������ � ����
                tag = "Elem";
                Instantiate(this.gameObject, startPos, Quaternion.identity, transform.parent); // ������� ����� ������� �� ��������� �������
                transform.position = roundedPosition; // ���������� ����� � ����� ��������� ������
                SpawnHpBar(); // ������� ������� ��������
                Destroy(_fieldSquare.gameObject); // ���������� ������� - ��� ����� ����� �� ������ �� �����

                manaBar.SpendMana(manaCost);
                animator.SetTrigger("enteredInGame");
            }
            else
            {
                transform.position = startPos;
                manaBar.ReturnPreview();
            }
        }
        if (_fieldSquare) _fieldSquare.enabled = false; // ���� ������� ��� ����������, �� ��� ������������ ���������� - ���������  

    }

    private bool isObjectInField()
    {
        return (transform.position.x > minX && transform.position.x < maxX) &&
            (transform.position.z > minZ && transform.position.z < maxZ);
    }

    // �������� ��� ������� ������ ��������
    private void OnTriggerEnter(Collider other)
    {
        // ���������� ������ ��� �������� ����� � ����
        if (enteredInGame)
        {
            // ���� ��� ������� - ������, � ���� ������ �� ����������� �����
            if (other.CompareTag("Projectile") && !other.gameObject.Equals(splash))
            {
                ProjectileMotion projectile = other.GetComponent<ProjectileMotion>();
                float projDamage = projectile.GetProjDamage();
                string projElement = projectile.GetProjElement();
                hp -= projDamage * elemParameters.GetDamageCoef(_element, projElement);

                ProjectileEffect(projectile);
            }
            else if (other.CompareTag("Tower") && !enterTower)
            {
                _speed = 0f;
                animator.SetTrigger("isAttack");
                enterTower = true;
                TowerController tower = other.GetComponent<TowerController>();
                tower.SetHpBarVisible();
                StartCoroutine(AttackTower(tower));
            }
        }
    }

    private IEnumerator AttackTower(TowerController tower)
    {
        while (true)
        {
            tower.TakeDamage(_attackDamage);
            yield return new WaitForSeconds(1 / _attackFreq);
        }
    }

    private void ProjectileEffect(ProjectileMotion projectile)
    {
        if (!projectile.IsSplash()) // ���� ������ - �� ������ �� �������, �� ����������� ������ ������� ._.
        {
            switch (projectile.GetProjElement()) // � ����������� �� �������� ������� ������� ��������������� ������
            {
                case "fire":
                    splash = Instantiate(elemParameters._fireEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, transform);
                    break;
                case "water":
                    splash = Instantiate(elemParameters._waterEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, transform);
                    break;
                case "metal":
                    projectile.MetalPenetrateReduceDamage(); // ��������� ���� ������� � ������ ���������� �� �����
                    splash = Instantiate(elemParameters._metalEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, transform);
                    break;
                case "wood":
                    WoodProcessHits();
                    break;
                case "earth":
                    EarthReduceSpeed();
                    break;
            }
        }
    }

    private void WoodProcessHits()
    {
        if (elemParameters.IsLastWoodHit())
        {
            Instantiate(elemParameters._woodEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.Euler(-90, 0, 0), transform);
            StartCoroutine(WoodRoot());
        }
        elemParameters.IncreaseWoodHits();
    }

    private void EarthReduceSpeed()
    {
        if (!earthSlow)
        {
            earthSlow = Instantiate(elemParameters._earthEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.Euler(-90, 0, 0), transform);
        }
        earthSlowTime = parameters._earthSlowDuration;
        _speed = startSpeed * parameters._earthSlowSpeedReduction;
    }

    private IEnumerator WoodRoot()
    {
        _speed = 0f;
        animator.SetTrigger("isRooted");
        yield return new WaitForSecondsRealtime(parameters._woodRootDuration);
        _speed = startSpeed;
        animator.SetTrigger("enteredInGame");
    }

    // ������� ������� ��������. ��� ��������� - �����, ��� ������ - ����� �����������
    private void SpawnHpBar()
    {
        _hpBar = Instantiate(_hpBar, Vector3.zero, Quaternion.identity, _fieldCanvas.transform);
        _hpBar.color = parameters.hpRingGreen; // ������ ���� ������� ����� - �������
        _hpBar.transform.localRotation = Quaternion.identity; // ������������ ����� �������
        _hpBar.transform.localPosition = new Vector3(transform.position.x, transform.position.z, 0); // ������� � ����� �����
    }

    // ������, ���� ���� - �������
    public bool isGhost() { return _isGhost; }

    public Parameters GetMainParameters() { return parameters; }
}
