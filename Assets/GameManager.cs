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
    public float cur_max_exp = 100;
    public float Experience_points = 0;
    [Range(0,1f)]
    public float Base_Speed = 0.5f;


    //Which pokemons can we buy?
    public int unlocked_buy_Level = 0;
    //What is the highest ranked pokemon available?
    [Min(1)]
    public int current_max_level = 1;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_UiManager = UIManager.Instance;
        m_spawnManager = SpawnManager.Instance;
        m_racetrack = RaceTrack.Instance;

        UpdatePriceRanking(0);
    }

    void UpdatePriceRanking(int rank)
    {
        //Initialize experience needed to level up
        UpdateMaxPokemonRank(rank);
        //Experience required tolevel up is a function of the score of the pokemon "one" rank above the current max
        // get max pokeon rank + 1
        Pokedex_ScriptableObject.PokemonData p = m_spawnManager.GetMaxRankPlus1Pokemon();
        cur_max_exp = p.pokemon_price;
    }

    public void UpdateMaxPokemonRank(int index)
    {
        current_max_level = index;
        unlocked_buy_Level = Mathf.RoundToInt((current_max_level / 2) + 0.01f);
        Debug.Log("Current Max level is : " + index);
        Debug.Log("Unlocked buy level is :" + unlocked_buy_Level);
    }


    // Update is called once per frame
    void Update()
    {
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
                
                if (newPokemon.pokemon_index > current_max_level)
                {
                    UpdateMaxPokemonRank(newPokemon.pokemon_index);
                }
                AddXP(newPokemon.pokemon_score);

                //field.AddPokemonToField(newPokemon);
                return newPokemon != null;
            }
        }

        return false;
    }

    public void AddXP(float value)
    {
        if (Experience_points + value > cur_max_exp)
        {

        }

        Experience_points += value;

        m_UiManager.SetRaceTrackProgressBarProgress(Mathf.Min(Experience_points / cur_max_exp, 100));

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
