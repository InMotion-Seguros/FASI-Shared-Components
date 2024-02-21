using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using InMotionGIT.Common.Extensions;

namespace InMotionGIT.Common.Services.Contracts
{

    [DebuggerDisplay("{PathFullName}")]
    [DataContract()]
    public class info
    {
        private static ParallelOptions _parallelsOptions = new ParallelOptions();

        public info()
        {

        }

        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public DateTime LastWrite { get; set; }

        [DataMember()]
        public long Length { get; set; }

        [DataMember()]
        public bool IsFolder { get; set; }

        [DataMember()]
        public string CheckSum { get; set; }

        [DataMember()]
        public string PathFullName { get; set; }

        [DataMember()]
        public List<info> Childs { get; set; }

        public static info Process(string path, info parent = null)
        {
            _parallelsOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 10;
            DirectoryInfo folderBase = null;
            info root = null;
            info current = null;

            try
            {
                if (!(parent == null))
                {
                    root = parent;
                }
                else
                {
                    root = new info()
                    {
                        Name = Path.GetFileName(path),
                        PathFullName = path,
                        IsFolder = true
                    };
                }
                if (root.Childs == null)
                {
                    root.Childs = new List<info>();
                }

                folderBase = new DirectoryInfo(path);
                FileInfo[] files = folderBase.GetFiles();
                Parallel.ForEach(files, _parallelsOptions, file => root.Childs.Add(new info()
                {
                    Name = file.Name,
                    PathFullName = file.FullName,
                    CheckSum = Helpers.MD5Helper.CheckSum(file.FullName),
                    IsFolder = false,
                    LastWrite = file.LastWriteTime.ToLocalTime(),
                    Length = file.Length
                }));

                DirectoryInfo[] folders = folderBase.GetDirectories();

                Parallel.ForEach(folders, _parallelsOptions, directory =>
                {
                    current = new info()
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

}