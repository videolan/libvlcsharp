using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibVLCSharp.Helpers;

namespace LibVLCSharp
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ProgramStructure
    {
        internal readonly int GroupId;
        internal readonly IntPtr Name;
        internal readonly bool  Selected;
        internal readonly bool Scrambled;
    }

    /// <summary>
    /// A program struct with info about the program
    /// </summary>
    public readonly struct Program
    {
        internal Program(int groupId, string? name, bool selected, bool scrambled)
        {
            GroupId = groupId;
            Name = name;
            Selected = selected;
            Scrambled = scrambled;
        }

        /// <summary>
        /// Id used for program selection
        /// </summary>
        public readonly int GroupId;

        /// <summary>
        /// Program name, always valid
        /// </summary>
        public readonly string? Name;

        /// <summary>
        /// True if the program is selected
        /// </summary>
        public readonly bool Selected;

        /// <summary>
        /// True if the program is scrambled
        /// </summary>
        public readonly bool Scrambled;
    }

    /// <summary>
    /// List of programs
    /// </summary>
    public class ProgramList : IEnumerable<Program>
    {
        readonly uint _count;
        readonly Program?[]? _programs;

        struct Native
        {
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_player_programlist_count")]
            internal static extern uint LibVLCPlayerProgramListCount(IntPtr list);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_player_programlist_at")]
            internal static extern IntPtr LibVLCPlayerProgramListAt(IntPtr list, uint index);

            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_player_programlist_delete")]
            internal static extern void LibVLCPlayerProgramListDelete(IntPtr list);
        }

        internal ProgramList(IntPtr programList)
        {
            if (programList == IntPtr.Zero)
                return;

            _count = Native.LibVLCPlayerProgramListCount(programList);
            _programs = new Program?[_count];

            for (var i = 0; i < _count; i++)
            {
                var program = Native.LibVLCPlayerProgramListAt(programList, (uint)i);
                _programs[i] = MarshalExtensions.BuildProgram(program);
            }

            Native.LibVLCPlayerProgramListDelete(programList);
        }

        /// <summary>
        /// Get count on program list items.
        /// </summary>
        public uint Count => _count;

        /// <summary>
        /// Gets the element at the specified index
        /// </summary>
        /// <param name="position">position in array where to insert</param>
        /// <returns>program instance at position, or null if not found.</returns>
        public Program? this[int position] => _programs?[position];

        /// <summary>
        /// </summary>
        public IEnumerator<Program> GetEnumerator() => new ProgramEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal class ProgramEnumerator : IEnumerator<Program>
        {
            int _position = -1;
            ProgramList? _programList;
            internal ProgramEnumerator(ProgramList programList)
            {
                _programList = programList;
            }
            object IEnumerator.Current => Current;
            public Program Current
            {
                get
                {
                    if(_programList == null)
                    {
                        throw new ObjectDisposedException(nameof(ProgramEnumerator));
                    }
                    return _programList[_position] ?? throw new ArgumentOutOfRangeException(nameof(_position));
                }
            }

            public bool MoveNext()
            {
                _position++;
                return _position < (_programList?.Count ?? 0);
            }

            public void Reset() => _position = -1;

            public void Dispose()
            {
                _position = -1;
                _programList = default;
            }
        }
    }
}
