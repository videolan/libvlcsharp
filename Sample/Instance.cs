using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libvlcsharp;

namespace Sample
{
    public class Instance
    {
        private libvlcsharp.Instance _instance;

        public Instance(int argc, string[] args)
        {
            unsafe
            {
                if (args == null || !args.Any())
                    _instance = libvlc.LibvlcNew(argc, null);
                else
                {
                    fixed (byte* arg0 = Encoding.ASCII.GetBytes(args[0]),
                        arg1 = Encoding.ASCII.GetBytes(args[1]),
                        arg2 = Encoding.ASCII.GetBytes(args[2]))
                    {
                        sbyte*[] arr = { (sbyte*)arg0, (sbyte*)arg1, (sbyte*)arg2 };
                        fixed (sbyte** argv = arr)
                        {
                            _instance = libvlc.LibvlcNew(argc, argv);
                        }
                    }
                }
            }
        }

        public libvlcsharp.Instance NativeReference => _instance;

    }
}
