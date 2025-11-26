using UnityEngine;
using dutpekmezi;

namespace dutpekmezi
{
    public class TargetIndicator : MonoBehaviour
    {
        private Transform target;
        private Transform center;
        private Camera mainCamera;

        private const float screenEdgePadding = 50f;

        public void Init(Transform target, Transform center)
        {
            this.target = target;
            this.center = center;
            this.mainCamera = Camera.main;
        }

        public void Tick()
        {
            if (target == null || center == null || mainCamera == null)
            {
                Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(this.gameObject);
                return;
            }

            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);
            bool isTargetOnScreen = viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1 && viewportPoint.z > 0;

            if (isTargetOnScreen)
            {
                gameObject.SetActive(false);
                return;
            }
            else
                gameObject.SetActive(true);

            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
            Vector3 centerScreenPos = mainCamera.WorldToScreenPoint(center.position);

            float minX = screenEdgePadding;
            float maxX = Screen.width - screenEdgePadding;
            float minY = screenEdgePadding;
            float maxY = Screen.height - screenEdgePadding;

            screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
            screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

            screenPos.z = 1f;

            transform.position = mainCamera.ScreenToWorldPoint(screenPos);

            Vector3 indicatorDirection = screenPos - centerScreenPos;

            var angle = Mathf.Atan2(indicatorDirection.y, indicatorDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}