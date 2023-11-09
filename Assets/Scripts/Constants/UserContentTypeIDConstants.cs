
namespace TDL.Constants
{
    public class UserContentTypeIDConstants
    {
        public const int Image = 100;
        public const int Video = 200;
    }

    public class UserContentTypeNameConstants
    {
        public const string Image = "Image";
        public const string Video = "Video";

        public static string GetName(int id)
        {
            switch (id)
            {
                case UserContentTypeIDConstants.Image:
                    return Image;
            
                case UserContentTypeIDConstants.Video:
                    return Video;
            }

            return null;
        }
    }   
}