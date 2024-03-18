#pragma warning disable IDE1006 // Naming Styles (interface for native functions)
using System;
using System.Runtime.InteropServices;

namespace Python.Runtime.Platform
{
    interface ILibDL
    {
        IntPtr dlopen(string fileName, int flags);
        IntPtr dlsym(IntPtr handle, string symbol);
        int dlclose(IntPtr handle);
        IntPtr dlerror();

        int RTLD_NOW { get; }
        int RTLD_GLOBAL { get; }
        IntPtr RTLD_DEFAULT { get; }
    }

    class LinuxLibDL : ILibDL
    {
        protected const string NativeDll = "libdl.so";

        public virtual int RTLD_NOW => 0x2;
        public virtual int RTLD_GLOBAL => 0x100;
        public IntPtr RTLD_DEFAULT => IntPtr.Zero;

        public static ILibDL GetInstance()
        {
            try
            {
                ILibDL libdl2 = new LinuxLibDL2();
                // call dlerror to ensure library is resolved
                libdl2.dlerror();
                return libdl2;
            } catch (DllNotFoundException)
            {
                return new LinuxLibDL();
            }
        }

        IntPtr ILibDL.dlopen(string fileName, int flags) => dlopen(fileName, flags);
        IntPtr ILibDL.dlsym(IntPtr handle, string symbol) => dlsym(handle, symbol);
        int ILibDL.dlclose(IntPtr handle) => dlclose(handle);
        IntPtr ILibDL.dlerror() => dlerror();

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int dlclose(IntPtr handle);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlerror();
    }

    class LinuxLibDL2 : ILibDL
    {
        private const string NativeDll = "libdl.so.2";

        public int RTLD_NOW => 0x2;
        public int RTLD_GLOBAL => 0x100;
        public IntPtr RTLD_DEFAULT => IntPtr.Zero;

        IntPtr ILibDL.dlopen(string fileName, int flags) => dlopen(fileName, flags);
        IntPtr ILibDL.dlsym(IntPtr handle, string symbol) => dlsym(handle, symbol);
        int ILibDL.dlclose(IntPtr handle) => dlclose(handle);
        IntPtr ILibDL.dlerror() => dlerror();

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int dlclose(IntPtr handle);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlerror();
    }

    class MacLibDL : ILibDL
    {
        public int RTLD_NOW => 0x2;
        public int RTLD_GLOBAL => 0x8;
        const string NativeDll = "/usr/lib/libSystem.dylib";
        public IntPtr RTLD_DEFAULT => new(-2);

        IntPtr ILibDL.dlopen(string fileName, int flags) => dlopen(fileName, flags);
        IntPtr ILibDL.dlsym(IntPtr handle, string symbol) => dlsym(handle, symbol);
        int ILibDL.dlclose(IntPtr handle) => dlclose(handle);
        IntPtr ILibDL.dlerror() => dlerror();

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int dlclose(IntPtr handle);

        [DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlerror();
    }

    class AndroidLibDL : LinuxLibDL {
        public override int RTLD_NOW => 0x0;
        public override int RTLD_GLOBAL => 0x00002;
        public new static ILibDL GetInstance() => new AndroidLibDL();

        // TODO Implement android_dlopen_ext to be able to load libraries from arbitrary namespace/location (not only those bundled within apk itself)

        //enum NAMESPACE_TYPE
        //{
        //    ANDROID_NAMESPACE_TYPE_REGULAR = 0,
        //    ANDROID_NAMESPACE_TYPE_ISOLATED = 1,
        //    ANDROID_NAMESPACE_TYPE_SHARED = 2,
        //    ANDROID_NAMESPACE_TYPE_SHARED_ISOLATED = ANDROID_NAMESPACE_TYPE_SHARED |
        //                                             ANDROID_NAMESPACE_TYPE_ISOLATED,
        //};

        //enum DLEXT_FLAG
        //{
        //    ANDROID_DLEXT_RESERVED_ADDRESS = 0x1,
        //    ANDROID_DLEXT_RESERVED_ADDRESS_HINT = 0x2,
        //    ANDROID_DLEXT_WRITE_RELRO = 0x4,
        //    ANDROID_DLEXT_USE_RELRO = 0x8,
        //    ANDROID_DLEXT_USE_LIBRARY_FD = 0x10,
        //    ANDROID_DLEXT_USE_LIBRARY_FD_OFFSET = 0x20,
        //    ANDROID_DLEXT_FORCE_LOAD = 0x40,
        //    ANDROID_DLEXT_FORCE_FIXED_VADDR = 0x80,
        //    ANDROID_DLEXT_LOAD_AT_FIXED_ADDRESS = 0x100,
        //    ANDROID_DLEXT_USE_NAMESPACE = 0x200,
        //    ANDROID_DLEXT_VALID_FLAG_BITS = ANDROID_DLEXT_RESERVED_ADDRESS |
        //                                          ANDROID_DLEXT_RESERVED_ADDRESS_HINT |
        //                                          ANDROID_DLEXT_WRITE_RELRO |
        //                                          ANDROID_DLEXT_USE_RELRO |
        //                                          ANDROID_DLEXT_USE_LIBRARY_FD |
        //                                          ANDROID_DLEXT_USE_LIBRARY_FD_OFFSET |
        //                                          ANDROID_DLEXT_FORCE_LOAD |
        //                                          ANDROID_DLEXT_FORCE_FIXED_VADDR |
        //                                          ANDROID_DLEXT_LOAD_AT_FIXED_ADDRESS |
        //                                          ANDROID_DLEXT_USE_NAMESPACE,
        //};

        //struct DLEXT_INFO
        //{
        //    ulong flags;
        //    int library_fd;
        //    ulong library_fd_offset;
        //    IntPtr? library_namespace;
        //    int relro_fd;
        //    IntPtr reserved_addr;
        //    int reserved_size;
        //}

        //[DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private static extern IntPtr android_dlopen_ext(string? fileName, int flags, DLEXT_INFO? info);

        //[DllImport(NativeDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private static extern IntPtr android_create_namespace(string? name, string? ld_library_path, string? default_library_path, ulong type, string? permitted_when_isolated_path);
    }
}
