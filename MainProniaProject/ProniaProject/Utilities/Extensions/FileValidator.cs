using ProniaProject.Models;

namespace ProniaProject.Utilities.Extensions
{
    public static class FileValidator
    {
        public static bool CheckFileType(this IFormFile file,string type)
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }

        public static bool ValidateSize(this IFormFile file,int kb)
        {
            if (file.Length < kb * 1024)
            {
                return true;
            }
            return false;
        }

        //public static void CreateFile(this IFormFile file1,string root,params string[] folders)
        //{
        //    string fileName = Guid.NewGuid().ToString() + file1.FileName;
        //    string path = Path.Combine(_env.WebRootPath, "assets", "images", "slider", fileName);
        //    FileStream file = new FileStream(path, FileMode.Create);
        //    await slide.Photo.CopyToAsync(file);
        //    file.Close();
        //}
    }
}
