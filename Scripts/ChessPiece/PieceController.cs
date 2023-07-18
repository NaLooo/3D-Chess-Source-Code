using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;

public class PieceController : MonoBehaviour, ISelectable
{
    public Movement mover;
    public Position position;
    public bool isWhite;
    public bool isFirstMove = true;
    public char notation;

    Animator animator;
    string currentAnim;
    Outlinable outlinable;
    bool selected = false;
    bool attacking = false;
    bool moving = false;

    Position target = null;
    PieceController enemy = null;

    public Sprite image;
    public EventHandler commandCallback;

    void Awake()
    {
        animator = GetComponent<Animator>();
        outlinable = GetComponent<Outlinable>();
        outlinable.enabled = false;
        mover = Movement.GetMover(notation);

        currentAnim = Idle();
        animator.SetBool(currentAnim, true);
        commandCallback = CallBackHolder;
    }

    void Update()
    {
        if (attacking) return;
        if (target != null)
        {
            var step = Constants.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.ToVector(), step);
            if (enemy != null && Vector3.Distance(transform.position, enemy.position.ToVector()) < 2.001)
            {
                attacking = true;
                transform.LookAt(enemy.position.ToVector());
                moving = false;
                SwitchAnimation(Attack());
            }
            else if (Vector3.Distance(transform.position, target.ToVector()) < 0.001)
            {
                mover.MoveCallBack(this, target);
                position.Copy(target);
                transform.position = target.ToVector();
                transform.rotation = isWhite ? Constants.front : Constants.back;
                target = null;
                moving = false;
                SwitchAnimation(Idle());

                GameManager.animating.Remove(this);
                commandCallback?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    void OnEnable()
    {
        SwitchAnimation((Idle()));
    }

    public void Init(bool isWHite, int x, int y)
    {
        this.isWhite = isWHite;
        position = new Position(x, y);
        if (notation == 'p' && y != 6) isFirstMove = false;
        if (notation == 'P' && y != 1) isFirstMove = false;
        image = SpriteHolder.dict[notation];
    }

    public void MoveTo(Position pos, bool skip = true)
    {
        position.Copy(pos);
        isFirstMove = false;
        if (skip)
        {
            transform.position = pos.ToVector();
        }
        else
        {
            GameManager.animating.Add(this);
            target = pos;
            enemy = Board.GetPiece(pos.tuple);
            if (enemy != null && isWhite == enemy.isWhite) enemy = null;
            SwitchAnimation(Move());
        }
    }

    public void EnPassant(Position pos, PieceController piece)
    {
        GameManager.animating.Add(this);
        target = pos;
        enemy = piece;
        SwitchAnimation(Move());
    }

    public HashSet<(int, int)> Select()
    {
        if ((isWhite != GameManager.playerIsWhite) && !GameManager.sandBox) return null;
        selected = true;
        outlinable.enabled = true;
        return mover.GetMoves(this);
    }

    public void UnSelect()
    {
        selected = false;
        outlinable.enabled = false;
    }

    public HashSet<(int, int)> GetAttacking()
    {
        return mover.GetAttacking(this);
    }

    public PieceController GetPiece()
    {
        return this;
    }

    public Block GetBlock()
    {
        return Board.GetBlock(position.tuple);
    }

    void OnMouseEnter()
    {
        if ((isWhite != GameManager.playerIsWhite && !GameManager.sandBox) || selected) return;
        outlinable.enabled = true;
    }

    void OnMouseExit()
    {
        if ((isWhite != GameManager.playerIsWhite && !GameManager.sandBox) || selected) return;
        outlinable.enabled = false;
    }

    void SwitchAnimation(string nextAnim)
    {
        animator.SetBool(currentAnim, false);
        currentAnim = nextAnim;
        animator.SetBool(currentAnim, true);
    }

    string Idle()
    {
        return "Idle" + UnityEngine.Random.Range(0, 4).ToString();
    }

    string Move()
    {
        moving = true;
        return "Move";
    }

    string Attack()
    {
        return "Attack" + UnityEngine.Random.Range(0, 2).ToString();
    }

    string Die()
    {
        return "Die" + UnityEngine.Random.Range(0, 2).ToString();
    }

    public void GoDie()
    {
        SwitchAnimation(Die());
        GameManager.animating.Add(this);
    }

    public void Died()
    {
        gameObject.SetActive(false);
        GameManager.animating.Remove(this);
        GameManager.AddCapturedPiece(this);
    }

    public void Hit()
    {
        if (enemy == null) return;
        enemy.GoDie();
    }

    public void AttackFinished()
    {
        attacking = false;
        enemy = null;
        SwitchAnimation(Move());
    }

    public void RandomIdle()
    {
        if (moving) return;
        SwitchAnimation(Idle());
    }

    public void CallBackHolder(object o, EventArgs e)
    {

    }
}
