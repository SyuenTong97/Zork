using Newtonsoft.Json;
using UnityEngine;
using Zork.Common;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LocationText;
    [SerializeField]
    private TextMeshProUGUI ScoreText;
    [SerializeField]
    private TextMeshProUGUI MovesText;
    [SerializeField]
    private UnityInputService InputService;
    [SerializeField]
    private UnityOutputService OutputService;

    private void Awake()
    {
        TextAsset gameJson = Resources.Load<TextAsset>("GameJson");
        _game = JsonConvert.DeserializeObject<Game>(gameJson.text);
        _game.Player.LocationChanged += Player_LocationChanged;
        _game.Player.MovesChanged += Player_MovesChanged;
        _game.Player.RewardsChanged += Player_RewardsChanged;
        _game.Run(InputService, OutputService);
    }

    private void Player_LocationChanged(object sender, Room location)
    {
        LocationText.text = location.Name;
    }

    private void Player_RewardsChanged(object sender, int rewards)
    {
        ScoreText.text = "Score: " + rewards;
    }

    private void Player_MovesChanged(object sender, int moves)
    {
        MovesText.text = "Moves: " + moves;
    }

    private void Start()
    {
        InputService.SetFocus();
        LocationText.text = _game.Player.CurrentRoom.Name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InputService.ProcessInput();
            InputService.SetFocus();
        }

        if (_game.IsRunning == false)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }

    private Game _game;
}
