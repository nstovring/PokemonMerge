using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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

    public UIDocument scene_ui_document;
    public ProgressBar racetrack_progress_bar;
    private void Awake()
    {
        Instance = this;
        var root = scene_ui_document.rootVisualElement;
        racetrack_progress_bar = root.Q<ProgressBar>("racetrack-level-progress");
    }
    void Start()
    {
        m_spawnManager = GameManager.Instance.m_spawnManager;
    }

    public void BuyPokemon()
    {
        int index = 0;
        Pokedex_ScriptableObject.PokemonData p = m_spawnManager.GetBuyablePokemon(out index);
        //check if affordable
        if (GameManager.Instance.Buy(p.pokemon_price))
        {
            //check if space available
            m_spawnManager.SpawnPokemon();
            //increase price
            p.pokemon_price +=(int)((p.pokemon_price) * 0.14f);
            m_spawnManager.Pokedex.SetPokemon(index, p);
        }
    }

    public void SetRaceTrackProgressBarProgress(float value)
    {
        Debug.Log("XP Value :" + value);
        racetrack_progress_bar.lowValue += value* 10;
    }

 
    // Update is called once per frame
    void Update()
    {
        m_scoretext.text = "SCORE : " + (int) GameManager.Instance.Score;
        m_buyText.text = "Lvl : " + GameManager.Instance.unlocked_buy_Level;

        Pokedex_ScriptableObject.PokemonData p = m_spawnManager.GetBuyablePokemon();
        m_buy_price_Text.text = "Price : " + p.pokemon_price;
    }
}
