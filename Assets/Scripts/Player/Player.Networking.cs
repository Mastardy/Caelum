using Unity.Netcode;

public partial class Player
{
    [ClientRpc]
    public void GatherResourcesClientRpc(string resourceType, int resourceGathered)
    {
        switch (resourceType)
        {
            case "Tree":
                Wood += resourceGathered;
                break;
            case "Stone":
                Stone += resourceGathered;
                break;
        }
    }
}
