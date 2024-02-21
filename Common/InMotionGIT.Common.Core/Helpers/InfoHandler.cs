using InMotionGIT.Common.Helpers;
using InMotionGIT.Common.Core.Extensions;

namespace InMotionGIT.Common.Core.Helpers;

public class Info
{
    public static Common.Domain.General.info Process(string path, InMotionGIT.Common.Domain.General.info parent = null)
    {
        var _parallelsOptions = new ParallelOptions();
        _parallelsOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 10;
        DirectoryInfo folderBase = null;
        Common.Domain.General.info root = null;
        Common.Domain.General.info current = null;

        try
        {
            if (!(parent == null))
            {
                root = parent;
            }
            else
            {
                root = new Common.Domain.General.info()
                {
                    Name = Path.GetFileName(path),
                    PathFullName = path,
                    IsFolder = true
                };
            }
            if (root.Childs == null)
            {
                root.Childs = new List<Common.Domain.General.info>();
            }

            folderBase = new DirectoryInfo(path);
            FileInfo[] files = folderBase.GetFiles();
            Parallel.ForEach(files, _parallelsOptions, file => root.Childs.Add(new Common.Domain.General.info()
            {
                Name = file.Name,
                PathFullName = file.FullName,
                CheckSum = MD5Handler.CheckSum(file.FullName),
                IsFolder = false,
                LastWrite = file.LastWriteTime.ToLocalTime(),
                Length = file.Length
            }));

            DirectoryInfo[] folders = folderBase.GetDirectories();

            Parallel.ForEach(folders, _parallelsOptions, directory =>
            {
                current = new Common.Domain.General.info()
                {
                    Name = Path.GetFileName(directory.Name),
                    PathFullName = directory.FullName,
                    IsFolder = true,
                    LastWrite = directory.LastWriteTime.ToLocalTime()
                };
                root.Childs.Add(current);
                Process(directory.FullName, current);
            });
        }
        catch (Exception ex)
        {
            root.Name = "Falla: " + ex.Message;
        }

        if (root.Childs.IsNotEmpty())
        {
            root.Childs = root.Childs.OrderByDescending(c => c.PathFullName).ToList();
        }

        return root;
    }
}