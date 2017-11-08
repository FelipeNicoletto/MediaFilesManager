using System;
using MediaFilesManager.Abstractions;

namespace MediaFilesManager
{
    public static class CrossMediaFileManager
    {
        static Lazy<IMediaFileManager> implementation = new Lazy<IMediaFileManager>(CreateCrossMediaAssetManager, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static bool IsSupported => implementation.Value == null ? false : true;

        public static IMediaFileManager Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IMediaFileManager CreateCrossMediaAssetManager()
        {
#if NETSTANDARD1_0
            return null;
#else
            return new MediaFileManager();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
