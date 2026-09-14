namespace HeroServer
{
    public class RegisterMemoryRequest : RegisterPostRequest
    {
        public Memory Memory { get; set; }

        public RegisterMemoryRequest()
        {
        }

        public RegisterMemoryRequest(Memory memory)
        {
            Memory = memory;
        }
    }
}
