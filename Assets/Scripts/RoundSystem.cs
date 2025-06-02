using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Система управления раундами и очередью ходов в игре.
/// Отслеживает состояние боя, управляет сменой раундов и определяет победителя.
/// </summary>
public class RoundSystem : MonoBehaviour
{
    /// <summary>
    /// Singleton экземпляр системы раундов
    /// </summary>
    public static RoundSystem Instance { get; private set; }
    #region SystemFields
    // Статические поля для хранения состояния системы
    private static int _round;              // Текущий раунд
    private static int _roundSteps;         // Количество шагов в раунде
    private static int _currentStep = 0;    // Текущий шаг в раунде
    private static int _endRound;           // Финальный раунд
    private static bool _isEnemyAction;     // Флаг очереди хода (true - враги, false - союзники)
    private static int WinnerStatus;        // Статус победителя (0 - союзники, 1 - враги, 2 - ничья)
    
    // Группы юнитов
    private static List<GameObject> enemyGroup = new List<GameObject>();
    private static List<GameObject> allienGroup = new List<GameObject>();
    #endregion
    #region InspectorFields
    // Настраиваемые параметры в инспекторе
    [SerializeField, Tooltip("Начальный раунд")]
    private int round = 1;
    [SerializeField, Tooltip("Количество шагов в раунде")]
    private int roundSteps = 2;
    [SerializeField, Tooltip("Финальный раунд")]
    private int endRound = 5;
    #endregion
    #region Events
    // События системы
    /// <summary>Событие окончания игры</summary>
    public static Action onEndGame;
    /// <summary>Событие изменения очереди хода</summary>
    public static Action onChangedQueue;
    /// <summary>Событие изменения раунда</summary>
    public static Action onChangedRound;
    /// <summary>Событие инициализации системы</summary>
    public static Action onInitSystem;
    /// <summary>Событие начала действий юнитов</summary>
    public static Action OnStartAction;
    #endregion
    #region Properties
    /// <summary>
    /// Текущая очередь хода (true - враги, false - союзники)
    /// </summary>
    public static bool GetQueue
    {
        get { return _isEnemyAction; }
    }

    /// <summary>
    /// Текущий номер раунда
    /// </summary>
    public static int GetRound
    {
        get { return _round; }
    }

    /// <summary>
    /// Номер финального раунда
    /// </summary>
    public static int GetEndRound
    {
        get { return _endRound; }
    }

    /// <summary>
    /// Статус победителя (0 - союзники, 1 - враги, 2 - ничья)
    /// </summary>
    public static int GetWinner
    {
        get { return WinnerStatus; }
    }

    /// <summary>
    /// Инициализирует статические поля значениями из инспектора
    /// </summary>
    private void initInspectorFields()
    {
        _round = round;
        _roundSteps = roundSteps;
        _endRound = endRound;
    }

    /// <summary>
    /// Список союзных юнитов
    /// </summary>
    public static List<GameObject> GetAllienGroup
    {
        get { return allienGroup; }
    }

    /// <summary>
    /// Список вражеских юнитов
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
    /// Вычисляет суммарное HP группы юнитов
    /// </summary>
    /// <param name="Units">Список юнитов</param>
    /// <returns>Суммарное HP группы</returns>
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
    /// Добавляет союзного юнита в соответствующую группу
    /// </summary>
    /// <param name="allien">Объект союзного юнита</param>
    private void UpdateAllienGroup(GameObject allien)
    {
        allienGroup.Add(allien);
    }

    /// <summary>
    /// Добавляет вражеского юнита в соответствующую группу
    /// </summary>
    /// <param name="enemy">Объект вражеского юнита</param>
    private void UpdateEnemyGroup(GameObject enemy)
    {
        enemyGroup.Add(enemy);
    }

    /// <summary>
    /// Определяет победителя на основе оставшегося HP
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
    /// Обновляет списки юнитов при инициализации нового юнита
    /// </summary>
    /// <param name="unit">Объект юнита</param>
    /// <param name="isEnemy">Принадлежность к вражеской группе</param>
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
    /// Регистрирует все существующие юниты на сцене
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
    /// Переход к следующему раунду
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
    /// Переход к следующему шагу в раунде
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
    /// Смена очереди хода между командами
    /// </summary>
    private void ChangeQueue()
    {
        _isEnemyAction = !_isEnemyAction;
    }

    /// <summary>
    /// Запускает действия юнитов в текущем шаге
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