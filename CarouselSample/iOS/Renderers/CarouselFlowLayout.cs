using System;
using System.Collections.Generic;
using System.Text;
using CoreAnimation;
using Foundation;
using UIKit;

namespace CarouselSample.iOS.Renderers
{
    public class CarouselFlowLayout : UICollectionViewFlowLayout
    {
        #region Constructors

        public CarouselFlowLayout()
            : base()
        {

        }

        public CarouselFlowLayout(IntPtr handle)
            : base(handle)
        {

        }

        #endregion

        #region Parent override

        public override void PrepareLayout()
        {
            base.PrepareLayout();
            ScrollDirection = UICollectionViewScrollDirection.Horizontal;
        }

        #endregion
    }
}
