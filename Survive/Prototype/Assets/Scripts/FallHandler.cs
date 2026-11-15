using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SurviveTheWild
{
    public class FallHandler : MonoBehaviour
    {
        [SerializeField] private Collider fallDetector;
        [SerializeField] private float rotationDuration ;
        private GameObject currentGameObject;

        public void ExecuteFall(UnityAction onComplete, GameObject obj)
        {
            print("executing Fall");
            currentGameObject = obj;
            StartCoroutine(RotateTo90(onComplete));
        }


        private IEnumerator RotateTo90(UnityAction Action)
        {
            Quaternion startRotation = currentGameObject.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(90f, 0f, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;

                var t = elapsedTime / rotationDuration;

                currentGameObject.transform.rotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

                yield return null;
            }
            yield return new WaitForSeconds(2);
            currentGameObject.transform.rotation = targetRotation;
            Action?.Invoke();
        }
    }
}