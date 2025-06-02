using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ����� ������ �����, ����������� ��� ���������������� � ����������
/// </summary>
public class Model : MonoBehaviour
{
    // �������� �������������� �����
    [SerializeField] float Health = 200;          // ������� �������� �����
    [SerializeField] float Speed = 15;            // �������� �����������
    [SerializeField] float Defence = 20;          // ������ (��������� ���������� ����)
    [SerializeField] float AttackForce = 15;      // ���� �����
    [SerializeField] float RotationSpeed = 10f;   // �������� ��������

    // ������ �� ���������� � ����������
    [SerializeField] Transform ShootFrom;         // ����� ��������
    [SerializeField] Transform CompTransf;        // ������ �� ��������� �������
    [SerializeField] SuperHitBar hitBar;          // ������ �� ����� �����-�����

    // �������
    [SerializeField] GameObject SuperHitPref;     // ������ ������� �����-�����
    [SerializeField] GameObject Bullet;           // ������ ����

    // ����� ���������
    bool isActionsStart = false;      // ���� ���������� ��������
    bool isSuperHitStart = false;     // ���� ��������� �����-�����
    bool isShoot = false;             // ���� ������������� ��������
    bool isMoving = false;            // ���� ��������
    bool _isActionComplete = true;    // ���� ���������� �������� ��������

    // ������� �������
    Vector2 TargetPosition;           // ������� ������� ��� ��������
    Vector2 BulletTargetPosition;     // ������� ������� ��� ��������

    /// <summary>
    /// ������� �������� ����� (�� ����� ���� ������ 0)
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
    /// ��������������� �������� �����
    /// </summary>
    /// <param name="count">���������� ������������������ ��������</param>
    public void Heal(float count)
    {
        Health += count;
    }

    /// <summary>
    /// ������������ ������ �����
    /// </summary>
    /// <param name="count">�������� ��������� ������</param>
    public void ModifyDefense(float count)
    {
        Defence += count;
    }

    /// <summary>
    /// ������������ ���� ����� �����
    /// </summary>
    /// <param name="count">�������� ��������� ���� �����</param>
    public void ModifyAttack(float count)
    {
        AttackForce += count;
    }

    /// <summary>
    /// ������� ���� ����� � ������ ������
    /// </summary>
    /// <param name="Damage">�������� ����</param>
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
    /// �������� ������ ���������� �����
    /// </summary>
    private void ChangeReadyStatus()
    {
        _isActionComplete = !_isActionComplete;
    }

    /// <summary>
    /// ������ ���������� ����� � ����� ���������
    /// </summary>
    public bool GetReadyStatus
    {
        get { return _isActionComplete; }
    }

    /// <summary>
    /// ������������� ������� ������� ��� ��������
    /// </summary>
    /// <param name="newTarget">����� ������� �������</param>
    public void SetTarget(Vector2 newTarget)
    {
        if (TargetPosition == newTarget) return;
        TargetPosition = newTarget;
        isMoving = true;
    }

    /// <summary>
    /// ������������� ������� ������� ��� ��������
    /// </summary>
    /// <param name="newTarget">����� ������� �������</param>
    public void SetBulletTarget(Vector2 newTarget)
    {
        BulletTargetPosition = newTarget;
        isShoot = true;
    }

    /// <summary>
    /// ���������� �����-����
    /// </summary>
    public void SetSuperHit()
    {
        isSuperHitStart = true;
    }

    /// <summary>
    /// ��������� ���������� ��������
    /// </summary>
    public void StartActions()
    {
        isActionsStart = true;
    }

    /// <summary>
    /// ��������� ���������� ��������
    /// </summary>
    public void StopActions()
    {
        isActionsStart = false;
    }

    /// <summary>
    /// ������������ ����� � ������� �������
    /// </summary>
    /// <param name="targetPosition">������� �������</param>
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
    /// ������������ ��� �������� � ����
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
    /// ��������� �������
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
    /// ���������� �����-����
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
    /// ������������ ������ �����
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