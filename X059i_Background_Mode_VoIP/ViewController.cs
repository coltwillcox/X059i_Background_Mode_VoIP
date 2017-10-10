using System;
using System.Net;
using System.Text;
using CoreFoundation;
using Foundation;
using UIKit;

namespace X059i_Background_Mode_VoIP
{

    public partial class ViewController : UIViewController
    {
        

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            lock (this)
//            {
              

                // UIApplication.SharedApplication.SetKeepAliveTimeout(600, null);
//            }
        }
    }

}