namespace ImageSample.Model
{
    public class PickedImage : IImage
    {
        public byte[] RawImage { get; set; }
        public string Name { get; set; }
    }
}