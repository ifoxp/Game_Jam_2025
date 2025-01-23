namespace _Scripts.DataModel
{
    public class GameResourceContainer
    {
        public GameResourcesType ResourceType { get; set; }
        public int Quantity { get; set; }

        public GameResourceContainer(GameResourcesType resourceType, int quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
        }
    }
}