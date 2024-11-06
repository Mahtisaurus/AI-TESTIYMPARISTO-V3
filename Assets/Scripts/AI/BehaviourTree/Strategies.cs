using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTrees
{

    public interface IStrategy
    {
        Node.Status Process();
        void Reset()
        {
            // Noop
        }
    }

    // Patrol through a list of waypoints
    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        List<Transform> patrolPoints;
        readonly float patrolSpeed;
        int currentIndex;
        bool isPathCalculated;
        bool waypointsArranged = false;

        // Constructor
        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, float patrolSpeed = 2f)
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
            this.patrolSpeed = patrolSpeed;
        }

        public Node.Status Process()
        {
            // Gone through all the points = success
            if (currentIndex == patrolPoints.Count) return Node.Status.Success;

            if (waypointsArranged == false)
            {
                ArrangeWaypoints();
            }

            // Next point, navmeshagent functionality
            var target = patrolPoints[currentIndex];
            agent.SetDestination(target.position);
            //Vector3 tempTargetLookAt = new Vector3(target.position.x, 0, target.position.z);
            //entity.LookAt(tempTargetLookAt);

            if (agent.remainingDistance < 0.2f)
            {
                currentIndex++;
            }

            //if(isPathCalculated && agent.remainingDistance < 0.2f)
            //{
            //    currentIndex++;
            //    isPathCalculated = false;
            //}

            //if (agent.pathPending)
            //{
            //    isPathCalculated = true;
            //}

            return Node.Status.Running;
        }

        // Arranges the patrolPoints so that we Start Patrolling from the closest point to us and advance to the next ones in their original order.
        public void ArrangeWaypoints()
        {
            if (patrolPoints == null || patrolPoints.Count == 0)
            {
                Debug.LogWarning("No patrol points available.");
                return;
            }

            Transform closestTransform = null;
            float closestDistance = Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < patrolPoints.Count; i++)
            {
                float distance = Vector3.Distance(entity.position, patrolPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = patrolPoints[i];
                    closestIndex = i;
                }
            }

            List<Transform> arrangedWaypoints = new List<Transform>();

            arrangedWaypoints.Add(closestTransform);

            for (int i = 1; i < patrolPoints.Count; i++)
            {
                int index = (closestIndex + i) % patrolPoints.Count;
                arrangedWaypoints.Add(patrolPoints[index]);
            }

            patrolPoints = arrangedWaypoints;
            waypointsArranged = true;
        }

        public void Reset()
        {
            currentIndex = 0;
            waypointsArranged = false;
        }
    }

    // Search Area
    public class SearchStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        List<Vector3> searchPoints = new();
        readonly float searchSpeed;
        int currentIndex;
        bool isPathCalculated;

        [Header("Supporting Functions")]
        public Vector3 walkPoint;
        private bool walkPointSet;
        public float walkPointRange = 5f;
        readonly LayerMask whatIsGround;

        public SearchStrategy(Transform entity, NavMeshAgent agent, LayerMask whatIsGround, float searchSpeed = 2f)
        {
            this.entity = entity;
            this.agent = agent;
            this.searchSpeed = searchSpeed;
            this.whatIsGround = whatIsGround;
        }

        public Node.Status Process()
        {
            // Calculate search points
            if (!walkPointSet && currentIndex == 0)
            {
                searchPoints = SearchPoints(entity);
            }

            // Gone through all the points = success
            if (currentIndex == searchPoints.Count)
            {
                AIManager.Instance.enemy.GetComponent<AI_BEHAVE>().CanSearchPlayer(false);
                return Node.Status.Success;
            }

            if (AIManager.Instance.aIVision.canSeePlayer == true)
            {
                return Node.Status.Success;
            }

            // Next point, navmeshagent functionality
            var target = searchPoints[currentIndex];
            agent.speed = searchSpeed;
            agent.SetDestination(target);
            //Vector3 tempTargetLookAt = new Vector3(target.x, 0, target.z);
            //entity.LookAt(tempTargetLookAt);

            if (agent.remainingDistance < 0.3f)
            {
                currentIndex++;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            currentIndex = 0;
            searchPoints.Clear();
            walkPointSet = false;
        }


        #region SUPPORTING FUNCTION

        // Clears list of search points, then adds search point vector3's around an area
        private List<Vector3> SearchPoints(Transform originPoint)
        {
            searchPoints.Clear();

            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
            {
                walkPointSet = false;

               
                float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
                float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

                walkPoint = new Vector3(originPoint.position.x + randomX, 0, originPoint.position.z + randomZ);

                

                //while (walkPointSet == false)
                //{
                //    // Calculate random point in range
                //    float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
                //    float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

                //    walkPoint = new Vector3(originPoint.position.x + randomX, 0, originPoint.position.z + randomZ);

                //    // Checking if walkpoint is on ground and not out of map! Also this maybe needs to be modified or groundLayer modified to be only on ground and not objects as well
                //    if (Physics.Raycast(walkPoint, -originPoint.up, 2f, whatIsGround))
                //    {
                //        walkPointSet = true;
                //    }
                //}

                searchPoints.Add(walkPoint);
            }

            walkPointSet = true;

            return searchPoints;
        }
        #endregion
    }

    // Chase Target
    public class ChaseStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform target;
        readonly float moveSpeed;
        readonly float stoppingDistance;

        // Constructor
        public ChaseStrategy(Transform entity, NavMeshAgent agent, Transform target, float moveSpeed = 4f, float stoppingDistance = 1.5f)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
            this.moveSpeed = moveSpeed;
            this.stoppingDistance = stoppingDistance;
        }

        public Node.Status Process()
        {
            if (AIManager.Instance.aIVision.canSeePlayer == false)
            {
                agent.speed = moveSpeed;
                AIManager.Instance.enemy.GetComponent<AI_BEHAVE>().CanSearchPlayer(true);
                AIManager.Instance.enemy.GetComponent<AI_BEHAVE>().CanChase(false);
                return Node.Status.Success;
            }

            agent.speed = moveSpeed;
            agent.SetDestination(target.position);
            agent.stoppingDistance = stoppingDistance;
            entity.LookAt(target);

            return Node.Status.Running;
        }

        public void Reset()
        {
            agent.speed = 2f;
            agent.stoppingDistance = 0f;
        }
    }
    
    // Attack in front
    public class AttackStrategy : IStrategy
    {
        readonly Action attack;
        readonly Transform entity;
        readonly Transform target;

        public AttackStrategy(Action attack, Transform entity, Transform target)
        {
            this.attack = attack;
            this.entity = entity;
            this.target = target;
        }

        public Node.Status Process()
        {
            entity.LookAt(target);
            attack();

            return Node.Status.Success;
        }
    }

    // Set the navmesh agent to move to target and success when at target
    public class MoveToTarget : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform target;
        readonly float moveSpeed;
        bool isPathCalculated;

        // Constructor
        public MoveToTarget(Transform entity, NavMeshAgent agent, Transform target, float moveSpeed = 2f)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
            this.moveSpeed = moveSpeed;
        }

        public Node.Status Process()
        {
            if (Vector3.Distance(entity.position, target.position) < 1f)
            {
                agent.speed = moveSpeed;
                return Node.Status.Success;
            }

            agent.speed = moveSpeed;
            agent.SetDestination(target.position);
            entity.LookAt(target);

            if (agent.pathPending)
            {
                isPathCalculated = true;
            }
            return Node.Status.Running;
        }

        public void Reset()
        {
            isPathCalculated = false;
            agent.speed = 2f;
        }
    }

    // Logical IF statement. Func true = success, Func false = failure
    public class Condition : IStrategy
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;
    }

    // Do something. Fire and forget.
    public class ActionStrategy : IStrategy
    {
        readonly Action doSomething;

        public ActionStrategy(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Node.Status Process()
        {
            doSomething();
            return Node.Status.Success;
        }
    }
}