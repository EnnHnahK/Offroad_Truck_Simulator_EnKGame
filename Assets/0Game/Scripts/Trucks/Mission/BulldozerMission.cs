using DG.Tweening;
using UnityEngine;

public class BulldozerMission : MonoBehaviour
{
    [Header("Bulldozer Controller")]
    [SerializeField] private Transform bulldozerAim;
    [SerializeField] private Transform bulldozerBlade, item;
    
    //Cache
    private Quaternion _curAimLocalRos, _curBladeLocalRos;

    [SerializeField] private MeshRenderer itemMeshRenderer;

    private void Awake()
    {
        _curAimLocalRos = bulldozerAim.localRotation;
        _curBladeLocalRos = bulldozerBlade.localRotation;
    }

    public void SetMatWithEnvironment(Material mat)
    {
        itemMeshRenderer.sharedMaterial = mat;
    }
    
    [Button]
    private void Test()
    {
        ScoopingPile(true);
    }

    public void ScoopingPile(bool isReceiving)
    {
        Sequence sequence = DOTween.Sequence();
        if (isReceiving)
        {
            sequence.AppendCallback(() =>
            {
                Quaternion targetRotation = Quaternion.Euler(340, bulldozerAim.localRotation.eulerAngles.y, bulldozerAim.localRotation.eulerAngles.z);
                bulldozerAim.DOLocalRotate(targetRotation.eulerAngles, 1f);
                item.gameObject.SetActive(true);
                item.DOScaleZ(1.15f, 3f).From(0);
            }).AppendInterval(1f).AppendCallback(() =>
            {
                Quaternion targetRotation = Quaternion.Euler(270, bulldozerBlade.localRotation.eulerAngles.y, bulldozerBlade.localRotation.eulerAngles.z);
                bulldozerBlade.DOLocalRotate(targetRotation.eulerAngles, 1f);
            });
        }
        else
        {
            sequence.AppendCallback(() =>
            {
                bulldozerBlade.DOLocalRotateQuaternion(_curBladeLocalRos, 1f);
                item.DOScaleZ(0, 3f).OnComplete(() =>
                {
                    item.gameObject.SetActive(false);
                });
            }).AppendInterval(1f).AppendCallback(() =>
            {
                bulldozerAim.DOLocalRotateQuaternion(_curAimLocalRos, 1f);
            });
        }
    }
}
