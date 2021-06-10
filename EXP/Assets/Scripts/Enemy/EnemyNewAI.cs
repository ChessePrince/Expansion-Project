using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNewAI : MonoBehaviour
{
	[HideInInspector]
	public NavMeshAgent agent;
	Transform player;

	[Header("Layers")]
	public LayerMask whatIsGround;
	public LayerMask whatIsPlayer;

	[Header("Patrolling")]
	public float patrollingMovSpeed = 8f;
	public float chasingMovSpeed = 13f;
	public float walkPointRange;
	Vector3 walkPoint;
	bool walkPointSet;
	public float sightRange;

	[Header("Attack")]
	public float attackMovSpeed = 2f;
	public float timeBetweenAttacks;
	bool alreadyAttacked;
	public float attackRange;
	Animator anim;

	bool playerInSightRange, playerInAttackRange;
	public enum State
	{
		patrolling,
		chasing,
		attack
	}
	public State state = State.patrolling;

	private void Awake()
	{
		player = GameObject.Find("New Player").transform;
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		//Check for sight and attack range
		playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
		playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

		switch (state)
		{
			case State.patrolling:
				Patroling();
				break;

			case State.chasing:
				ChasePlayer();
				break;
			case State.attack:
				AttackPlayer();
				break;
		}
		if (!playerInSightRange && !playerInAttackRange) state = State.patrolling; //Patroling();
		if (playerInSightRange && !playerInAttackRange) state = State.chasing; //ChasePlayer();
		if (playerInAttackRange && playerInSightRange) state = State.attack; //AttackPlayer();
	}
	private void LateUpdate()
	{
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}

	private void Patroling()
	{
		agent.speed = patrollingMovSpeed;
		if (!walkPointSet) SearchWalkPoint();

		if (walkPointSet)
		{
			agent.SetDestination(walkPoint);
		}

		Vector3 distanceToWalkPoint = transform.position - walkPoint;

		//Walkpoint reached
		if (distanceToWalkPoint.magnitude < 1f)
			walkPointSet = false;
	}
	private void SearchWalkPoint()
	{
		//Calculate random point in range
		float randomZ = Random.Range(-walkPointRange, walkPointRange);
		float randomX = Random.Range(-walkPointRange, walkPointRange);

		walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

		if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
			walkPointSet = true;
	}

	private void ChasePlayer()
	{
		agent.SetDestination(player.position);
	}
	private void AttackPlayer()
	{
		//Make sure enemy doesn't move
		agent.SetDestination(transform.position);
		transform.LookAt(player);

		if (!alreadyAttacked)
		{
			Attack();
			agent.speed = attackMovSpeed;
			alreadyAttacked = true;
			//Invoke(nameof(ResetAttack), timeBetweenAttacks);
		}
	}
	private void Attack()
	{
		Debug.Log("attack");
		anim.Play("EnemyAttack");
	}
	public void ResetAttack()
	{
		alreadyAttacked = false;
		anim.Play("EnemyIdle");
		Debug.Log("reset attackl");
	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, sightRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
