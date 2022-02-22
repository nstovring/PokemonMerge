using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Pokemon : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;
    public int pokemon_index;
    public int pokemon_score;
    public int pokemon_price;
 

    public SpawnField m_spawnField;
    public SpriteRenderer m_spriteRenderer;
    public Cart m_cart;

    public string GUID;

    private void Awake()
    {
        GUID = Guid.NewGuid().ToString();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {

    }

    public void SetSprite(Sprite s)
    {
        Debug.Log("Setting Sprite" + s.ToString());

        if (m_spriteRenderer == null)
            m_spriteRenderer = GetComponent<SpriteRenderer>();

        sprite = s;
        m_spriteRenderer.sprite = s;
    }


    Vector2 clickOffset;
    private void OnMouseDown()
    {
        Vector2 mouseVel = Input.mousePosition;
        Vector2 currentPosition = Camera.main.WorldToScreenPoint(transform.position);
        clickOffset = currentPosition - mouseVel;
    }

    private void OnMouseDrag()
    {
        Vector2 mouseVel = Input.mousePosition;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(mouseVel + clickOffset);
        newPosition.z = 0;
        transform.position = newPosition;
    }

    public LayerMask raycastMask;

    private void OnMouseUp()
    {

        SpawnField field = SpawnManager.Instance.GetSpawnField(transform.position);
        
        bool race = GameManager.Instance.RacePokemon(this, transform.position);
        if (race)
        {
            Debug.Log("Racing Pokekom", this);
            return;
        }

        bool merge = GameManager.Instance.MergePokemon(this, field);
        if (merge)
        {
            Debug.Log("Merging Pokekom", this);
            return;
        }

        bool moveToField = GameManager.Instance.MovePokemonToField(this, field);
        if (moveToField)
        {
            Debug.Log("Moving Pokekom", this);
            return;
        }

        //reset position
        iTween.MoveTo(gameObject, m_spawnField.transform.position, 0.5f);
    }

    public void MoveToField(SpawnField spawnField)
    {
        m_spawnField.RemovePokemonFromField();
        spawnField.AddPokemonToField(this);
        transform.position = m_spawnField.transform.position;
    }

    void Merge()
    {

    }

    public void RemoveFromTrack()
    {
        RaceTrack.Instance.RemoveCart(m_cart);
    }



    internal float GetScore()
    {
        return pokemon_score;
    }
}
