using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnField : MonoBehaviour
{
    public bool is_Occupied;
    public SpriteRenderer m_spriteRenderer;
    public Pokemon m_pokemon;


    public Color current_color;
    private Color[] color_range = { Color.blue, Color.red };
    


    [Range(0.01f, 1f)]
    public float scale = 1;
    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CreatePokemon(Pokedex_ScriptableObject.PokemonData data)
    {
        m_pokemon = Instantiate(SpawnManager.Instance.Pokemon_prefab, transform.position, Quaternion.identity).GetComponent<Pokemon>();
        m_pokemon.SetSprite(data.m_sprite);
        m_pokemon.pokemon_index = data.pokemon_index;
        m_pokemon.pokemon_score = data.pokemon_score;
        m_pokemon.pokemon_price = data.pokemon_price;
        
        m_pokemon.m_spawnField = this;
        is_Occupied = true;
    }

    public void AddPokemonToField(Pokemon pokemon)
    {
        m_pokemon = pokemon;
        m_pokemon.m_spawnField = this;
        is_Occupied = true;

        iTween.MoveTo(pokemon.gameObject, transform.position, 0.5f);

    }

    public void RemovePokemonFromField()
    {
        m_pokemon = null;
        is_Occupied = false;
    }


    // Update is called once per frame
    void Update()
    {
        int testInt = is_Occupied ? 1 : 0;
        current_color = color_range[testInt];

        transform.localScale = Vector3.one * scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = current_color;
        Gizmos.DrawWireCube(transform.position, Vector3.one * scale);
    }
}
