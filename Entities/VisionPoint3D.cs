
namespace HeroServer
{
    public class VisionPoint3D
    {
#pragma warning disable IDE1006
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
#pragma warning restore IDE1006

        public VisionPoint3D()
        {
        }

        public VisionPoint3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}