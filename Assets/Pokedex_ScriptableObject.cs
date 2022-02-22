using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokedex", menuName = "ScriptableObjects/Pokedex", order = 0)]
[System.Serializable]
public class Pokedex_ScriptableObject : ScriptableObject
{

    [System.Serializable]
    public class PokemonData
    {
        public string name;
        public int pokemon_index;
        public int pokemon_score;
        public int pokemon_price;

        public Sprite m_sprite;
    }


    public List<Sprite> pokemon_sprites;

    public Sprite GetSprite(int index) { return pokemon_sprites[index]; }

    public PokemonData GetPokemon(int index) { return pokemon[index]; }


    public List<PokemonData> pokemon;

    [ContextMenu("InitializePokedex")]
    public void InitializePokedex()
    {
        pokemon = new List<PokemonData>();
        int index = 0;
        int init_score = 25;
        foreach (Sprite sprite in pokemon_sprites)
        {
            PokemonData data = new PokemonData();
            data.pokemon_index = index;
            data.pokemon_score = init_score;
            data.pokemon_price = (index + 1) * 100;

            data.m_sprite = sprite;
            pokemon.Add(data);

            init_score += init_score;
            index++;
        }
    }

    internal void SetPokemon(int v, PokemonData p)
    {
        pokemon[v] = p;
    }
}
