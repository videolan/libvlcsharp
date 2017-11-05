using System.Linq;
using CppSharp.AST;
using CppSharp.Passes;
using ASTContext = CppSharp.AST.ASTContext;

namespace libvlcsharp
{
    internal class MoveCustomFunctionToClassPass : TranslationUnitPass
    {
        public override bool VisitFunctionDecl(Function function)
        {
            if (function.Name == "libvlc_new")
            {
                var instance = ASTContext.FindClass("libvlc_instance_t").Single();
                MoveFunction(function, instance);
            }
                    
            return true;
        }

        private static void MoveFunction(Function function, Class @class)
        {
            // TODO: Need to make a constructor which passes its params to the native libvlc_new call and store the returned pointer in field
            // TODO: also add finalizer with libvlc_release and pass the pointer
            var method = new Method(function)
            {
                Namespace = @class,
                Kind = CXXMethodKind.Constructor,
                        
            };
            //function.ExplicitlyIgnore();

              
            //@class.Methods.Add(method);
            //@class.Functions.Add(function);

            function.Namespace = @class;
            function.OriginalNamespace = @class;                    
            function.Access = AccessSpecifier.Internal;
        }
    }
}