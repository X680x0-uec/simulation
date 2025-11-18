using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class EnemyAllyChaser : MonoBehaviour
// ��
// EnemyBase ���p�����܂�
public class EnemyAllyChaser : EnemyController
{
    // ����AI�����Ŏg���ϐ�
    private int targetnumber;

    // �e�iEnemyBase�j���u�K�������v�Ɩ��߂���
    // ExecuteAI() �̒��g���u�㏑���ioverride�j�v���܂��B
    protected override void ExecuteAI()
    {
        // ���̓G��AI�́uAlly�������_���ɒǂ��v
        if (allys_list.Count != 0)
        {
            // ���t���[���A�^�[�Q�b�g�������_���ɑI�ђ���
            targetnumber = Random.Range(0, allys_list.Count);

            if (allys_list[targetnumber] != null)
            {
                // ���ʋ@�\�� FollowTarget ���g���Ēǔ�
                FollowTarget(allys_list[targetnumber]);
            }
        }
        else
        {
            // ����Ally���S�ł��Ă�����A����Ƀ~�R�V��ǂ��i�ی��j
            FollowTarget(mikoshiObject);
        }
    }
}