using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Класс, представляющий базовую единицу (юнит) в игре.
/// Управляет состоянием юнита, его действиями и взаимодействием с системой очереди ходов.
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField] private bool _isEnemy = false;         // Флаг, определяющий принадлежность юнита к вражеской команде
    [SerializeField] private uint ActionCounts = 3;         // Количество доступных действий за ход
    [SerializeField] private Controller Controller;         // Ссылка на контроллер управления юнитом
    [SerializeField] private UnityEvent onUnitActionsStart; // Событие, вызываемое при начале действий юнита

    /// <summary>
    /// Принадлежность юнита к вражеской команде
    /// </summary>
    public bool IsEnemy
    {
        get { return _isEnemy; }
    }

    /// <summary>
    /// Статическое событие инициализации юнита
    /// Параметры:
    /// GameObject - ссылка на объект юнита
    /// bool - принадлежность к вражеской команде
    /// </summary>
    public static Action<GameObject, bool> onInitUnit;

    private void Awake()
    {
        Init();
        QueueCheck();
    }

    /// <summary>
    /// Уменьшает количество доступных действий юнита.
    /// Если действия закончились - отключает контроллер и сбрасывает счетчик.
    /// </summary>
    public void ChangeFreeActions()
    {
        ActionCounts--;
        if (ActionCounts == 0)
        {
            Controller.enabled = false;
            ActionCounts = 3;
        }
        Debug.Log($"Name:{gameObject.name} actions free {ActionCounts}");
    }

    /// <summary>
    /// Проверяет, совпадает ли текущая очередь хода с принадлежностью юнита.
    /// Включает/выключает контроллер в зависимости от результата проверки.
    /// </summary>
    public void QueueCheck()
    {
        Debug.Log($"CheckQueue:{gameObject.name}");
        if (RoundSystem.GetQueue == _isEnemy)
        {
            Controller.enabled = true;
        }
        else
        {
            Controller.enabled = false;
        }
    }

    /// <summary>
    /// Инициализирует юнит, вызывая событие onInitUnit
    /// </summary>
    private void Init()
    {
        onInitUnit?.Invoke(gameObject, _isEnemy);
        Debug.Log($"Initiation {gameObject.name}");
    }

    /// <summary>
    /// Запускает действия юнита, переключая состояние контроллера и вызывая событие onUnitActionsStart
    /// </summary>
    public void StartUnitActions()
    {
        Controller.ChangeControllerStatus();
        onUnitActionsStart?.Invoke();
        Controller.ChangeControllerStatus();
    }

    private void OnDisable()
    {
        // Отписываемся от событий при отключении объекта
        RoundSystem.onChangedQueue -= QueueCheck;
        RoundSystem.OnStartAction -= StartUnitActions;
    }
}