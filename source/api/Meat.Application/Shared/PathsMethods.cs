using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Meat.Application.Shared.Settings;
using System;
using System.IO;
using System.Text;

namespace Meat.Application.Shared
{
    public class PathsMethods
    {
        private readonly Directories Directories;

        public PathsMethods(IOptions<Directories> directories)
        {
            Directories = directories.Value;
        }

        public string Directory_SAPVENTA_RUT
        {
            get
            {
                var path = Directories.POSBI_CLIENT_SAPVENTA_RUT;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public string Directory_CERTIFICADO_FE
        {
            get
            {
                var path = Directories.CERTIFICADO_FE;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public string Directory_CLISITEFPLUS_RUT
        {
            get
            {
                var path = Directories.POSBI_CLIENT_CLISITEFPLUS_RUT;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        public string Directory_CLISITEFPLUS_LOCAL
        {
            get
            {
                var path = Directories.POSBI_CLIENT_CLISITEFPLUS_LOCAL;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }
        
        public string Directory_POSBI_SYNC_ARTICULOS_PRECIOS
        {
            get
            {
                var path = Directories.POSBI_SYNC_ARTICULOS_PRECIOS;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }
    }
}