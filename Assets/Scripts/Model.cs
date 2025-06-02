using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Основной класс модели юнита, управляющий его характеристиками и поведением
/// </summary>
public class Model : MonoBehaviour
{
    // Основные характеристики юнита
    [SerializeField] float Health = 200;          // Текущее здоровье юнита
    [SerializeField] float Speed = 15;            // Скорость перемещения
    [SerializeField] float Defence = 20;          // Защита (уменьшает получаемый урон)
    [SerializeField] float AttackForce = 15;      // Сила атаки
    [SerializeField] float RotationSpeed = 10f;   // Скорость поворота

    // Ссылки на трансформы и компоненты
    [SerializeField] Transform ShootFrom;         // Точка выстрела
    [SerializeField] Transform CompTransf;        // Ссылка на трансформ объекта
    [SerializeField] SuperHitBar hitBar;          // Ссылка на шкалу супер-удара

    // Префабы
    [SerializeField] GameObject SuperHitPref;     // Префаб области супер-удара
    [SerializeField] GameObject Bullet;           // Префаб пули

    // Флаги состояний
    bool isActionsStart = false;      // Флаг разрешения действий
    bool isSuperHitStart = false;     // Флаг активации супер-удара
    bool isShoot = false;             // Флаг необходимости выстрела
    bool isMoving = false;            // Флаг движения
    bool _isActionComplete = true;    // Флаг завершения текущего действия

    // Целевые позиции
    Vector2 TargetPosition;           // Целевая позиция для движения
    Vector2 BulletTargetPosition;     // Целевая позиция для выстрела

    /// <summary>
    /// Текущее здоровье юнита (не может быть меньше 0)
    /// </summary>
    public float GetHP
    {
        get
        {
            if (Health > 0) return Health;
            else return 0;
        }
    }

    /// <summary>
    /// Восстанавливает здоровье юнита
    /// </summary>
    /// <param name="count">Количество восстанавливаемого здоровья</param>
    public void Heal(float count)
    {
        Health += count;
    }

    /// <summary>
    /// Модифицирует защиту юнита
    /// </summary>
    /// <param name="count">Значение изменения защиты</param>
    public void ModifyDefense(float count)
    {
        Defence += count;
    }

    /// <summary>
    /// Модифицирует силу атаки юнита
    /// </summary>
    /// <param name="count">Значение изменения силы атаки</param>
    public void ModifyAttack(float count)
    {
        AttackForce += count;
    }

    /// <summary>
    /// Наносит урон юниту с учетом защиты
    /// </summary>
    /// <param name="Damage">Исходный урон</param>
    public void setDamage(float Damage)
    {
        Health = Health - (Damage - Defence);
        SuperHitBar.AddPoint(gameObject.GetComponent<Unit>().IsEnemy);
    }

    private void Awake()
    {
        CompTransf = GetComponent<Transform>();
    }

    /// <summary>
    /// Изменяет статус готовности юнита
    /// </summary>
    private void ChangeReadyStatus()
    {
        _isActionComplete = !_isActionComplete;
    }

    /// <summary>
    /// Статус готовности юнита к новым действиям
    /// </summary>
    public bool GetReadyStatus
    {
        get { return _isActionComplete; }
    }

    /// <summary>
    /// Устанавливает целевую позицию для движения
    /// </summary>
    /// <param name="newTarget">Новая целевая позиция</param>
    public void SetTarget(Vector2 newTarget)
    {
        if (TargetPosition == newTarget) return;
        TargetPosition = newTarget;
        isMoving = true;
    }

    /// <summary>
    /// Устанавливает целевую позицию для выстрела
    /// </summary>
    /// <param name="newTarget">Новая целевая позиция</param>
    public void SetBulletTarget(Vector2 newTarget)
    {
        BulletTargetPosition = newTarget;
        isShoot = true;
    }

    /// <summary>
    /// Активирует супер-удар
    /// </summary>
    public void SetSuperHit()
    {
        isSuperHitStart = true;
    }

    /// <summary>
    /// Разрешает выполнение действий
    /// </summary>
    public void StartActions()
    {
        isActionsStart = true;
    }

    /// <summary>
    /// Запрещает выполнение действий
    /// </summary>
    public void StopActions()
    {
        isActionsStart = false;
    }

    /// <summary>
    /// Поворачивает юнита к целевой позиции
    /// </summary>
    /// <param name="targetPosition">Целевая позиция</param>
    private void RotateTowardsTarget(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)CompTransf.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        CompTransf.rotation = Quaternion.Slerp(
            CompTransf.rotation,
            targetRotation,
            RotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Осуществляет шаг движения к цели
    /// </summary>
    private void MovementStep()
    {
        if (!isMoving) return;

        _isActionComplete = false;
        Vector2 toTarget = TargetPosition - (Vector2)CompTransf.position;
        float distance = toTarget.magnitude;

        RotateTowardsTarget(TargetPosition);

        if (distance <= 1)
        {
            isMoving = false;
            ChangeReadyStatus();
            return;
        }

        Vector2 currentDirection = toTarget.normalized;
        CompTransf.position += (Vector3)(currentDirection * Speed * Time.deltaTime);
    }

    /// <summary>
    /// Выполняет выстрел
    /// </summary>
    private void Shooting()
    {
        ChangeReadyStatus();
        if (isShoot)
        {
            var bullet = Instantiate(Bullet, ShootFrom);
            var bullet_conf = bullet.GetComponent<bullet>();
            bullet_conf.AttackForce = AttackForce;

            bullet_conf.isEnemy = gameObject.GetComponent<Unit>().IsEnemy;
            bullet.GetComponent<Rigidbody2D>().AddForce(
                (BulletTargetPosition - (Vector2)ShootFrom.position).normalized * 25,
                ForceMode2D.Impulse);

            Destroy(bullet, 30f);
            isShoot = false;
        }
        ChangeReadyStatus();
    }

    /// <summary>
    /// Активирует супер-удар
    /// </summary>
    private void SuperHit()
    {
        ChangeReadyStatus();
        if (isSuperHitStart)
        {
            var superHitArea = Instantiate(SuperHitPref, CompTransf.position, CompTransf.rotation);
            Destroy(superHitArea, 5f);
            isSuperHitStart = false;
        }
        ChangeReadyStatus();
    }

    /// <summary>
    /// Обрабатывает смерть юнита
    /// </summary>
    public void Death()
    {
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isActionsStart)
        {
            MovementStep();
            Shooting();
            SuperHit();
            Death();
        }
    }
}