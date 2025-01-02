using UnityEditor;
using UnityEngine;

public class Gimmick : MonoBehaviour
{
    public Define.GimmickType gimmickType;
    public int hp;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGimmick(Define.GimmickType _type, int _hp, Color _color)
    {
		this.hp = _hp;
		this.gimmickType = _type;
        spriteRenderer.color = _color;
	}

    public void SetColor(Color _color)
    {
		spriteRenderer.color = _color;
	}
}
