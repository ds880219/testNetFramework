namespace bel.web.api.core.objects.Interfaces
{
    using System.Collections.Generic;
    using System.Drawing;

    using bel.web.api.core.objects.Imaging;
    using bel.web.api.core.objects.Product;

    using ImageMagick;

    public interface IImageEffect
    {
        IMagickImage DoEmbroidery(IMagickImage input, Color? bgColor = null, bool bgRemoved = false, int numberOfColors = 16);

        IMagickImage DoDebossing(IMagickImage input, bool isText = false);

        void RemoveColors(Bitmap processedBitmap, List<Color> colorsToRemove);

        byte[] ApplyImageEffect(
            IMagickImage originalImage,
            objects.ImageEffects.Effect effect,
            ProductDetails productDetails, bool removeBg);

        bool ApplyImageEffect(
            IMagickImage originalImage,
            ImageActions imageActions,
            ProductDetails productDetails,
            bool newAsset,
            int actualColorCountReduced,
            ref Color? backgroundColor,
            bool bgRemoved,
            ref IMagickImage processedImage);

        IMagickImage ProcessBackground(ImageActions actions, bool newAsset, Color? backgroundColor, IMagickImage image);

        Bitmap DoBgRemovalProcess(
            ImageActions actions,
            bool newAsset,
            Color? backgroundColor,
            Bitmap img,
            bool removeOnTransparent = false);

        Bitmap ProcessLightBmp(
            ImageActions internalImageActions,
            Bitmap img1,
            bool newAsset,
            int lowerThreshold = 35,
            int upperThreshold = 255);

        Bitmap ProcessDarkBmp(ImageActions internalImageActions, Bitmap bitmap, bool newAsset);

        bool ApplyOneColor(ImageActions imageActions, bool newAsset, Color? backgroundColor,
            ref IMagickImage processedImage);
    }
}
