using UnityEngine;

public class Line : MonoBehaviour
{
    SpriteRenderer spriteRenderer;



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetLine(Vector3 pos1, Vector3 pos2)
    {
        transform.position = (pos2 + pos1) / 2;
        transform.localScale = new Vector3((pos2 - pos1).magnitude, 0.1f, 1f);

        float angle = Vector3.SignedAngle(transform.right, pos1 - pos2, Vector3.forward);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ChangeWeightColorByValue(float value)
    {
        spriteRenderer.color = Program.WeightColorChange(value);
    }
}
