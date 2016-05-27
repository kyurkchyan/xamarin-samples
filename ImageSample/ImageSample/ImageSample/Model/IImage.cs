namespace ImageSample.Model
{
    public interface IImage
    {
        byte[] RawImage { get; set; }
        string Name { get; set; }
    }
}