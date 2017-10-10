using System;
using System.Net;
using System.Text;
using CoreFoundation;
using Foundation;
using UIKit;

// https://forums.xamarin.com/discussion/79198/ios-app-with-voice-over-ip-background-enabled-not-working-properly-in-background-on-ios-10
// https://techmegabyte.wordpress.com/2014/12/10/ios-background-mode-voip-exploring-with-experiments/
namespace X059i_Background_Mode_VoIP
{

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private string ip = "192.168.0.47";
        private int port = 4444;
        private CFReadStream readStream;
        private CFWriteStream writeStream;
        private NSInputStream inputStream;
        private NSOutputStream outputStream;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            CFStream.CreatePairWithSocketToHost(ipEndPoint, out readStream, out writeStream);
            
            inputStream = ObjCRuntime.Runtime.GetNSObject<NSInputStream>(readStream.Handle);
            outputStream = ObjCRuntime.Runtime.GetNSObject<NSOutputStream>(writeStream.Handle);

            inputStream.ServiceType = NSStreamServiceType.VoIP;
            outputStream.ServiceType = NSStreamServiceType.VoIP;
//            // or ?
            inputStream[NSStream.NetworkServiceType] = NSStream.NetworkServiceTypeVoIP;
            outputStream[NSStream.NetworkServiceType] = NSStream.NetworkServiceTypeVoIP;

            inputStream.OnEvent += HandleInputEvent;
            outputStream.OnEvent += HandleOutputEvent;

            outputStream.Schedule(NSRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode); // TODO check
            inputStream.Schedule(NSRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode); // TODO check

//            readStream.EnableEvents(CFRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode);
//            writeStream.EnableEvents(CFRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode);

            outputStream.Open();
            inputStream.Open();

//            UIApplication.SharedApplication.SetKeepAliveTimeout(1, null); // Not supported any more.

            return true;
        }

        public void HandleInputEvent(object sender, NSStreamEventArgs e)
        {
            Console.WriteLine("***** Input ***** " + e.StreamEvent);
            if (e.StreamEvent == NSStreamEvent.HasBytesAvailable)
            {
                var buffer = new byte[1024];
                nint bytesRead = inputStream.Read(buffer, 3);
                var stringRead = Encoding.UTF8.GetString(buffer);
                Console.WriteLine("***** String ***** " + stringRead);
            }
        }

        public void HandleOutputEvent(object sender, NSStreamEventArgs e)
        {
            Console.WriteLine("***** Output ***** " + e.StreamEvent);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }

}