using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ����� ��� ���������� �������� ���������� ����� "Super Hit".
/// ��������� ����������� ���� ��� ���� ������ (����/�������) � ����������,
/// ����� ��������� ���������� ����� ��� ������������ ��������.
/// </summary>
public class SuperHitBar : MonoBehaviour
{
    // ����������� ���� ��� �������� �������� �����
    private static int _enemyPoints = 0;      // ������� ���� �����
    private static int _allianPoints = 0;     // ������� ���� ��������
    private static int _maxPoints = 100;      // ������������ ���������� ����� ��� ������������ ��������
    private static int _step = 25;            // ��� ���������� �����

    // ������������� ���� ��� ��������� � ���������� Unity
    [SerializeField] private int inspectorEnemyPoints = 0;    // ��������� ���� ����� (������������� � ����������)
    [SerializeField] private int inspectorAllianPoints = 0;   // ��������� ���� �������� (������������� � ����������)
    [SerializeField] private int inspectorMaxPoints = 100;    // ����. ���� (������������� � ����������)
    [SerializeField] private int inspectorStep = 25;          // ��� ���������� (������������� � ����������)

    /// <summary>
    /// �������, ���������� ��� ��������� ���������� ����� ����� �� ������
    /// </summary>
    public static Action onChangedPoints;

    /// <summary>
    /// ���������� ������� ��� ���������� �����
    /// </summary>
    public static int GetStep
    {
        get { return _step; }
    }

    /// <summary>
    /// �������� ������� ���������� ����� ��� ��������� �������
    /// </summary>
    /// <param name="IsEnemy">���� true - ���������� ���� �����, ����� - ���� ��������</param>
    /// <returns>������� ���������� �����</returns>
    public static int GetPoints(bool IsEnemy)
    {
        if (IsEnemy) return _enemyPoints;
        return _allianPoints;
    }

    /// <summary>
    /// ���������� ������������ ���������� �����, ����������� ��� ������������ ��������
    /// </summary>
    public static int GetMaxPoints
    {
        get { return _maxPoints; }
    }

    private void Awake()
    {
        // ������������� ����������� ����� ���������� �� ����������
        _enemyPoints = inspectorEnemyPoints;
        _allianPoints = inspectorAllianPoints;
        _maxPoints = inspectorMaxPoints;
        _step = inspectorStep;
    }

    /// <summary>
    /// ��������� ���� ��������� ������� (�������� �������� �� ��������������� ������)
    /// </summary>
    /// <param name="IsEnemy">���� true - ��������� ���� ��������, ����� - �����</param>
    public static void AddPoint(bool IsEnemy)
    {
        if (IsEnemy) _allianPoints += _step;
        else _enemyPoints += _step;
        onChangedPoints?.Invoke();
    }

    /// <summary>
    /// ���������, ��������� �� ���������� ����� ��� ������������ ��������.
    /// ���� �� - ������� ����������� ���������� ����� � ���������� true.
    /// </summary>
    /// <param name="IsEnemy">���� true - ��������� ���� �����, ����� - ��������</param>
    /// <returns>True, ���� ����� ���������� ��� ������������ ��������</returns>
    public static bool IsHavePoints(bool IsEnemy)
    {
        if (IsEnemy)
        {
            if (_enemyPoints >= _maxPoints)
            {
                _enemyPoints -= _maxPoints;
                onChangedPoints?.Invoke();
                return true;
            }
        }
        else
        {
            if (_allianPoints >= _maxPoints)
            {
                _allianPoints -= _maxPoints;
                onChangedPoints?.Invoke();
                return true;
            }
        }
        return false;
    }
}