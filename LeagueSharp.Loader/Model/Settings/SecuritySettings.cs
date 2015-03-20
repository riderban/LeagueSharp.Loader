using System.Linq;
using System.Net;
using System.Security.Permissions;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class SecuritySettings : ObservableObject
    {
        private SocketPermission _socketPermission;
        private WebPermission _webPermission;
        private FileIOPermission _fileIoPermission;
        private MediaPermission _mediaPermission;

        /// <summary>
        /// Default: None
        /// </summary>
        public SocketPermission SocketPermission
        {
            get { return _socketPermission; }
            set { Set(() => SocketPermission, ref _socketPermission, value); }
        }

        /// <summary>
        /// Default: None
        /// </summary>
        public WebPermission WebPermission
        {
            get { return _webPermission; }
            set { Set(() => WebPermission, ref _webPermission, value); }
        }

        /// <summary>
        /// Default: None
        /// Always: Directories.DataDirectory
        /// </summary>
        public FileIOPermission FileIoPermission
        {
            get { return _fileIoPermission; }
            set { Set(() => FileIoPermission, ref _fileIoPermission, value); }
        }

        /// <summary>
        /// Default: None
        /// </summary>
        public MediaPermission MediaPermission
        {
            get { return _mediaPermission; }
            set { Set(() => MediaPermission, ref _mediaPermission, value); }
        }


        public SecuritySettings()
        {
            SocketPermission = new SocketPermission(PermissionState.None);

            WebPermission = new WebPermission(PermissionState.None);
            foreach (var uri in  Config.Instance.SelectedProfile.InstalledAssemblies.Select(u => u.Location))
            {
                WebPermission.AddPermission(NetworkAccess.Connect, uri);
            }
           
            FileIoPermission = new FileIOPermission(PermissionState.None);
            FileIoPermission.AddPathList(FileIOPermissionAccess.AllAccess, Directories.DataDirectory);

            MediaPermission = new MediaPermission(PermissionState.None);
        }
    }
}