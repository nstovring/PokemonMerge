using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public Pokedex_ScriptableObject Pokedex;
    public Transform Pokemon_prefab;
    public List<SpawnField> SpawnFields;
    public List<Collider2D> Colliders;
    private List<Sprite> SpawnList;

    

    public float spawnRate = 2f;
    private float currentTime = 0;

    private void Awake()
    {
        Instance = this;
        currentTime = Time.time;

        Pokedex.InitializePokedex();

        foreach (SpawnField spawnField in SpawnFields)
        {
            Colliders.Add(spawnField.GetComponent<Collider2D>());
        }
    }


    public Pokedex_ScriptableObject.PokemonData GetMaxRankPokemon()
    {
        Pokedex_ScriptableObject.PokemonData p = Pokedex.GetPokemon(GameManager.Instance.current_max_level);
        return p;   
    }
    public Pokedex_ScriptableObject.PokemonData GetMaxRankPlus1Pokemon()
    {
        Pokedex_ScriptableObject.PokemonData p = Pokedex.GetPokemon(GameManager.Instance.current_max_level + 1);
        return p;
    }

    public Pokedex_ScriptableObject.PokemonData GetBuyablePokemon()
    {
        int index = GameManager.Instance.current_max_level / 2;
        Pokedex_ScriptableObject.PokemonData p = Pokedex.GetPokemon(index);
        return p;
    }

    public Pokedex_ScriptableObject.PokemonData GetBuyablePokemon(out int index)
    {
        index = GameManager.Instance.current_max_level / 2;
        Pokedex_ScriptableObject.PokemonData p = Pokedex.GetPokemon(index);
        return p;
    }
    public void SpawnPokemon()
    {
        SpawnField emptySpot;
        if (CheckIfEmptySpot(out emptySpot))
        {
            FillSpot(emptySpot);
        }
    }

    bool CheckIfEmptySpot(out SpawnField o)
    {
        foreach (SpawnField field in SpawnFields)
        {
            if (field.is_Occupied == false)
            {
                o = field;
                return true;
            }
        }
        o = null;
        return false;
    }

    public SpawnField GetSpawnField(Vector2 point)
    {
        int index = 0;
        foreach (Collider2D collider in Colliders)
        {
            //Collider2D col = GetComponent<Collider2D>();
            if (collider.OverlapPoint(point)){
                return SpawnFields[index];
            }
            index++;
        }

        return null;
    }

    void FillSpot(SpawnField spot)
    {
        //For now get random sprite and assign
        int spriteIndex = Random.Range(0,6);

        Pokedex_ScriptableObject.PokemonData data = Pokedex.GetPokemon(spriteIndex);

        Debug.Log(data.pokemon_index.ToString());
        if (data == null)
            Debug.LogError("No pokemon assigned");

        spot.CreatePokemon(data);
    }

    public Pokemon FillSpot(SpawnField spot, int pokemonIndex)
    {
        Pokedex_ScriptableObject.PokemonData data = Pokedex.GetPokemon(pokemonIndex);

        Debug.Log(data.pokemon_index.ToString());
        if (data == null)
            Debug.LogError("No pokemon assigned");

        return spot.CreatePokemon(data);
    }

    bool Timer()
    {
        if(Time.time - currentTime > spawnRate)
        {
            currentTime = Time.time;
            return true;
        }
        return false;
    }


    public Pokemon Merge(Pokemon a, Pokemon b, SpawnField s_field)
    {
        Debug.Log("Pokemon Index: " + a.pokemon_index);
        
        int newIndex = a.pokemon_index + 1;

        Destroy(a.gameObject);
        Destroy(b.gameObject);

        Debug.Log("Merging Pokemon Index: " + newIndex);

        return FillSpot(s_field, newIndex);
    }
}
