using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SurviveTheWild
{
    public class FallHandler : MonoBehaviour
    {
        [SerializeField] private Collider fallDetector;
        [SerializeField] private float rotationDuration = 1f;
        private bool _onComplete;

        public void ExecuteFall(UnityAction onComplete)
        {
            StartCoroutine(RotateTo90(RotationComplete));
        }

        private void RotationComplete()
        {
            _onComplete = true;
            // fire an event to deactivate this game object and drop resources 
        }

        private IEnumerator RotateTo90(UnityAction Action)
        {
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(90f, 0f, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;

                var t = elapsedTime / rotationDuration;

                transform.rotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

                yield return null;
            }

            transform.rotation = targetRotation;
            Action?.Invoke();
            // after this replace the object with dropables 
        }
    }
}