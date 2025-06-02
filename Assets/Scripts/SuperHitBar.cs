using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///  ласс дл€ управлени€ системой накоплени€ очков "Super Hit".
/// ѕозвол€ет накапливать очки дл€ двух сторон (враг/союзник) и определ€ть,
/// когда накоплено достаточно очков дл€ специального действи€.
/// </summary>
public class SuperHitBar : MonoBehaviour
{
    // —татические пол€ дл€ хранени€ значений очков
    private static int _enemyPoints = 0;      // “екущие очки врага
    private static int _allianPoints = 0;     // “екущие очки союзника
    private static int _maxPoints = 100;      // ћаксимальное количество очков дл€ специального действи€
    private static int _step = 25;            // Ўаг приращени€ очков

    // —ериализуемые пол€ дл€ настройки в инспекторе Unity
    [SerializeField] private int inspectorEnemyPoints = 0;    // Ќачальные очки врага (настраиваетс€ в инспекторе)
    [SerializeField] private int inspectorAllianPoints = 0;   // Ќачальные очки союзника (настраиваетс€ в инспекторе)
    [SerializeField] private int inspectorMaxPoints = 100;    // ћакс. очки (настраиваетс€ в инспекторе)
    [SerializeField] private int inspectorStep = 25;          // Ўаг приращени€ (настраиваетс€ в инспекторе)

    /// <summary>
    /// —обытие, вызываемое при изменении количества очков любой из сторон
    /// </summary>
    public static Action onChangedPoints;

    /// <summary>
    /// ¬озвращает текущий шаг приращени€ очков
    /// </summary>
    public static int GetStep
    {
        get { return _step; }
    }

    /// <summary>
    /// ѕолучает текущее количество очков дл€ указанной стороны
    /// </summary>
    /// <param name="IsEnemy">≈сли true - возвращает очки врага, иначе - очки союзника</param>
    /// <returns>“екущее количество очков</returns>
    public static int GetPoints(bool IsEnemy)
    {
        if (IsEnemy) return _enemyPoints;
        return _allianPoints;
    }

    /// <summary>
    /// ¬озвращает максимальное количество очков, необходимое дл€ специального действи€
    /// </summary>
    public static int GetMaxPoints
    {
        get { return _maxPoints; }
    }

    private void Awake()
    {
        // »нициализаци€ статических полей значени€ми из инспектора
        _enemyPoints = inspectorEnemyPoints;
        _allianPoints = inspectorAllianPoints;
        _maxPoints = inspectorMaxPoints;
        _step = inspectorStep;
    }

    /// <summary>
    /// ƒобавл€ет очки указанной стороне (обратите внимание на инвертированную логику)
    /// </summary>
    /// <param name="IsEnemy">≈сли true - добавл€ет очки союзнику, иначе - врагу</param>
    public static void AddPoint(bool IsEnemy)
    {
        if (IsEnemy) _allianPoints += _step;
        else _enemyPoints += _step;
        onChangedPoints?.Invoke();
    }

    /// <summary>
    /// ѕровер€ет, накоплено ли достаточно очков дл€ специального действи€.
    /// ≈сли да - снимает необходимое количество очков и возвращает true.
    /// </summary>
    /// <param name="IsEnemy">≈сли true - провер€ет очки врага, иначе - союзника</param>
    /// <returns>True, если очков достаточно дл€ специального действи€</returns>
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