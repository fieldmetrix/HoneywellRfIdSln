using Messenger;

namespace HoneywellRfIdShared.Messaging
{
    public class PubSubHandler
    {
        // Implement with IEventAggregator (if Prism is Used)  
        // Implement with MessageCenter (if Xamarin.Forms is Used) 
        // Implement with Mvvm Cross Messenger (if MvvmCross is used)

        // Currently using Messenger.Cross (extracted from MvvmCross)

        //Singleton (Use Dependency injection in whatever framework is used in App)
        private static IMessenger _messenger = new MessengerHub();

        public static IMessenger GetInstance()
        {
            return _messenger;
        }
    }
}