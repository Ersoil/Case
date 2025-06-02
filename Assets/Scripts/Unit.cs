using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// �����, �������������� ������� ������� (����) � ����.
/// ��������� ���������� �����, ��� ���������� � ��������������� � �������� ������� �����.
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField] private bool _isEnemy = false;         // ����, ������������ �������������� ����� � ��������� �������
    [SerializeField] private uint ActionCounts = 3;         // ���������� ��������� �������� �� ���
    [SerializeField] private Controller Controller;         // ������ �� ���������� ���������� ������
    [SerializeField] private UnityEvent onUnitActionsStart; // �������, ���������� ��� ������ �������� �����

    /// <summary>
    /// �������������� ����� � ��������� �������
    /// </summary>
    public bool IsEnemy
    {
        get { return _isEnemy; }
    }

    /// <summary>
    /// ����������� ������� ������������� �����
    /// ���������:
    /// GameObject - ������ �� ������ �����
    /// bool - �������������� � ��������� �������
    /// </summary>
    public static Action<GameObject, bool> onInitUnit;

    private void Awake()
    {
        Init();
        QueueCheck();
    }

    /// <summary>
    /// ��������� ���������� ��������� �������� �����.
    /// ���� �������� ����������� - ��������� ���������� � ���������� �������.
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
    /// ���������, ��������� �� ������� ������� ���� � ��������������� �����.
    /// ��������/��������� ���������� � ����������� �� ���������� ��������.
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
    /// �������������� ����, ������� ������� onInitUnit
    /// </summary>
    private void Init()
    {
        onInitUnit?.Invoke(gameObject, _isEnemy);
        Debug.Log($"Initiation {gameObject.name}");
    }

    /// <summary>
    /// ��������� �������� �����, ���������� ��������� ����������� � ������� ������� onUnitActionsStart
    /// </summary>
    public void StartUnitActions()
    {
        Controller.ChangeControllerStatus();
        onUnitActionsStart?.Invoke();
        Controller.ChangeControllerStatus();
    }

    private void OnDisable()
    {
        // ������������ �� ������� ��� ���������� �������
        RoundSystem.onChangedQueue -= QueueCheck;
        RoundSystem.OnStartAction -= StartUnitActions;
    }
}