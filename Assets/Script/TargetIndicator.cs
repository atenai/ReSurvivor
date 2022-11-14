using UnityEngine;
using UnityEngine.UI;

//�Q�l�T�C�g
//https://qiita.com/o8que/items/46e486f62bdf05c29559

[RequireComponent(typeof(RectTransform))]//RectTransform���A�^�b�`���Ă��Ȃ�������A�^�b�`����
public class TargetIndicator : MonoBehaviour
{
    [SerializeField] Transform target;//�^�[�Q�b�g(��or�A�C�e��)�̍��W
    [SerializeField] Image arrow;//�����̉摜

    Camera mainCamera;
    RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;//���C���J�������擾
        rectTransform = this.GetComponent<RectTransform>();//���̃X�N���v�g���A�^�b�`���Ă���Q�[���I�u�W�F�N�g��RectTransform���擾
    }

    void LateUpdate()
    {
        if (target == null || target.gameObject.activeSelf == false)
        {
            transform.Find("Image_Arrow").gameObject.SetActive(false);
            return;
        }
        else
        {
            transform.Find("Image_Arrow").gameObject.SetActive(true);
        }

        //���[�g(Canvas)�̃X�P�[���l���擾����
        float canvasScale = transform.root.localScale.z;
        var screenCenter = 0.5f * new Vector3(Screen.width, Screen.height);//�X�N���[���̒��S�_�S�̂̒�������0.5�������邱�Ƃŋ��߂�[�i��)2 * 0.5 = 1]

        //(��ʒ��S�����_(0,0)�Ƃ���)�^�[�Q�b�g�̃X�N���[�����W�����߂�
        var targetIndicatorPos = mainCamera.WorldToScreenPoint(target.position) - screenCenter;//���[���h���W�@���@�X�N���[�����W��

        //�J��������ɂ���^�[�Q�b�g�̃X�N���[�����W�́A��ʊO�Ɉړ�����
        if (targetIndicatorPos.z < 0f)//�X�N���[�����W.z��0�ȉ��̎����g�����s����
        {
            targetIndicatorPos.x = -targetIndicatorPos.x;
            targetIndicatorPos.y = -targetIndicatorPos.y;

            //�J�����Ɛ����ȃ^�[�Q�b�g�̃X�N���[�����W��␳����
            //pos.y == 0f�Ȃ�true
            if (Mathf.Approximately(targetIndicatorPos.y, 0f) == true)//Mathf.Approximately�͕��������_�����m�����������ǂ������r�������ꍇ�A���҂̍���������l�ȓ��Ȃ�قړ�����
            {
                targetIndicatorPos.y = -screenCenter.y;
            }
        }

        //��ʒ[�̕\���ʒu�𒲐�����
        //UI���W�n�̒l���X�N���[�����W�n�̒l�ɕϊ�����
        var halfSize = 0.5f * canvasScale * rectTransform.sizeDelta;//rectTransform�̃T�C�Y�̔��������߂�

        //Mathf.Max�͈�ԑ傫�����l��Ԃ��AMathf.Abs�͐�Βl��Ԃ�//�܂肱���͐�Βl�̒��ōł��傫���������߂�
        //���̐�Βl�����߂�̂��Ƃ����Ɖ�ʒ[��1��-1�����Ȃ��ׁA
        float edgeOfScreen = Mathf.Max(Mathf.Abs(targetIndicatorPos.x / (screenCenter.x - halfSize.x)), Mathf.Abs(targetIndicatorPos.y / (screenCenter.y - halfSize.y)));//��ʒ[�̃e�L�X�g�\���ʒu�����߂�

        //�^�[�Q�b�g�̃X�N���[�����W����ʊO�Ȃ�A��ʒ[�ɂȂ�悤�ɒ�������
        //��ʒ[��1��-1�Ȃ�true
        bool isOffscreen = (targetIndicatorPos.z < 0f || 1f < edgeOfScreen);
        if (isOffscreen == true)
        {
            targetIndicatorPos.x = targetIndicatorPos.x / edgeOfScreen;
            targetIndicatorPos.y = targetIndicatorPos.y / edgeOfScreen;
        }

        //�X�N���[�����W�n�̒l��UI���W�n�̒l�ɕϊ�����
        rectTransform.anchoredPosition = targetIndicatorPos / canvasScale;//�^�[�Q�b�g�̃X�N���[�����W������UI��RectTransform�ɓ����

        //�^�[�Q�b�g�̃X�N���[�����W����ʊO�Ȃ�A�^�[�Q�b�g�̕������w������\������
        arrow.enabled = isOffscreen;
        if (isOffscreen == true)
        {
            arrow.rectTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(-targetIndicatorPos.x, targetIndicatorPos.y) * Mathf.Rad2Deg);//������]�����Ă����鏈�������Ă���
        }
    }
}