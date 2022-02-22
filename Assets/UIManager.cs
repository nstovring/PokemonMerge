using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public SpawnManager m_spawnmanager;
    public RaceTrack m_racetrack;

    public TextMeshProUGUI m_scoretext;

    public TextMeshProUGUI m_buyText;

    public TextMeshProUGUI m_buy_price_Text;
    // Start is called before the first frame update

    public static UIManager Instance;

    private SpawnManager m_spawnManager;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        m_spawnManager = GameManager.Instance.m_spawnManager;
    }

    public void BuyPokemon()
    {
        Pokedex_ScriptableObject.PokemonData p = m_spawnManager.Pokedex.GetPokemon(m_spawnManager.unlockedLevel + 1);
        if (GameManager.Instance.Buy(p.pokemon_price))
        {
            m_spawnManager.SpawnPokemon();
            p.pokemon_price +=(int)((p.pokemon_price) * 0.14f);
            m_spawnManager.Pokedex.SetPokemon(m_spawnManager.unlockedLevel + 1, p);
        }
        //increase price
        //check if affordable
        //check if space available
    }

    public void BuyNextLevel()
    {
        m_spawnManager.LevelUp();
    }

    // Update is called once per frame
    void Update()
    {
        m_scoretext.text = "SCORE : " + (int) GameManager.Instance.Score;
        m_buyText.text = "Lvl : " + GameManager.Instance.m_spawnManager.spawnLevel;
        Pokedex_ScriptableObject.PokemonData p = m_spawnManager.Pokedex.GetPokemon(m_spawnManager.unlockedLevel + 1);
        float value = m_spawnManager.Pokedex.GetPokemon(m_spawnManager.unlockedLevel + 1).pokemon_index + 1;
        m_buy_price_Text.text = "Price : " + p.pokemon_price;
    }
}
