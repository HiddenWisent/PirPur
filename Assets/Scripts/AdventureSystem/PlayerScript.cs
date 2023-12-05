using UnityEngine;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    public void MovePlayer(Vector3 vector)
    {
        transform.DOJump(vector, 3f, 1, 1.2f, false);
    }
}
