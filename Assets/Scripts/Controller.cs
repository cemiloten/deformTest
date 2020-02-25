using System;
using Deform;
using Deform.Masking;
using UnityEngine;

[RequireComponent(typeof(Deformable))]
public class Controller : MonoBehaviour
{
    [SerializeField] private int smoothingIterations;

    private Deformable _deformable;
    private TransformDeformer _transformDeformer;
    private OffsetDeformer _offsetDeformer;
    private VerticalGradientMask _gradientMask;

    private Vector3 _touchStart;
    private Vector3 _touchCurrent;
    private bool _touching;

    private static LayerMask _groundLayerMask;

    private void Start()
    {
        _groundLayerMask = LayerMask.GetMask("Ground");

        _deformable = GetComponent<Deformable>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 50f, _groundLayerMask))
                return;

            _touchStart = hit.point;
            _touchStart.y = _deformable.transform.position.y;

            // Create and initialize transform deformer
            var transformDeformer = new GameObject("TransformDeformer");
            transformDeformer.transform.position = _touchStart;
            _transformDeformer = transformDeformer.AddComponent<TransformDeformer>();
            _transformDeformer.Target = _transformDeformer.transform;
            _deformable.AddDeformer(_transformDeformer);

            // Create and initialize gradient mask
            var gradientMask = new GameObject("GradientMask");
            Transform gradientMaskTransform = gradientMask.transform;
            gradientMaskTransform.parent = _transformDeformer.transform;
            gradientMaskTransform.position = _touchStart;
            _gradientMask = gradientMask.AddComponent<VerticalGradientMask>();

            _gradientMask.Factor = 1f;
            _gradientMask.Invert = true;
            _deformable.AddDeformer(_gradientMask);

            // Create and initialize Look at
            gradientMask.AddComponent<LookAtTarget>().InitializeTarget(_deformable.transform.position);

            _touching = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _deformable.RemoveDeformer(_transformDeformer);
            _deformable.RemoveDeformer(_gradientMask);
            Destroy(_transformDeformer.gameObject);

            _gradientMask.GetComponent<LookAtTarget>().DestroyTarget();
            Destroy(_gradientMask.gameObject);

            Mesh meshCopy = Instantiate(_deformable.GetMesh());

            Vector3[] vertices = null;

            for (int i = 0; i < smoothingIterations; ++i)
                vertices = SmoothFilter.laplacianFilter(meshCopy.vertices, meshCopy.triangles);

            meshCopy.vertices = vertices;
            _deformable.ChangeMesh(meshCopy);

            _touching = false;
        }

        if (!_touching)
            return;

        Ray currentRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(currentRay, out RaycastHit currentHit, 50f, _groundLayerMask))
            return;

        _touchCurrent = currentHit.point;
        _touchCurrent.y = _deformable.transform.position.y;
        _transformDeformer.transform.position = _touchCurrent;
    }
}