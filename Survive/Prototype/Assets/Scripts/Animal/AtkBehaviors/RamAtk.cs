using System.Collections;
using Player;
using UnityEngine;


// if i am doint this why do i need the so i can make the prefab and and store it in the animal base and it can do attack .ExecuteAttack 
namespace SurviveTheWild.Animal.AtkBehaviors
{
    public class RamAtk : Attacks
    {

        [SerializeField] private float runSpeed;
        private bool drawCircle;
        public override void Execute()
        {
            StartCoroutine(RamAttack());
        }

        public IEnumerator RamAttack()
        {
            var warningRadius = 10f;
            float stopOffset = 1.5f;
            var waitBeforeNextRam = .5f;

            Transform player = PlayerRepository.instance.GetPlayerTransform();

            while (animal.CurrentHealth > 100 * 0.10f)
            {
                var reached = false;

                while (!reached) // initial attack 
                {
                    var dirToPlayer = (player.position - transform.position).normalized;
                    var stopPoint = player.position - dirToPlayer * stopOffset;

                    if (RetPosOnNv.TryGetNavMeshPoint(stopPoint, out Vector3 navStopPoint))
                        animal.MoveTo(navStopPoint, null, runSpeed, true);

                    if (Vector3.Distance(transform.position, navStopPoint) <= animal.agent.stoppingDistance + 0.2f)
                        reached = true;

                    yield return null;
                }

                animal.DoDamage(attack);
                yield return new WaitForSeconds(1f); // get Away from player 


                var circlePoint = GetRandomPointOnCircle(transform.position, warningRadius);
                drawCircle = true;
                if (!RetPosOnNv.TryGetNavMeshPoint(circlePoint, out Vector3 navCirclePoint))
                {
                    Debug.Log("pos failed Remove Animal");
                    animal.GetOutOfChunk();
                    yield break;
                }

                bool arrivedCircle = false;
                animal.MoveTo(navCirclePoint, () => arrivedCircle = true, runSpeed);

                while (!arrivedCircle)
                {
                    float dist = Vector3.Distance(transform.position, player.position);
                    if (dist > warningRadius)
                    {
                        Debug.Log("Remove Animal ");
                        animal.GetOutOfChunk();
                        yield break;
                    }

                    yield return null;
                }

                yield return new WaitForSeconds(waitBeforeNextRam);
                Debug.Log("wait before next");
            }
        }

        private Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(
                center.x + Mathf.Cos(rad) * radius,
                center.y,
                center.z + Mathf.Sin(rad) * radius
            );
            RetPosOnNv.TryGetNavMeshPoint(pos, out Vector3 navMeshHit);
            if (navMeshHit == Vector3.zero)
            {
                Debug.Log("return pos zero");
            }

            return navMeshHit;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (drawCircle)
                Gizmos.DrawSphere(transform.position, 10f);
        }
    }
}