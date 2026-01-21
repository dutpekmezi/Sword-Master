using UnityEngine;

public class FracturedChest : MonoBehaviour
{
    public Transform[] cellTransforms;

    private Vector3[] initialLocalPositions;
    private Quaternion[] initialLocalRotations;

    private void Awake()
    {
        CacheInitialTransforms();
    }

    public void Init()
    {
        CacheInitialTransforms();
        ResetCells();
    }

    private void CacheInitialTransforms()
    {
        if (cellTransforms == null || cellTransforms.Length == 0)
        {
            return;
        }

        if (initialLocalPositions != null && initialLocalPositions.Length == cellTransforms.Length)
        {
            return;
        }

        initialLocalPositions = new Vector3[cellTransforms.Length];
        initialLocalRotations = new Quaternion[cellTransforms.Length];

        for (int i = 0; i < cellTransforms.Length; i++)
        {
            var cellTransform = cellTransforms[i];
            if (cellTransform == null) continue;

            initialLocalPositions[i] = cellTransform.localPosition;
            initialLocalRotations[i] = cellTransform.localRotation;
        }
    }

    private void ResetCells()
    {
        if (cellTransforms == null || cellTransforms.Length == 0)
        {
            return;
        }

        for (int i = 0; i < cellTransforms.Length; i++)
        {
            var cellTransform = cellTransforms[i];
            if (cellTransform == null) continue;

            cellTransform.localPosition = initialLocalPositions[i];
            cellTransform.localRotation = initialLocalRotations[i];

            if (cellTransform.TryGetComponent<Rigidbody2D>(out var body))
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
                body.Sleep();
            }
        }
    }
}
