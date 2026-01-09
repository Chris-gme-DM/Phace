using FishNet.Object;
using FishNet;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        Instance = this;
    }
}