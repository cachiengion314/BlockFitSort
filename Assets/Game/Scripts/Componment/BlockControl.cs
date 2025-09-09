using UnityEngine;

public class BlockControl : MonoBehaviour, ISpriteRenderer
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void SetColor(int ColorValue)
    {
       spriteRenderer.sprite = RendererSystem.Instance.GetSpriteByColorValue(ColorValue);
    }
}
