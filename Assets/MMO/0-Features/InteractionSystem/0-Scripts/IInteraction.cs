namespace MMO.InteractionSystem
{
    public interface IInteraction
    {
        int ObjectId { get; }
        float InteractionTime { get; }
        void Interact(int clientId, bool asServer);
    }
}