using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SpawnManager m_spawnManager;
    public UIManager m_UiManager;
    public RaceTrack m_racetrack;

    public static GameManager Instance;

    public float Score = 2000;
    [Range(0,1f)]
    public float Base_Speed = 0.5f;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_spawnManager = SpawnManager.Instance;
        m_racetrack = RaceTrack.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Update Score
        float race_point_sum = m_racetrack.GetScore();

        //Score +=(race_point_sum * Time.deltaTime);


    }

    public void AddScore(int score)
    {
        Score += score;
    }

    public bool Buy(int price)
    {
        if(Score - price > 0)
        {
            Score -= price;
            return true;
        }
        return false;
    }

    public bool MovePokemonToField(Pokemon pokemon, SpawnField target_field)
    {

        //Move
        if (target_field.is_Occupied == false || pokemon.GUID == target_field.m_pokemon.GUID)
        {
            pokemon.RemoveFromTrack();

            pokemon.m_spawnField.RemovePokemonFromField();
            target_field.AddPokemonToField(pokemon);

            return true;
        }
        return false;

    }

    public bool MergePokemon(Pokemon pokemon, SpawnField target_field)
    {
        if (target_field.m_pokemon == null)
            return false;

        if (pokemon.GUID == target_field.m_pokemon.GUID)
            return false;

        //Merge
        if (target_field.is_Occupied)
        {
            if (target_field.m_pokemon.pokemon_index == pokemon.pokemon_index)
            {
                pokemon.RemoveFromTrack();
                pokemon.m_spawnField.RemovePokemonFromField();
                Pokemon newPokemon = SpawnManager.Instance.Merge(target_field.m_pokemon, pokemon, target_field);
                //field.AddPokemonToField(newPokemon);
                return newPokemon != null;
            }
        }

        return false;
    }

    public bool RacePokemon(Pokemon pokemon, Vector2 point)
    {
        //Race
        if (m_racetrack.IsPointOnTrack(point))
        {
            MoveToTrack(pokemon, point);
            return true;
        }

        return false;
    }

    public void MoveToTrack(Pokemon p, Vector2 point)
    {
        Vector3 p_pos = p.transform.position;
        if (p.m_cart.cart == null)
        {
            p.m_cart = m_racetrack.AddCartAtPosition(point);
            if(p.m_cart == null)
                Debug.LogError("No Cart instantiated!");

            p.m_cart.m_pokemon = p;
            p.transform.parent = p.m_cart.cart;
        }
        else
        {
            m_racetrack.SetCartAtPosition(p.m_cart, point);
        }

        p.transform.localPosition = Vector3.zero;
    }
}
