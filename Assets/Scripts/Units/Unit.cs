using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentZ { set; get; }
    public int id;

    public int noDed = 0;
    public int noNotDed = 0;

    public bool isDed;
    public bool move = false;
    public bool attack = false;
    public Vector3 requestedPos;
    private NavMeshAgent agent;
    public Animator animator;
    protected int gridSize = GameManager.gridSize;
    public Collider coll;



    public void Start()
    {
        coll = this.gameObject.GetComponent<Collider>();
        animator = this.gameObject.GetComponent<Animator>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        //gameObject.GetComponent<NavMeshAgent>().enabled = false;
        if (isDed)
        {
            gameObject.tag = "ded";
            noDed += 1;
        }
        else if (!isDed)
        {
            gameObject.tag = "notDed";
            noNotDed += 1;
        }

    }


    public void SetPosition(int x, int z)
    {
        CurrentX = x;
        CurrentZ = z;
    }

    void FixedUpdate()
    {
       // StartCoroutine(EnableNav());
        if (move && !agent.pathPending)
        {
            print("moving");
            if (agent.remainingDistance <= 0.1 / 30)
            {
                animator.ResetTrigger("Move");
                move = false;
                GameManager.unitBusy = false;
            }
        }

    }

    //IEnumerator EnableNav()
    //{
    //    yield return new WaitForSeconds(20f);
    //    gameObject.GetComponent<NavMeshAgent>().enabled = true;
    //}
    IEnumerator StartMovementTimer()
    {
        yield return new WaitForSeconds(5f);
        if (move)
        {
            gameObject.transform.position = requestedPos;
            animator.ResetTrigger("Move");
            move = false;
            GameManager.unitBusy = false;
        }
        
    }
    public void moveCharacter(Vector3 requestedPos)
    {
        print("reqpos: "+requestedPos);
        agent.destination = requestedPos;
        animator.SetTrigger("Move");
        GameManager.unitBusy = true;
        move = true;
        StartCoroutine(StartMovementTimer());
    }
    public virtual void DealDamage(GameObject c) { }

    public virtual bool[,] PossibleMove()
    {
        return new bool[gridSize,gridSize];
    }

    public virtual bool[,] PossibleAttack()
    {
        return new bool[gridSize, gridSize];
    }

    public bool InsideOfBounds(int x, int z)
    {
        if (x >= gridSize || x < 0 || z >= gridSize || z < 0) return false;
        else return true;
    }
    public void AttemptMove(int x, int z, ref bool[,] r)
    {
        
        Unit c;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(GameManager.GetTileCenter(x, z), out hit, 0.03f, NavMesh.AllAreas))
        {
            if (InsideOfBounds(x, z))
            {
                c = GameManager.Instance.Units[x, z];
                if (c == null)
                {
                    r[x, z] = true;
                }
                else if (isDed != c.isDed)
                {
                    r[x, z] = false;
                }

            }
        }
    }

    public void AttemptAttack(int x, int z, ref bool[,] r)
    {
        Unit c;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.transform.position, out hit, 0.03f, NavMesh.AllAreas))
        {
            if (InsideOfBounds(x, z))
            {
                c = GameManager.Instance.Units[x, z];
                if (c == null)
                {
                    r[x, z] = false;
                }
                else if (isDed != c.isDed)
                {
                    r[x, z] = true;
                }

            }
        }


    }
    public void Attack(int x, int z)
    {
        Unit c;
        c = GameManager.Instance.Units[x, z];
        if (isDed != c.isDed)
        {
            this.transform.LookAt(c.transform);
            StartCoroutine(RunAnimation(3f, "Attack", c.gameObject));
        }
    }
    private IEnumerator RunAnimation(float animationLength, string trigger, GameObject c)
    {
        GameManager.unitBusy = true;
        animator.SetTrigger(trigger);
        yield return new WaitForSeconds(animationLength);
        DealDamage(c);
        //healthbar damage
        animator.ResetTrigger(trigger);
        GameManager.unitBusy = false;

    }


}
