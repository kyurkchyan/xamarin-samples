using System;
using Android.Graphics;
using MvvmCross.Plugins.Messenger;

namespace ImageSample.Droid.Services
{
    public class CameraImageSelectedMessage : MvxMessage
    {
        public CameraImageSelectedMessage(object sender, Guid receiverID, Bitmap image)
            : base(sender)
        {
            ReceiverID = receiverID;
            Image = image;
        }

        public Guid ReceiverID { get; set; }
        public Bitmap Image { get; set; }
    }
}