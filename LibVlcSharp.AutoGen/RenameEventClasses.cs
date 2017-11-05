using System.Linq;
using CppSharp.AST;
using CppSharp.Passes;

namespace libvlcsharp
{
    internal class RenameEventClasses : TranslationUnitPass
    {
        public override bool VisitClassDecl(Class @class)
        {
            if (@class.Name == string.Empty && @class.Fields.Count == 1)
            {
                @class.Name = @class.Fields.First().Name;
                return true;
            }
            return base.VisitClassDecl(@class);
        }
    }
}