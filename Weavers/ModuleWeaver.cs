using Fody;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace Weavers
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            foreach (var type in ModuleDefinition.Types)
            {
                foreach (var f in type.Fields.Where(IsEventHandler))
                {
                    System.Diagnostics.Debug.WriteLine(f.FullName + " is now `static`");
                    f.IsStatic = true;
                }

                foreach (var m in type.Methods.Where(NeedStaticKeyword))
                {
                    System.Diagnostics.Debug.WriteLine(m.FullName + " is now `static`");
                    m.IsStatic = true;
                }

                //TODO: replace this to null in IL
            }
        }

        TypeDefinition PInvokeAttribute => ModuleDefinition.Types.First(t => t.Name.Equals("MonoPInvokeCallbackAttribute"));
        TypeDefinition AOTAttribute => ModuleDefinition.Types.First(t => t.Name.Equals("AOT"));

        bool NeedStaticKeyword(MethodDefinition methodDefinition)
            => HasPInvokeAttribute(methodDefinition);// || HasAOTAttribute(methodDefinition);

        bool HasPInvokeAttribute(MethodDefinition methodDefinition)
            => methodDefinition.CustomAttributes.Any(a => a.AttributeType.Name.Equals(PInvokeAttribute.Name));

        bool HasAOTAttribute(MethodDefinition methodDefinition) 
            => methodDefinition.CustomAttributes.Any(a => a.AttributeType.Name.Equals(AOTAttribute.Name));
        
        bool IsEventHandler(FieldDefinition fieldDefinition)
            => fieldDefinition.FieldType.GetElementType().FullName.Equals("System.EventHandler`1");
        
        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
        }

        public override bool ShouldCleanReference => true;
    }
}