namespace SequencedAction
{
    public interface IActionAbortedOnDestinationReached
    {
        void OnDestinationReached();
    }

    public static class IActionAbortedOnDestinationReachedHelper
    {
        public static void ProcessOnDestinationReachedEvent(SequencedActionPlayer SequencedActionPlayer)
        {
            foreach (var currentAction in SequencedActionPlayer.GetCurrentActions(true))
            {
                var destinationReachedListeningNode = currentAction as IActionAbortedOnDestinationReached;
                if (destinationReachedListeningNode != null) destinationReachedListeningNode.OnDestinationReached();
            }
        }
    }
}