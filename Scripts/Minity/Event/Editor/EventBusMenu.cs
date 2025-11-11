using UnityEditor;

namespace Minity.Event.Editor
{
    public static class EventBusMenu
    {
        [MenuItem("Minity/Publish Event", true)]
        private static bool ValidateOpenPublishWindow()
        {
            return EditorApplication.isPlaying;
        }
    
        [MenuItem("Minity/Publish Event")]
        public static void OpenPublishWindow()
        {
            EventArgsPublishWindow.Open();
        }
    }
}