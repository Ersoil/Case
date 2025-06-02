using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// ������� ���������� �������� � �������� ����� � ����.
/// ����������� ��������� ���, ��������� ������ ������� � ���������� ����������.
/// </summary>
public class RoundSystem : MonoBehaviour
{
    /// <summary>
    /// Singleton ��������� ������� �������
    /// </summary>
    public static RoundSystem Instance { get; private set; }
    #region SystemFields
    // ����������� ���� ��� �������� ��������� �������
    private static int _round;              // ������� �����
    private static int _roundSteps;         // ���������� ����� � ������
    private static int _currentStep = 0;    // ������� ��� � ������
    private static int _endRound;           // ��������� �����
    private static bool _isEnemyAction;     // ���� ������� ���� (true - �����, false - ��������)
    private static int WinnerStatus;        // ������ ���������� (0 - ��������, 1 - �����, 2 - �����)
    
    // ������ ������
    private static List<GameObject> enemyGroup = new List<GameObject>();
    private static List<GameObject> allienGroup = new List<GameObject>();
    #endregion
    #region InspectorFields
    // ������������� ��������� � ����������
    [SerializeField, Tooltip("��������� �����")]
    private int round = 1;
    [SerializeField, Tooltip("���������� ����� � ������")]
    private int roundSteps = 2;
    [SerializeField, Tooltip("��������� �����")]
    private int endRound = 5;
    #endregion
    #region Events
    // ������� �������
    /// <summary>������� ��������� ����</summary>
    public static Action onEndGame;
    /// <summary>������� ��������� ������� ����</summary>
    public static Action onChangedQueue;
    /// <summary>������� ��������� ������</summary>
    public static Action onChangedRound;
    /// <summary>������� ������������� �������</summary>
    public static Action onInitSystem;
    /// <summary>������� ������ �������� ������</summary>
    public static Action OnStartAction;
    #endregion
    #region Properties
    /// <summary>
    /// ������� ������� ���� (true - �����, false - ��������)
    /// </summary>
    public static bool GetQueue
    {
        get { return _isEnemyAction; }
    }

    /// <summary>
    /// ������� ����� ������
    /// </summary>
    public static int GetRound
    {
        get { return _round; }
    }

    /// <summary>
    /// ����� ���������� ������
    /// </summary>
    public static int GetEndRound
    {
        get { return _endRound; }
    }

    /// <summary>
    /// ������ ���������� (0 - ��������, 1 - �����, 2 - �����)
    /// </summary>
    public static int GetWinner
    {
        get { return WinnerStatus; }
    }

    /// <summary>
    /// �������������� ����������� ���� ���������� �� ����������
    /// </summary>
    private void initInspectorFields()
    {
        _round = round;
        _roundSteps = roundSteps;
        _endRound = endRound;
    }

    /// <summary>
    /// ������ ������� ������
    /// </summary>
    public static List<GameObject> GetAllienGroup
    {
        get { return allienGroup; }
    }

    /// <summary>
    /// ������ ��������� ������
    /// </summary>
    public static List<GameObject> GetEnemyGroup
    {
        get { return enemyGroup; }
    }
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initInspectorFields();
            DontDestroyOnLoad(gameObject);
            RegisterExistingUnits();
            onInitSystem?.Invoke();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Unit.onInitUnit += UpdateLists;
    }
    #region PrivateMethods
    /// <summary>
    /// ��������� ��������� HP ������ ������
    /// </summary>
    /// <param name="Units">������ ������</param>
    /// <returns>��������� HP ������</returns>
    private float GetHP(List<GameObject> Units)
    {
        float HP = 0;
        foreach (var unit in Units)
        {
            HP += unit.GetComponent<Model>().GetHP;
        }
        return HP;
    }

    /// <summary>
    /// ��������� �������� ����� � ��������������� ������
    /// </summary>
    /// <param name="allien">������ �������� �����</param>
    private void UpdateAllienGroup(GameObject allien)
    {
        allienGroup.Add(allien);
    }

    /// <summary>
    /// ��������� ���������� ����� � ��������������� ������
    /// </summary>
    /// <param name="enemy">������ ���������� �����</param>
    private void UpdateEnemyGroup(GameObject enemy)
    {
        enemyGroup.Add(enemy);
    }

    /// <summary>
    /// ���������� ���������� �� ������ ����������� HP
    /// </summary>
    private void SetWinner()
    {
        var AlienHP = GetHP(allienGroup);
        var EnemyHP = GetHP(enemyGroup);
        Debug.Log(AlienHP);
        Debug.Log(EnemyHP);
        if (AlienHP > EnemyHP) WinnerStatus = 0;
        else if (AlienHP < EnemyHP) WinnerStatus = 1;
        else WinnerStatus = 2;
    }

    /// <summary>
    /// ��������� ������ ������ ��� ������������� ������ �����
    /// </summary>
    /// <param name="unit">������ �����</param>
    /// <param name="isEnemy">�������������� � ��������� ������</param>
    private void UpdateLists(GameObject unit, bool isEnemy)
    {
        OnStartAction += unit.GetComponent<Unit>().StartUnitActions;
        onChangedQueue += unit.GetComponent<Unit>().QueueCheck;
        if (isEnemy) UpdateEnemyGroup(unit);
        else UpdateAllienGroup(unit);
        Debug.Log($"Added {(isEnemy ? "Enemy" : "Allien")}: {unit.name}");
    }

    private void Update()
    {
        Debug.Log(allienGroup.Count);
    }

    /// <summary>
    /// ������������ ��� ������������ ����� �� �����
    /// </summary>
    private void RegisterExistingUnits()
    {
        Unit[] units = FindObjectsOfType<Unit>(true);
        foreach (Unit unit in units)
        {
            UpdateLists(unit.gameObject, unit.IsEnemy);
        }
    }

    /// <summary>
    /// ������� � ���������� ������
    /// </summary>
    private void ChangeRound()
    {
        if (_round == _endRound)
        {
            SetWinner();
            onEndGame?.Invoke();
            return;
        }
        _round++;
    }

    /// <summary>
    /// ������� � ���������� ���� � ������
    /// </summary>
    private void Step()
    {
        ChangeQueue();
        _currentStep++;
        if (_currentStep % _roundSteps == 0)
        {
            ChangeRound();
            _currentStep = 0;
        }
    }

    /// <summary>
    /// ����� ������� ���� ����� ���������
    /// </summary>
    private void ChangeQueue()
    {
        _isEnemyAction = !_isEnemyAction;
    }

    /// <summary>
    /// ��������� �������� ������ � ������� ����
    /// </summary>
    public void StartActions()
    {
        Step();
        OnStartAction?.Invoke();
        onChangedRound?.Invoke();
        onChangedQueue?.Invoke();
    }
    #endregion
}